using System.Net.Http.Headers;
using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using Microsoft.Extensions.Logging;

namespace JsonUi.Infrastructure.Execution;

public sealed class HttpToolExecutor : IToolExecutor
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ISsrfGuard _ssrfGuard;
    private readonly ILogger<HttpToolExecutor> _logger;

    public HttpToolExecutor(IHttpClientFactory httpClientFactory, ISsrfGuard ssrfGuard, ILogger<HttpToolExecutor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _ssrfGuard = ssrfGuard;
        _logger = logger;
    }

    public async Task<ToolExecutionResult> ExecuteAsync(Integration integration, ProxyAction action, string apiKeyPrefix, object? payload, CancellationToken cancellationToken = default)
    {
        var target = new Uri(integration.BaseUrl, action.PathTemplate);
        await _ssrfGuard.EnsureAllowedAsync(target, integration.Allowlists, cancellationToken);

        var client = _httpClientFactory.CreateClient("proxy");
        using var request = new HttpRequestMessage(new HttpMethod(action.Method), target);
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        request.Headers.TryAddWithoutValidation("X-JsonUi-Caller", apiKeyPrefix);

        if (payload != null)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(payload);
            request.Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
        }

        var response = await client.SendAsync(request, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        var headers = response.Headers.Concat(response.Content.Headers)
            .Where(h => !HeaderBlacklist.Contains(h.Key))
            .ToDictionary(h => h.Key, h => string.Join(", ", h.Value));

        _logger.LogInformation("Proxy action {Action} returned {Status}", action.Slug, response.StatusCode);

        return new ToolExecutionResult((int)response.StatusCode, headers, body);
    }

    private static readonly HashSet<string> HeaderBlacklist = new(StringComparer.OrdinalIgnoreCase)
    {
        "Authorization",
        "Cookie",
        "Set-Cookie",
        "X-Api-Key"
    };
}
