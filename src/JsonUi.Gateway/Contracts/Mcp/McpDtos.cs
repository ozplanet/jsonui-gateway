namespace JsonUi.Gateway.Contracts.Mcp;

public sealed record McpToolResponse(
    Guid ActionId,
    Guid IntegrationId,
    string Name,
    string Slug,
    string Method,
    string PathTemplate,
    string? JsonSchema);

public sealed record McpToolResult(int StatusCode, IDictionary<string, string> Headers, string Body);
