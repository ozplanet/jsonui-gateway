namespace JsonUi.Gateway.Contracts.Admin;

public sealed record SecretCreateRequest(string Name, string Value);

public sealed record SecretResponse(Guid Id, string Name, DateTimeOffset CreatedAt);
