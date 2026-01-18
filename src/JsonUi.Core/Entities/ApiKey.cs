namespace JsonUi.Core.Entities;

public sealed class ApiKey
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Prefix { get; private set; } = string.Empty;
    public byte[] Hash { get; private set; } = Array.Empty<byte>();
    public byte[] Salt { get; private set; } = Array.Empty<byte>();
    public bool IsAdmin { get; private set; }
    public IReadOnlyCollection<ApiKeyScope> Scopes => _scopes.AsReadOnly();
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? LastUsedAt { get; private set; }

    private readonly List<ApiKeyScope> _scopes = new();

    private ApiKey()
    {
    }

    public ApiKey(string prefix, byte[] hash, byte[] salt, bool isAdmin)
    {
        Prefix = prefix;
        Hash = hash;
        Salt = salt;
        IsAdmin = isAdmin;
    }

    public void SetScopes(IEnumerable<ApiKeyScope> scopes)
    {
        _scopes.Clear();
        _scopes.AddRange(scopes);
    }

    public void Touch() => LastUsedAt = DateTimeOffset.UtcNow;
}

public sealed class ApiKeyScope
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid ApiKeyId { get; init; }
    public Guid? IntegrationId { get; init; }
    public Guid? ActionId { get; init; }
}
