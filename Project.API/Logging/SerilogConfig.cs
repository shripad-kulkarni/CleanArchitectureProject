using Serilog;

namespace Project.API.Logging
{
    public static class SerilogConfig
    {
        public static void Configure(IHostBuilder host)
        {
            host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .MinimumLevel.Information()
                    .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    //.Enrich.WithMachineName()
                    // .Enrich.WithEnvironmentName()
                    .WriteTo.Console(outputTemplate:
                        "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {NewLine}{Exception}")
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 30,
                        outputTemplate:
                            "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            });
        }
    }
}

