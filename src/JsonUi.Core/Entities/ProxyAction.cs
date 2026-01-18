namespace JsonUi.Core.Entities;

public sealed class ProxyAction
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public Guid IntegrationId { get; init; }
    public string Name { get; private set; } = string.Empty;
    public string Slug { get; private set; } = string.Empty;
    public string Method { get; private set; } = "GET";
    public string PathTemplate { get; private set; } = "/";
    public string? JsonSchema { get; private set; }
    public bool Enabled { get; private set; } = true;
    public bool CacheEnabled { get; private set; }
    public TimeSpan? CacheTtl { get; private set; }

    private ProxyAction()
    {
    }

    public ProxyAction(Guid integrationId, string name, string method, string pathTemplate)
    {
        IntegrationId = integrationId;
        Name = name.Trim();
        Slug = GenerateSlug(name);
        Method = method.ToUpperInvariant();
        PathTemplate = pathTemplate.Trim();
    }

    public void Update(string name, string method, string pathTemplate, string? jsonSchema, bool enabled, bool cacheEnabled, TimeSpan? cacheTtl)
    {
        Name = name.Trim();
        Slug = GenerateSlug(name);
        Method = method.ToUpperInvariant();
        PathTemplate = pathTemplate.Trim();
        JsonSchema = jsonSchema;
        Enabled = enabled;
        CacheEnabled = cacheEnabled;
        CacheTtl = cacheTtl;
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
