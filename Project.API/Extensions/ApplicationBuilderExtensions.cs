using Microsoft.AspNetCore.Identity;
using Project.Domain.Aggregates;
using Project.Infrastructure.Hubs;
using Project.Infrastructure.Identity;
using Project.Infrastructure.Persistence;

namespace Project.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IEndpointRouteBuilder MapNotificationHub(this IEndpointRouteBuilder app)
        {
            app.MapHub<NotificationHub>("/hubs/notifications");
            return app;
        }

        public static IEndpointRouteBuilder MapChatHub(this IEndpointRouteBuilder app)
        {
            app.MapHub<ChatHub>("/hubs/chat");
            return app;
        }

        public static async Task<IApplicationBuilder> UseDbInitializerAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            await DbInitializer.InitializeAsync(db, roleManager, userManager);
            return app;
        }

        public static IApplicationBuilder UseSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "no-referrer");
                await next();
            });

            return app;
        }
    }
}
