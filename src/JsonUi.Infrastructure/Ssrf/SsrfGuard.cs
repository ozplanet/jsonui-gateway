using System.Net;
using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;

namespace JsonUi.Infrastructure.Ssrf;

public sealed class SsrfGuard : ISsrfGuard
{
    private static readonly IPNetwork[] DefaultDeniedNetworks =
    [
        IPNetwork.Parse("127.0.0.0/8"),
        IPNetwork.Parse("10.0.0.0/8"),
        IPNetwork.Parse("172.16.0.0/12"),
        IPNetwork.Parse("192.168.0.0/16"),
        IPNetwork.Parse("169.254.0.0/16"),
        IPNetwork.Parse("100.64.0.0/10"),
        IPNetwork.Parse("::1/128"),
        IPNetwork.Parse("fe80::/10")
    ];

    public async Task EnsureAllowedAsync(Uri target, IEnumerable<IntegrationAllowlist> allowlists, CancellationToken cancellationToken = default)
    {
        if (target.Scheme is not ("http" or "https"))
        {
            throw new InvalidOperationException("Only HTTP/HTTPS targets are allowed");
        }

        var addresses = await Dns.GetHostAddressesAsync(target.Host, cancellationToken);
        foreach (var address in addresses)
        {
            if (DefaultDeniedNetworks.Any(network => network.Contains(address)))
            {
                if (!IsExplicitlyAllowed(address, allowlists))
                {
                    throw new InvalidOperationException("Target address blocked by SSRF guard");
                }
            }
        }
    }

    private static bool IsExplicitlyAllowed(IPAddress address, IEnumerable<IntegrationAllowlist> allowlists)
    {
        foreach (var entry in allowlists)
        {
            if (entry.Kind == AllowlistKind.Host)
            {
                if (Uri.CheckHostName(entry.Value) != UriHostNameType.Unknown)
                {
                    try
                    {
                        var hostAddresses = Dns.GetHostAddresses(entry.Value);
                        if (hostAddresses.Any(a => Equals(a, address)))
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        // ignore resolution errors
                    }
                }
            }
            else if (entry.Kind == AllowlistKind.Cidr)
            {
                if (IPNetwork.TryParse(entry.Value, out var network) && network.Contains(address))
                {
                    return true;
                }
            }
        }

        return false;
    }
}
