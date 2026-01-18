using System.Security.Cryptography;
using JsonUi.Core.Abstractions;

namespace JsonUi.Infrastructure.Security;

public sealed class ApiKeyHasher : IApiKeyHasher
{
    public ApiKeyHash Hash(string rawKey)
    {
        var salt = RandomNumberGenerator.GetBytes(32);
        var hash = Rfc2898DeriveBytes.Pbkdf2(rawKey, salt, 100_000, HashAlgorithmName.SHA256, 32);
        return new ApiKeyHash(hash, salt);
    }

    public bool Verify(string rawKey, ApiKeyHash stored)
    {
        var computed = Rfc2898DeriveBytes.Pbkdf2(rawKey, stored.Salt, 100_000, HashAlgorithmName.SHA256, 32);
        return CryptographicOperations.FixedTimeEquals(computed, stored.Hash);
    }
}
