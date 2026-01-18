namespace JsonUi.Core.Abstractions;

public interface ISecretProtector
{
    SecretPayload Encrypt(string plaintext);
    string Decrypt(SecretPayload payload);
}

public sealed record SecretPayload(byte[] Ciphertext, byte[] Nonce, byte[] Tag);
