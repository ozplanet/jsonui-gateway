using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Gateway.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JsonUi.Gateway.Controllers;

[ApiController]
[Route("admin/integrations")]
[Authorize(Policy = "Admin")]
public sealed class AdminIntegrationsController : ControllerBase
{
    private readonly IIntegrationRepository _integrations;
    private readonly ISecretRepository _secrets;

    public AdminIntegrationsController(IIntegrationRepository integrations, ISecretRepository secrets)
    {
        _integrations = integrations;
        _secrets = secrets;
    }

    [HttpGet]
    public async Task<IReadOnlyCollection<IntegrationResponse>> ListAsync(CancellationToken cancellationToken)
    {
        var items = await _integrations.ListAsync(cancellationToken);
        return items.Select(Map).ToList();
    }

    [HttpPost]
    public async Task<ActionResult<IntegrationResponse>> CreateAsync([FromBody] IntegrationRequest request, CancellationToken cancellationToken)
    {
        var validation = JsonUi.Core.Validation.IntegrationValidator.ValidateForCreate(request.Name, request.BaseUrl, request.AuthMode);
        if (!validation.IsValid || validation.ParsedUri is null)
        {
            return BadRequest(new { error = validation.Error });
        }

        if (request.SecretId is { } secretId)
        {
            var secret = await _secrets.GetAsync(secretId, cancellationToken);
            if (secret is null)
            {
                return BadRequest(new { error = "Secret not found" });
            }
        }

        var integration = new Integration(request.Name, validation.ParsedUri, request.AuthMode);
        if (request.SecretId is { } secretToAssign)
        {
            integration.SetSecret(secretToAssign);
        }

        integration.Update(request.Name, validation.ParsedUri, request.AuthMode, request.Enabled);
        integration.ReplaceAllowlists(request.Allowlists.Select(MapAllowlist));

        await _integrations.AddAsync(integration, cancellationToken);

        return CreatedAtAction(nameof(GetAsync), new { id = integration.Id }, Map(integration));
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<IntegrationResponse>> GetAsync(Guid id, CancellationToken cancellationToken)
    {
        var integration = await _integrations.GetAsync(id, cancellationToken);
        if (integration is null)
        {
            return NotFound();
        }

        return Map(integration);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<IntegrationResponse>> UpdateAsync(Guid id, [FromBody] IntegrationRequest request, CancellationToken cancellationToken)
    {
        var integration = await _integrations.GetAsync(id, cancellationToken);
        if (integration is null)
        {
            return NotFound();
        }

        var validation = JsonUi.Core.Validation.IntegrationValidator.ValidateForCreate(request.Name, request.BaseUrl, request.AuthMode);
        if (!validation.IsValid || validation.ParsedUri is null)
        {
            return BadRequest(new { error = validation.Error });
        }

        if (request.SecretId is { } secretId)
        {
            var secret = await _secrets.GetAsync(secretId, cancellationToken);
            if (secret is null)
            {
                return BadRequest(new { error = "Secret not found" });
            }

            integration.SetSecret(secretId);
        }
        else
        {
            integration.ClearSecret();
        }

        integration.Update(request.Name, validation.ParsedUri, request.AuthMode, request.Enabled);
        integration.ReplaceAllowlists(request.Allowlists.Select(MapAllowlist));

        await _integrations.UpdateAsync(integration, cancellationToken);
        return Map(integration);
    }

    private static IntegrationAllowlist MapAllowlist(AllowlistEntryRequest entry)
    {
        var kind = entry.Kind?.ToLowerInvariant() == "cidr" ? AllowlistKind.Cidr : AllowlistKind.Host;
        return new IntegrationAllowlist
        {
            Value = entry.Value,
            Kind = kind
        };
    }

    private static IntegrationResponse Map(Integration integration)
        => new(integration.Id, integration.Name, integration.Slug, integration.BaseUrl.ToString(), integration.AuthMode, integration.Enabled, integration.SecretId,
            integration.Allowlists.Select(a => new AllowlistEntryResponse(a.Id, a.Value, a.Kind.ToString())).ToList());
}
