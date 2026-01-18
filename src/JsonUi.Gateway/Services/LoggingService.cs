using System.Text.Json;
using JsonUi.Gateway.Logging;
using Microsoft.Extensions.Options;

namespace JsonUi.Gateway.Services;

public sealed class LoggingService : ILoggingService
{
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        WriteIndented = true
    };

    private readonly IOptionsMonitor<LoggingOptions> _options;

    public LoggingService(IOptionsMonitor<LoggingOptions> options)
    {
        _options = options;
    }

    public LoggingOptions GetOptions() => _options.CurrentValue;

    public async Task UpdateAsync(LoggingOptions options, CancellationToken cancellationToken = default)
    {
        await PersistAsync(options, cancellationToken);
        LoggingConfigurator.UpdateOptions(options);
    }

    private static Task PersistAsync(LoggingOptions options, CancellationToken cancellationToken)
    {
        var path = LoggingConfigurator.ConfigPath;
        var directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var payload = JsonSerializer.Serialize(new { JsonUi = new { Logging = options } }, SerializerOptions);
        return File.WriteAllTextAsync(path, payload, cancellationToken);
    }
}
