using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Abstractions.Identity;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.ExternalServices;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.User;
using Project.Application.Services;
using Project.Application.Validators.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Application.DependencyInjection
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {

            // Validators
            services.AddScoped<IValidator<CreateUserDto>, CreateUserValidator>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IInfoSettingsService, InfoSettingsService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMenuSettingService, MenuSettingService>();
            services.AddScoped<IPaymentService, PaymentService>();
 
            return services;
        }
    }
}
