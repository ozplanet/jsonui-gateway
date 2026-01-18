using JsonUi.Gateway.Logging;

namespace JsonUi.Gateway.Services;

public interface ILoggingService
{
    LoggingOptions GetOptions();
    Task UpdateAsync(LoggingOptions options, CancellationToken cancellationToken = default);
}
