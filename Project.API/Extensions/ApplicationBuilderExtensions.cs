using Project.Infrastructure.DbInitializers;

namespace Project.API.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> UseDbInitializerAsync(
            this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var initializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
            await initializer.InitializeAsync();
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
