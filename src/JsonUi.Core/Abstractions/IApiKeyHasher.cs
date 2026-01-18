namespace JsonUi.Core.Abstractions;

public interface IApiKeyHasher
{
    ApiKeyHash Hash(string rawKey);
    bool Verify(string rawKey, ApiKeyHash stored);
}

public sealed record ApiKeyHash(byte[] Hash, byte[] Salt);
