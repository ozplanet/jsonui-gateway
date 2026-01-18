using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Infrastructure.Repositories;

public sealed class ApiKeyRepository(JsonUiDbContext dbContext) : IApiKeyRepository
{
    public async Task<ApiKey?> FindByPrefixAsync(string prefix, CancellationToken cancellationToken = default)
        => await dbContext.ApiKeys.Include(x => x.Scopes).SingleOrDefaultAsync(x => x.Prefix == prefix, cancellationToken);

    public async Task<bool> AnyAsync(CancellationToken cancellationToken = default)
        => await dbContext.ApiKeys.AnyAsync(cancellationToken);

    public async Task AddAsync(ApiKey key, CancellationToken cancellationToken = default)
    {
        await dbContext.ApiKeys.AddAsync(key, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ApiKey key, CancellationToken cancellationToken = default)
    {
        dbContext.ApiKeys.Update(key);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
