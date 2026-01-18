using JsonUi.Core.Abstractions;
using JsonUi.Core.Entities;
using JsonUi.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Infrastructure.Repositories;

public sealed class SecretRepository(JsonUiDbContext dbContext) : ISecretRepository
{
    public async Task<Secret?> GetAsync(Guid id, CancellationToken cancellationToken = default)
        => await dbContext.Secrets.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);

    public async Task AddAsync(Secret secret, CancellationToken cancellationToken = default)
    {
        await dbContext.Secrets.AddAsync(secret, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Secret secret, CancellationToken cancellationToken = default)
    {
        dbContext.Secrets.Update(secret);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
