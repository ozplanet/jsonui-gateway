using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface ISecretRepository
{
    Task<Secret?> GetAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Secret secret, CancellationToken cancellationToken = default);
    Task UpdateAsync(Secret secret, CancellationToken cancellationToken = default);
}
