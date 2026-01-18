using JsonUi.Core.Abstractions;
using JsonUi.Gateway.Contracts.Mcp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("mcp")]
[Authorize]
public sealed class McpController : ControllerBase
{
    private readonly IIntegrationRepository _integrations;
    private readonly IProxyActionRepository _actions;
    private readonly IToolExecutor _executor;
    private readonly IApiKeyRepository _apiKeyRepository;

    public McpController(IIntegrationRepository integrations, IProxyActionRepository actions, IToolExecutor executor, IApiKeyRepository apiKeyRepository)
    {
        _integrations = integrations;
        _actions = actions;
        _executor = executor;
        _apiKeyRepository = apiKeyRepository;
    }

    [HttpGet("tools")]
    public async Task<IReadOnlyCollection<McpToolResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var prefix = User.FindFirst("prefix")?.Value ?? string.Empty;
        var apiKey = await _apiKeyRepository.FindByPrefixAsync(prefix, cancellationToken);
        if (apiKey is null)
        {
            return Array.Empty<McpToolResponse>();
        }

        var integrations = await _integrations.ListAsync(cancellationToken);
        var responses = new List<McpToolResponse>();
        foreach (var integration in integrations.Where(i => i.Enabled))
        {
            var integrationActions = await _actions.ListByIntegrationAsync(integration.Id, cancellationToken);
            foreach (var action in integrationActions.Where(a => a.Enabled))
            {
                if (IsActionAllowed(apiKey, integration.Id, action.Id))
                {
                    responses.Add(new McpToolResponse(action.Id, integration.Id, action.Name, action.Slug, action.Method, action.PathTemplate, action.JsonSchema));
                }
            }
        }

        return responses;
    }

    [HttpPost("call/{actionId:guid}")]
    public async Task<ActionResult<McpToolResult>> CallAsync(Guid actionId, [FromBody] object payload, CancellationToken cancellationToken)
    {
        var action = await _actions.GetAsync(actionId, cancellationToken);
        if (action is null)
        {
            return NotFound();
        }

        var integration = await _integrations.GetAsync(action.IntegrationId, cancellationToken);
        if (integration is null)
        {
            return NotFound();
        }

        var prefix = User.FindFirst("prefix")?.Value ?? string.Empty;
        var apiKey = await _apiKeyRepository.FindByPrefixAsync(prefix, cancellationToken);
        if (apiKey is null)
        {
            return Forbid();
        }

        if (!IsActionAllowed(apiKey, integration.Id, action.Id))
        {
            return Forbid();
        }

        var result = await _executor.ExecuteAsync(integration, action, prefix, payload, cancellationToken);
        return new McpToolResult(result.StatusCode, result.Headers, result.Body);
    }

    private static bool IsActionAllowed(JsonUi.Core.Entities.ApiKey apiKey, Guid integrationId, Guid actionId)
    {
        if (apiKey.IsAdmin)
        {
            return true;
        }

        if (apiKey.Scopes.Any(s => s.ActionId == actionId))
        {
            return true;
        }

        if (apiKey.Scopes.Any(s => s.IntegrationId == integrationId))
        {
            return true;
        }

        return false;
    }
}
