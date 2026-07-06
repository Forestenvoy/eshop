using Serilog;
using Serilog.Events;
using Serilog.Enrichers.WithCaller;

namespace eshop.application.Configurations
{
    public class ConfigureSerilog
    {
        public static void Prepare(string applicationName, IConfiguration configuration)
        {
            if (string.IsNullOrWhiteSpace(applicationName))
            {
                throw new ArgumentNullException(nameof(applicationName));
            }

            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .MinimumLevel.Information()
                .Enrich.WithProperty("AppName", applicationName)
                .Enrich.WithCaller()
                .WriteTo.Debug()
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                    levelSwitch: new Serilog.Core.LoggingLevelSwitch(LogEventLevel.Information)
                )
                .CreateLogger();
        }
    }
}
