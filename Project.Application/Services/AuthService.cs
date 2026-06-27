using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Identity;
using Project.Domain.Aggregates;
using Project.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.Services
{
    public sealed class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IEmailService _emailService;
        private readonly IOptions<FrontendOptions> _frontendOptions;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IJwtTokenGenerator jwtTokenGenerator,
            IEmailService emailService,
            IOptions<FrontendOptions> frontendOptions)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtTokenGenerator;
            _emailService = emailService;
            _frontendOptions = frontendOptions;
        }

        public async Task<AuthResult> LoginAsync(
            string email, string password, CancellationToken ct = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user is null || !user.IsActive)
                return new AuthResult(false, null, "Invalid credentials.");

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

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtTokenGenerator.GenerateToken(user, roles);

            return new AuthResult(true, token, null);
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
