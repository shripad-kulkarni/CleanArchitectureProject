using Microsoft.Extensions.FileProviders;
using Project.API.Extensions;
using Project.API.Logging;
using Project.API.DependencyInjection;
using Project.Application.DependencyInjection;
using Project.Infrastructure;

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

// Serve default wwwroot static files
app.UseStaticFiles();

// Serve uploaded files (photos, videos, documents) from the configured uploads folder
var uploadsPath = builder.Configuration["FileStorage:BasePath"] ?? Path.Combine(builder.Environment.ContentRootPath, "Uploads");
Directory.CreateDirectory(uploadsPath);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/uploads",
});

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.UseExceptionHandler();
app.MapControllers();
app.MapNotificationHub();

app.Run();
