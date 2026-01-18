using System.Security.Claims;
using System.Text.Encodings.Web;
using JsonUi.Core.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace JsonUi.Gateway.Authentication;

public sealed class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "ApiKey";

    private readonly IApiKeyRepository _repository;
    private readonly IApiKeyHasher _hasher;

    public ApiKeyAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        ISystemClock clock,
        IApiKeyRepository repository,
        IApiKeyHasher hasher) : base(options, logger, encoder, clock)
    {
        _repository = repository;
        _hasher = hasher;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("X-Api-Key", out var header) || string.IsNullOrWhiteSpace(header))
        {
            return AuthenticateResult.Fail("Missing API key");
        }

        var rawKey = header.ToString().Trim();
        if (rawKey.Length < 12)
        {
            return AuthenticateResult.Fail("API key format invalid");
        }

        var prefix = rawKey[..8];
        var apiKey = await _repository.FindByPrefixAsync(prefix, Context.RequestAborted);
        if (apiKey is null)
        {
            return AuthenticateResult.Fail("Unknown API key");
        }

        var storedHash = new ApiKeyHash(apiKey.Hash, apiKey.Salt);
        if (!_hasher.Verify(rawKey, storedHash))
        {
            return AuthenticateResult.Fail("Invalid API key");
        }

        apiKey.Touch();
        await _repository.UpdateAsync(apiKey, Context.RequestAborted);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, apiKey.Id.ToString()),
            new("prefix", apiKey.Prefix)
        };

        if (apiKey.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }

        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}
