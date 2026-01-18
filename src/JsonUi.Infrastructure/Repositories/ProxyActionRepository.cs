using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Infrastructure.Repositories;

public sealed class ProxyActionRepository(JsonUiDbContext dbContext) : IProxyActionRepository
{
    public async Task<ProxyAction?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.ProxyActions.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<ProxyAction>> ListByIntegrationAsync(Guid integrationId, CancellationToken cancellationToken = default)
        => await dbContext.ProxyActions.Where(x => x.IntegrationId == integrationId).ToListAsync(cancellationToken);

    public async Task AddAsync(ProxyAction action, CancellationToken cancellationToken = default)
    {
        await dbContext.ProxyActions.AddAsync(action, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(ProxyAction action, CancellationToken cancellationToken = default)
    {
        dbContext.ProxyActions.Update(action);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
