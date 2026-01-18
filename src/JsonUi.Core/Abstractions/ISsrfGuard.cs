using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface ISsrfGuard
{
    Task EnsureAllowedAsync(Uri target, IEnumerable<IntegrationAllowlist> allowlists, CancellationToken cancellationToken = default);
}
