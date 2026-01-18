using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Gateway.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("admin/actions")]
[Authorize(Policy = "Admin")]
public sealed class AdminActionsController : ControllerBase
{
    private readonly IProxyActionRepository _actions;
    private readonly IIntegrationRepository _integrations;

    public AdminActionsController(IProxyActionRepository actions, IIntegrationRepository integrations)
    {
        _actions = actions;
        _integrations = integrations;
    }

    [HttpPost]
    public async Task<ActionResult<ProxyActionResponse>> CreateAsync([FromBody] ProxyActionRequest request, CancellationToken cancellationToken)
    {
        var integration = await _integrations.GetAsync(request.IntegrationId, cancellationToken);
        if (integration is null)
        {
            return BadRequest(new { error = "Integration not found" });
        }

        var action = new ProxyAction(request.IntegrationId, request.Name, request.Method, request.PathTemplate);
        action.Update(request.Name, request.Method, request.PathTemplate, request.JsonSchema, request.Enabled, request.CacheEnabled, ToTimeSpan(request.CacheTtlSeconds));

        await _actions.AddAsync(action, cancellationToken);

        return CreatedAtAction(nameof(GetAsync), new { id = action.Id }, Map(action));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProxyActionResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var action = await _actions.GetAsync(id, cancellationToken);
        if (action is null)
        {
            return NotFound();
        }

        return Map(action);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProxyActionResponse>> UpdateAsync(Guid id, [FromBody] ProxyActionRequest request, CancellationToken cancellationToken)
    {
        var action = await _actions.GetAsync(id, cancellationToken);
        if (action is null)
        {
            return NotFound();
        }

        var integration = await _integrations.GetAsync(request.IntegrationId, cancellationToken);
        if (integration is null)
        {
            return BadRequest(new { error = "Integration not found" });
        }

        action.Update(request.Name, request.Method, request.PathTemplate, request.JsonSchema, request.Enabled, request.CacheEnabled, ToTimeSpan(request.CacheTtlSeconds));
        await _actions.UpdateAsync(action, cancellationToken);
        return Map(action);
    }

    private static TimeSpan? ToTimeSpan(int? seconds) => seconds.HasValue ? TimeSpan.FromSeconds(Math.Max(0, seconds.Value)) : null;

    private static ProxyActionResponse Map(ProxyAction action)
        => new(action.Id, action.IntegrationId, action.Name, action.Slug, action.Method, action.PathTemplate, action.JsonSchema, action.Enabled, action.CacheEnabled, (int?)action.CacheTtl?.TotalSeconds);
}
