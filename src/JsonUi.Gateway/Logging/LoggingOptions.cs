namespace JsonUi.Gateway.Logging;

public sealed class LoggingOptions
{
    public string MinimumLevel { get; set; } = "Information";
    public FileOptions File { get; set; } = new();
    public ApplicationInsightsOptions ApplicationInsights { get; set; } = new();
}

public sealed class FileOptions
{
    public bool Enabled { get; set; } = true;
    public string Path { get; set; } = "/app/data/logs/jsonui.log";
    public long FileSizeLimitBytes { get; set; } = 5 * 1024 * 1024;
    public bool RollOnFileSizeLimit { get; set; } = true;
}

public sealed class ApplicationInsightsOptions
{
    public bool Enabled { get; set; }
    public string ConnectionString { get; set; } = string.Empty;
}
