namespace JsonUi.Gateway.Contracts.Admin;

public sealed record ProxyActionRequest(
    Guid IntegrationId,
    string Name,
    string Method,
    string PathTemplate,
    string? JsonSchema,
    bool Enabled,
    bool CacheEnabled,
    int? CacheTtlSeconds);

public sealed record ProxyActionResponse(
    Guid Id,
    Guid IntegrationId,
    string Name,
    string Slug,
    string Method,
    string PathTemplate,
    string? JsonSchema,
    bool Enabled,
    bool CacheEnabled,
    int? CacheTtlSeconds);
