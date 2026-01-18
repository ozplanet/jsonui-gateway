using System.Security.Cryptography;
using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using Microsoft.Extensions.Logging;

namespace JsonUi.Infrastructure.Security;

public sealed class AdminBootstrapService : IAdminBootstrapService
{
    private readonly IApiKeyRepository _repository;
    private readonly IApiKeyHasher _hasher;
    private readonly ILogger<AdminBootstrapService> _logger;

    public AdminBootstrapService(IApiKeyRepository repository, IApiKeyHasher hasher, ILogger<AdminBootstrapService> logger)
    {
        _repository = repository;
        _hasher = hasher;
        _logger = logger;
    }

    public async Task<BootstrapResult> EnsureAdminKeyAsync(CancellationToken cancellationToken = default)
    {
        if (await _repository.AnyAsync(cancellationToken))
        {
            _logger.LogInformation("Admin key already exists");
            return new BootstrapResult(false, null, null);
        }

        var rawKey = GenerateApiKey();
        var prefix = rawKey[..8];
        var hash = _hasher.Hash(rawKey);
        var apiKey = new ApiKey(prefix, hash.Hash, hash.Salt, isAdmin: true);

        await _repository.AddAsync(apiKey, cancellationToken);

        _logger.LogWarning("Generated initial admin API key; store securely. Prefix {Prefix}", prefix);
        return new BootstrapResult(true, rawKey, prefix);
    }

    private static string GenerateApiKey()
    {
        var bytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToBase64String(bytes);
    }
}
