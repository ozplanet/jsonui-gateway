namespace JsonUi.Gateway.Configuration;

public sealed class RateLimitOptions
{
    public int ProxyPermitLimit { get; set; } = 60;
    public int ProxyWindowSeconds { get; set; } = 60;
    public int AdminPermitLimit { get; set; } = 30;
    public int AdminWindowSeconds { get; set; } = 60;
    public int McpPermitLimit { get; set; } = 120;
    public int McpWindowSeconds { get; set; } = 60;
}
