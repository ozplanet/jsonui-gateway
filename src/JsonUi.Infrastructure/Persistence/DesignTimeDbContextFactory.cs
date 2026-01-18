using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JsonUi.Infrastructure.Persistence;

public sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<JsonUiDbContext>
{
    public JsonUiDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<JsonUiDbContext>();
        var connectionString = "Data Source=design-time-jsonui.db";
        optionsBuilder.UseSqlite(connectionString);
        return new JsonUiDbContext(optionsBuilder.Options);
    }
}
