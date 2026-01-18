using JsonUi.Core.Entities;

namespace JsonUi.Core.Abstractions;

public interface IApiKeyRepository
{
    Task<ApiKey?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(CancellationToken cancellationToken = default);
    Task AddAsync(ApiKey key, CancellationToken cancellationToken = default);
    Task UpdateAsync(ApiKey key, CancellationToken cancellationToken = default);
}
