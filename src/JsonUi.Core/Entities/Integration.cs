namespace JsonUi.Core.Entities;

public sealed class Integration
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public Uri BaseUrl { get; private set; } = new("http://localhost");
    public bool Enabled { get; private set; } = true;
    public string AuthMode { get; private set; } = "none";
    public Guid? SecretId { get; private set; }
    public IReadOnlyCollection<IntegrationAllowlist> Allowlists => _allowlists.AsReadOnly();

    private readonly List<IntegrationAllowlist> _allowlists = new();

    private Integration()
    {
    }

    public Integration(string name, Uri baseUrl, string authMode)
    {
        Name = name.Trim();
        Slug = GenerateSlug(name);
        BaseUrl = baseUrl;
        AuthMode = authMode;
    }

    public void Update(string name, Uri baseUrl, string authMode, bool enabled)
    {
        Name = name.Trim();
        Slug = GenerateSlug(name);
        BaseUrl = baseUrl;
        AuthMode = authMode;
        Enabled = enabled;
    }

    public void SetSecret(Guid secretId) => SecretId = secretId;

    public void ClearSecret() => SecretId = null;

    public void ReplaceAllowlists(IEnumerable<IntegrationAllowlist> entries)
    {
        _allowlists.Clear();
        _allowlists.AddRange(entries);
    }

    private static string GenerateSlug(string input)
    {
        var slug = new string(input.ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-')
            .Select(c => c == ' ' ? '-' : c)
            .ToArray());
        return slug.Trim('-');
    }
}
