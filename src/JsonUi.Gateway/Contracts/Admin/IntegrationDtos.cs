namespace JsonUi.Gateway.Contracts.Admin;

public sealed record IntegrationRequest(
    string Name,
    string BaseUrl,
    string AuthMode,
    bool Enabled,
    Guid? SecretId,
    IReadOnlyCollection<AllowlistEntryRequest> Allowlists);

public sealed record AllowlistEntryRequest(string Value, string Kind);

public sealed record IntegrationResponse(
    Guid Id,
    string Name,
    string Slug,
    string BaseUrl,
    string AuthMode,
    bool Enabled,
    Guid? SecretId,
    IReadOnlyCollection<AllowlistEntryResponse> Allowlists);

public sealed record AllowlistEntryResponse(Guid Id, string Value, string Kind);
