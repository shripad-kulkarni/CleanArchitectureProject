using Project.API.Extensions;
using Project.API.Logging;
using Project.Infrastructure.DependencyInjection;
using Project.API.DependencyInjection;
using Project.Application.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

SerilogConfig.Configure(builder.Host);

builder.Services
    .AddInfrastructureServices(builder.Configuration)
    .AddApplicationServices(builder.Configuration)
    .AddPresentationServices(builder.Configuration);

var app = builder.Build();

await app.UseDbInitializerAsync();

if (app.Environment.IsDevelopment())
    app.UseSwaggerConfig();

app.UseSecurityHeaders();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();

app.Run();
