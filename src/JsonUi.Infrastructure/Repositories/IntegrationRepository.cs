using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Infrastructure.Repositories;

public sealed class IntegrationRepository(JsonUiDbContext dbContext) : IIntegrationRepository
{
    public async Task<Integration?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Integrations.Include(x => x.Allowlists).SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Integration>> ListAsync(CancellationToken cancellationToken = default)
        => await dbContext.Integrations.Include(x => x.Allowlists).ToListAsync(cancellationToken);

    public async Task AddAsync(Integration integration, CancellationToken cancellationToken = default)
    {
        await dbContext.Integrations.AddAsync(integration, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Integration integration, CancellationToken cancellationToken = default)
    {
        dbContext.Integrations.Update(integration);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
