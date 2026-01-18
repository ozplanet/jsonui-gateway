namespace JsonUi.Core.Entities;

public sealed class Secret
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; private set; } = string.Empty;
    public byte[] Ciphertext { get; private set; } = Array.Empty<byte>();
    public byte[] Nonce { get; private set; } = Array.Empty<byte>();
    public byte[] Tag { get; private set; } = Array.Empty<byte>();
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;

    private Secret()
    {
    }

    public Secret(string name)
    {
        Name = name.Trim();
    }

    public void UpdateValue(byte[] ciphertext, byte[] nonce, byte[] tag)
    {
        Ciphertext = ciphertext;
        Nonce = nonce;
        Tag = tag;
    }
}
