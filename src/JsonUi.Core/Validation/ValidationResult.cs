namespace JsonUi.Core.Validation;

public sealed class ValidationResult
{
    private ValidationResult(bool isValid, string? error, Uri? parsedUri)
    {
        IsValid = isValid;
        Error = error;
        ParsedUri = parsedUri;
    }

    public bool IsValid { get; }
    public string? Error { get; }
    public Uri? ParsedUri { get; }

    public static ValidationResult Valid(Uri? parsedUri = null) => new(true, null, parsedUri);
    public static ValidationResult Invalid(string error) => new(false, error, null);
}
