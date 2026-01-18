namespace JsonUi.Core.Entities;

public sealed class IntegrationAllowlist
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid IntegrationId { get; init; }
    public string Value { get; init; } = string.Empty;
    public AllowlistKind Kind { get; init; } = AllowlistKind.Host;
}

public enum AllowlistKind
{
    Host = 0,
    Cidr = 1
}
