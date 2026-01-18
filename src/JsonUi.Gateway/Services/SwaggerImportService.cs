using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Gateway.Contracts.Admin;
using NSwag;
using NSwag.Collections;

namespace JsonUi.Gateway.Services;

public sealed class SwaggerImportService : ISwaggerImportService
{
    private readonly IProxyActionRepository _actions;
    private readonly IIntegrationRepository _integrations;

    public SwaggerImportService(IProxyActionRepository actions, IIntegrationRepository integrations)
    {
        _actions = actions;
        _integrations = integrations;
    }

    public async Task<IReadOnlyCollection<ProxyActionResponse>> ImportAsync(Guid integrationId, Stream swaggerStream, CancellationToken cancellationToken = default)
    {
        var integration = await _integrations.GetAsync(integrationId, cancellationToken);
        if (integration is null)
        {
            throw new InvalidOperationException("Integration not found");
        }

        using var reader = new StreamReader(swaggerStream);
        var swaggerJson = await reader.ReadToEndAsync(cancellationToken);
        var document = await OpenApiDocument.FromJsonAsync(swaggerJson, cancellationToken);
        var responses = new List<ProxyActionResponse>();

        foreach (var pathEntry in document.Paths)
        {
            var path = pathEntry.Key;
            var pathItem = pathEntry.Value;
            foreach (var operationPair in pathItem)
            {
                var method = operationPair.Key.ToString().ToUpperInvariant();
                var operation = operationPair.Value;
                var name = operation.OperationId ?? $"{method}_{path}";
                var action = new ProxyAction(integrationId, name, method, path);
                string? schema = null;
                if (operation.RequestBody?.Content?.TryGetValue("application/json", out var bodyContent) == true)
                {
                    schema = bodyContent.Schema?.ToJson();
                }

                action.Update(name, method, path, schema, enabled: true, cacheEnabled: method == "GET", cacheTtl: null);
                await _actions.AddAsync(action, cancellationToken);
                responses.Add(new ProxyActionResponse(action.Id, action.IntegrationId, action.Name, action.Slug, action.Method, action.PathTemplate, action.JsonSchema, action.Enabled, action.CacheEnabled, (int?)action.CacheTtl?.TotalSeconds));
            }
        }

        return responses;
    }
}
