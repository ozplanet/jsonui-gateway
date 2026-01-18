using System.Net;
using System.Numerics;

namespace JsonUi.Infrastructure.Ssrf;

public readonly struct IPNetwork
{
    private readonly BigInteger _network;
    private readonly BigInteger _mask;

    private IPNetwork(BigInteger network, BigInteger mask)
    {
        _network = network;
        _mask = mask;
    }

    public static IPNetwork Parse(string cidr)
    {
        if (!TryParse(cidr, out var network))
        {
            throw new FormatException($"Invalid CIDR {cidr}");
        }

        return network;
    }

    public static bool TryParse(string cidr, out IPNetwork network)
    {
        network = default;
        var parts = cidr.Split('/');
        if (parts.Length != 2)
        {
            return false;
        }

        if (!IPAddress.TryParse(parts[0], out var ip))
        {
            return false;
        }

        if (!int.TryParse(parts[1], out var prefixLength))
        {
            return false;
        }

        var bytes = ip.GetAddressBytes();
        var maxBits = bytes.Length * 8;
        if (prefixLength < 0 || prefixLength > maxBits)
        {
            return false;
        }

        var maskBytes = new byte[bytes.Length];
        for (var i = 0; i < bytes.Length; i++)
        {
            var bits = Math.Clamp(prefixLength - (i * 8), 0, 8);
            maskBytes[i] = (byte)(0xFF << (8 - bits));
        }

        network = new IPNetwork(new BigInteger(bytes, isUnsigned: true, isBigEndian: true) & new BigInteger(maskBytes, isUnsigned: true, isBigEndian: true),
            new BigInteger(maskBytes, isUnsigned: true, isBigEndian: true));
        return true;
    }

    public bool Contains(IPAddress address)
    {
        var addressValue = new BigInteger(address.GetAddressBytes(), isUnsigned: true, isBigEndian: true);
        return (addressValue & _mask) == _network;
    }
}
