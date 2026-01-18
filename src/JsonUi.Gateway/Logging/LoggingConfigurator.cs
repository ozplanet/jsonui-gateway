using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace JsonUi.Gateway.Logging;

public static class LoggingConfigurator
{
    private static readonly object SyncRoot = new();
    private static string _configPath = "/app/data/logging.settings.json";
    private static LoggingOptions _currentOptions = new();

    public static string ConfigPath => _configPath;
    public static LoggingOptions CurrentOptions => _currentOptions;

    public static void ConfigureSerilog(WebApplicationBuilder builder)
    {
        _configPath = builder.Configuration["JSONUI_LOGGING_CONFIG_PATH"] ?? _configPath;
        EnsureConfigFileRegistered(builder.Configuration);

        var options = new LoggingOptions();
        builder.Configuration.GetSection("JsonUi:Logging").Bind(options);
        _currentOptions = options;

        ApplyLogger(options, builder);
    }

    public static void UpdateOptions(LoggingOptions options)
    {
        _currentOptions = options;
        ApplyLogger(options, null);
    }

    private static void ApplyLogger(LoggingOptions options, WebApplicationBuilder? builder)
    {
        lock (SyncRoot)
        {
            var loggerConfiguration = BuildLoggerConfiguration(options);
            Log.Logger = loggerConfiguration.CreateLogger();
            if (builder is not null)
            {
                builder.Host.UseSerilog();
            }
        }
    }

    private static LoggerConfiguration BuildLoggerConfiguration(LoggingOptions options)
    {
        var loggerConfiguration = new LoggerConfiguration()
            .MinimumLevel.Is(ParseLevel(options.MinimumLevel))
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(formatProvider: System.Globalization.CultureInfo.InvariantCulture);

        if (options.File.Enabled)
        {
            var directory = Path.GetDirectoryName(options.File.Path);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            loggerConfiguration = loggerConfiguration.WriteTo.File(
                formatter: new CompactJsonFormatter(),
                path: options.File.Path,
                rollingInterval: RollingInterval.Day,
                fileSizeLimitBytes: options.File.FileSizeLimitBytes,
                rollOnFileSizeLimit: options.File.RollOnFileSizeLimit,
                retainedFileCountLimit: 7,
                shared: true,
                flushToDiskInterval: TimeSpan.FromSeconds(5));
        }

        if (options.ApplicationInsights.Enabled && !string.IsNullOrWhiteSpace(options.ApplicationInsights.ConnectionString))
        {
            loggerConfiguration = loggerConfiguration.WriteTo.ApplicationInsights(
                new TelemetryConfiguration
                {
                    ConnectionString = options.ApplicationInsights.ConnectionString
                },
                TelemetryConverter.Traces);
        }

        return loggerConfiguration;
    }

    private static void EnsureConfigFileRegistered(ConfigurationManager configuration)
    {
        if (!string.IsNullOrWhiteSpace(_configPath))
        {
            var directory = Path.GetDirectoryName(_configPath);
            if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            configuration.AddJsonFile(_configPath, optional: true, reloadOnChange: true);
        }
    }

    private static LogEventLevel ParseLevel(string level)
        => Enum.TryParse<LogEventLevel>(level, ignoreCase: true, out var parsed)
            ? parsed
            : LogEventLevel.Information;
}
