using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface IProxyActionRepository
{
    Task<ProxyAction?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ProxyAction>> ListByIntegrationAsync(Guid integrationId, CancellationToken cancellationToken = default);
    Task AddAsync(ProxyAction action, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProxyAction action, CancellationToken cancellationToken = default);
}
