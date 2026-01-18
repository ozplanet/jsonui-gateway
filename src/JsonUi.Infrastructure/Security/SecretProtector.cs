using System.Security.Cryptography;
using JsonUi.Core.Abstractions;
using Microsoft.Extensions.Options;

namespace JsonUi.Infrastructure.Security;

public sealed class SecretProtector : ISecretProtector
{
    private readonly byte[] _masterKey;

    public SecretProtector(IOptions<SecretOptions> options)
    {
        var key = options.Value.MasterKey;
        if (string.IsNullOrWhiteSpace(key))
        {
            throw new InvalidOperationException("JSONUI_MASTER_KEY is required");
        }

        _masterKey = Convert.FromBase64String(key);
        if (_masterKey.Length < 32)
        {
            throw new InvalidOperationException("Master key must be at least 32 bytes");
        }
    }

    public SecretPayload Encrypt(string plaintext)
    {
        var plaintextBytes = System.Text.Encoding.UTF8.GetBytes(plaintext);
        var nonce = RandomNumberGenerator.GetBytes(12);
        var ciphertext = new byte[plaintextBytes.Length];
        var tag = new byte[16];

        using var aes = new AesGcm(_masterKey);
        aes.Encrypt(nonce, plaintextBytes, ciphertext, tag);

        return new SecretPayload(ciphertext, nonce, tag);
    }

    public string Decrypt(SecretPayload payload)
    {
        var plaintext = new byte[payload.Ciphertext.Length];
        using var aes = new AesGcm(_masterKey);
        aes.Decrypt(payload.Nonce, payload.Ciphertext, payload.Tag, plaintext);
        return System.Text.Encoding.UTF8.GetString(plaintext);
    }
}

public sealed class SecretOptions
{
    public string MasterKey { get; set; } = string.Empty;
}
