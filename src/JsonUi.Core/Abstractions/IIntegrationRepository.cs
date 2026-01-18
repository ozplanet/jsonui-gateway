using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface IIntegrationRepository
{
    Task<Integration?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Integration>> ListAsync(CancellationToken cancellationToken = default);
    Task AddAsync(Integration integration, CancellationToken cancellationToken = default);
    Task UpdateAsync(Integration integration, CancellationToken cancellationToken = default);
}
