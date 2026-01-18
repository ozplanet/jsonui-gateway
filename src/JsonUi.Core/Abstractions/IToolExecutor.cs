using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface IToolExecutor
{
    Task<ToolExecutionResult> ExecuteAsync(Integration integration, ProxyAction action, string apiKeyPrefix, object? payload, CancellationToken cancellationToken = default);
}

public sealed record ToolExecutionResult(int StatusCode, IDictionary<string, string> Headers, string Body);
