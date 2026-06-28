using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Persistence;
using Project.Domain.Aggregates;
using Project.Domain.Aggregates.UserAggregate;
using Project.Domain.Enums;
using Project.Infrastructure.Options;
using System.Text;

namespace Project.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IOptions<FrontendOptions> _frontendOptions;
        private readonly IRepository<User> _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService,
            IOptions<FrontendOptions> frontendOptions,
            IRepository<User> userRepository,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _frontendOptions = frontendOptions;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AuthResult> LoginAsync(
            string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return new AuthResult(false, null, "Invalid credentials.");

            if (!user.EmailConfirmed)
                return new AuthResult(false, null, "Please verify your email before signing in. Check your inbox for the verification link.");

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, lockoutOnFailure: false);
            if (!result.Succeeded)
                return new AuthResult(false, null, "Invalid credentials.");

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return new AuthResult(true, token, null,
                Id: user.Id,
                Name: $"{user.FirstName} {user.LastName}".Trim(),
                Email: user.Email);
        }

        public async Task<AuthResult> RegisterAsync(
            string email, string password,
            string firstName, string lastName,
            string role, string? phone = null, CancellationToken ct = default)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null)
                return new AuthResult(false, null, "Email already in use.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                PhoneNumber = phone,
                IsActive = true
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
                return new AuthResult(false, null, string.Join(", ", result.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);

            var rawToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            var encodedEmail = Uri.EscapeDataString(email);
            var link = $"{_frontendOptions.Value.BaseUrl}/verify-email?email={encodedEmail}&token={encodedToken}";

            var body = $"""
                <div style="font-family:sans-serif;max-width:480px;margin:auto">
                  <h2 style="color:#1e40af">Verify Your Email</h2>
                  <p>Hello {firstName},</p>
                  <p>Thanks for registering! Click the button below to verify your email address and activate your account.</p>
                  <p style="margin:24px 0">
                    <a href="{link}" style="background:#1e40af;color:#fff;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block">Verify Email</a>
                  </p>
                  <p style="color:#888;font-size:0.85em">This link expires in 24 hours. If you did not create an account, you can safely ignore this email.</p>
                </div>
                """;

            await _emailService.SendAsync(email, "Verify Your Email Address", body, ct);

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> InviteUserAsync(
            string email, string firstName, string lastName, string role, CancellationToken ct = default)
        {
            var existing = await _userManager.FindByEmailAsync(email);
            if (existing is not null)
                return new AuthResult(false, null, "Email already in use.");

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                IsActive = true,
                EmailConfirmed = true
            };

            // Placeholder password satisfying Identity requirements (user will set their own via the invite link)
            var placeholder = "Sch00l!" + Random.Shared.Next(1000,9999);
            var createResult = await _userManager.CreateAsync(user, placeholder);
            if (!createResult.Succeeded)
                return new AuthResult(false, null, string.Join(", ", createResult.Errors.Select(e => e.Description)));

            await _userManager.AddToRoleAsync(user, role);

            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            var encodedEmail = Uri.EscapeDataString(email);
            var link = $"{_frontendOptions.Value.BaseUrl}/set-password?email={encodedEmail}&token={encodedToken}";

            var body = $"""
                <div style="font-family:sans-serif;max-width:480px;margin:auto">
                  <h2 style="color:#2196F3">Welcome</h2>
                  <p>Hello {firstName},</p>
                  <p>Your account has been created as <strong>{role}</strong>. Click the button below to set your password and activate your account.</p>
                  <p style="margin:24px 0">
                    <a href="{link}" style="background:#2196F3;color:#fff;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block">Set My Password</a>
                  </p>
                  <p style="color:#888;font-size:0.85em">This link expires in 24 hours. If you did not expect this email, please ignore it.</p>
                </div>
                """;

            await _emailService.SendAsync(email, "Welcome – Set Your Password", body, ct);

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> SetUserActiveAsync(string email, bool isActive, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return new AuthResult(false, null, "User not found.");

            user.IsActive = isActive;
            await _userManager.UpdateAsync(user);

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken ct = default)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is null || !user.IsActive)
                return new AuthResult(false, null, "User not found.");

            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (!result.Succeeded)
                return new AuthResult(false, null, string.Join(", ", result.Errors.Select(e => e.Description)));

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> VerifyEmailAsync(string email, string token, CancellationToken ct = default)
        {
            var identityUser = await _userManager.FindByEmailAsync(email);
            if (identityUser is null)
                return new AuthResult(false, null, "Invalid verification link.");

            if (identityUser.EmailConfirmed)
                return new AuthResult(true, null, null);

            string rawToken;
            try
            {
                rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                return new AuthResult(false, null, "Invalid or expired verification link.");
            }

            var result = await _userManager.ConfirmEmailAsync(identityUser, rawToken);
            if (!result.Succeeded)
                return new AuthResult(false, null, "Verification failed. The link may have expired.");

            // Create the domain User record so the user appears in the admin panel.
            // Fields not collected at registration use placeholder defaults the user can update via profile.
            var alreadyExists = await _userRepository.ExistsAsync(u => u.Email == email.ToLowerInvariant(), ct);
            if (!alreadyExists)
            {
                var domainUser = new User(
                    firstName: identityUser.FirstName ?? string.Empty,
                    lastName: identityUser.LastName ?? string.Empty,
                    email: email,
                    phone: identityUser.PhoneNumber ?? "-",
                    dateOfBirth: new DateOnly(2000, 1, 1),
                    gender: Gender.Other,
                    street: "-",
                    city: "-",
                    state: "-",
                    pinCode: "-");

                await _userRepository.AddAsync(domainUser, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> ForgotPasswordAsync(string email, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // Always return success — never reveal whether the email exists
            if (user is null || !user.IsActive)
                return new AuthResult(true, null, null);

            var rawToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(rawToken));
            var encodedEmail = Uri.EscapeDataString(email);
            var link = $"{_frontendOptions.Value.BaseUrl}/reset-password?email={encodedEmail}&token={encodedToken}";

            var body = $"""
                <div style="font-family:sans-serif;max-width:480px;margin:auto">
                  <h2 style="color:#1e40af">Reset Your Password</h2>
                  <p>Hello {user.FirstName},</p>
                  <p>We received a request to reset your password. Click the button below to choose a new one.</p>
                  <p style="margin:24px 0">
                    <a href="{link}" style="background:#1e40af;color:#fff;padding:12px 24px;text-decoration:none;border-radius:6px;display:inline-block">Reset Password</a>
                  </p>
                  <p style="color:#888;font-size:0.85em">This link expires in 24 hours. If you did not request a password reset, you can safely ignore this email.</p>
                </div>
                """;

            await _emailService.SendAsync(email, "Reset Your Password", body, ct);

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> SetPasswordAsync(
            string email, string token, string newPassword, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null)
                return new AuthResult(false, null, "Invalid request.");

            string rawToken;
            try
            {
                rawToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
            }
            catch
            {
                return new AuthResult(false, null, "Invalid or expired link.");
            }

            var result = await _userManager.ResetPasswordAsync(user, rawToken, newPassword);
            if (!result.Succeeded)
                return new AuthResult(false, null, string.Join(", ", result.Errors.Select(e => e.Description)));

            return new AuthResult(true, null, null);
        }

        public async Task<AuthResult> RefreshTokenAsync(string token, CancellationToken ct = default)
        {
            return new AuthResult(false, null, "Refresh token not implemented.");
        }
    }
}
