using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Project.Application.Abstractions.Persistence;
using Project.Application.Abstractions.Services;
using Project.Application.DTOs.Student;
using Project.Application.Services;
using Project.Application.Validators.Student;
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
            services.AddScoped<IValidator<CreateStudentDto>, CreateStudentValidator>();

            // Application services

            
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<IMenuSettingService, MenuSettingService>();
 
            return services;
        }
    }
}
