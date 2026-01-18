using System.Security.Claims;
using JsonUi.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;

namespace JsonUi.Gateway.Authorization;

public sealed class ScopeRequirement : IAuthorizationRequirement
{
    public ScopeRequirement(Guid? integrationId = null, Guid? actionId = null)
    {
        IntegrationId = integrationId;
        ActionId = actionId;
    }

    public Guid? IntegrationId { get; }
    public Guid? ActionId { get; }
}

public sealed class ScopeAuthorizationHandler : AuthorizationHandler<ScopeRequirement>
{
    private readonly IApiKeyRepository _repository;

    public ScopeAuthorizationHandler(IApiKeyRepository repository)
    {
        _repository = repository;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, ScopeRequirement requirement)
    {
        var prefix = context.User.FindFirst("prefix")?.Value;
        if (string.IsNullOrWhiteSpace(prefix))
        {
            return;
        }

        var apiKey = await _repository.FindByPrefixAsync(prefix);
        if (apiKey is null)
        {
            return;
        }

        if (apiKey.IsAdmin)
        {
            context.Succeed(requirement);
            return;
        }

        if (requirement.ActionId is { } actionId)
        {
            if (apiKey.Scopes.Any(s => s.ActionId == actionId))
            {
                context.Succeed(requirement);
                return;
            }
        }

        if (requirement.IntegrationId is { } integrationId)
        {
            if (apiKey.Scopes.Any(s => s.IntegrationId == integrationId))
            {
                context.Succeed(requirement);
                return;
            }
        }
    }
}
