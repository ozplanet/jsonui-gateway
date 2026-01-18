using System.Net;
using System.Text.RegularExpressions;
using JsonUi.Core.Entities;

namespace JsonUi.Core.Validation;

public static class IntegrationValidator
{
    private static readonly Regex SlugRegex = new("^[a-z0-9-]+$", RegexOptions.Compiled);

    public static ValidationResult ValidateForCreate(string name, string baseUrl, string authMode)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return ValidationResult.Invalid("Name is required");
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri))
        {
            return ValidationResult.Invalid("BaseUrl must be absolute");
        }

        if (!SlugRegex.IsMatch(GenerateSlug(name)))
        {
            return ValidationResult.Invalid("Name must produce a valid slug (letters, numbers, hyphen)");
        }

        if (!IsSupportedAuthMode(authMode))
        {
            return ValidationResult.Invalid("Unsupported auth mode");
        }

        if (uri.Scheme is not ("http" or "https"))
        {
            return ValidationResult.Invalid("BaseUrl must be HTTP or HTTPS");
        }

        return ValidationResult.Valid(uri);
    }

    private static bool IsSupportedAuthMode(string authMode) =>
        authMode switch
        {
            "none" => true,
            "header" => true,
            "bearer" => true,
            _ => false
        };

    private static string GenerateSlug(string input)
    {
        var slug = new string(input.ToLowerInvariant()
            .Where(c => char.IsLetterOrDigit(c) || c == ' ' || c == '-')
            .Select(c => c == ' ' ? '-' : c)
            .ToArray());
        return slug.Trim('-');
    }
}
