using System.Text.Json.Serialization;
using Project.API.Converters;
using Project.API.Extensions;
using Project.API.Handlers;
using Project.API.Versioning;

namespace Project.API.DependencyInjection
{
    public static class PresentationServiceRegistration
    {
        public static IServiceCollection AddPresentationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddControllersWithViews()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new UtcDateTimeJsonConverter());
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });
            services.AddEndpointsApiExplorer();

            services.AddApiVersioningConfig();
            services.AddSwaggerConfig();
          //  services.AddCorsPolicies(configuration);

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddHttpContextAccessor();

            return services;
        }
    }
}
