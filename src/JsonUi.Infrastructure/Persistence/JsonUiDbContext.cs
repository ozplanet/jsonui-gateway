using JsonUi.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Infrastructure.Persistence;

public sealed class JsonUiDbContext(DbContextOptions<JsonUiDbContext> options) : DbContext(options)
{
    public DbSet<Integration> Integrations => Set<Integration>();
    public DbSet<IntegrationAllowlist> IntegrationAllowlists => Set<IntegrationAllowlist>();
    public DbSet<ProxyAction> ProxyActions => Set<ProxyAction>();
    public DbSet<Secret> Secrets => Set<Secret>();
    public DbSet<ApiKey> ApiKeys => Set<ApiKey>();
    public DbSet<ApiKeyScope> ApiKeyScopes => Set<ApiKeyScope>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Integration>(entity =>
        {
            entity.ToTable("integrations");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Slug).IsRequired();
            entity.Property(x => x.BaseUrl)
                .HasConversion(v => v.ToString(), v => new Uri(v));
            entity.HasMany(x => x.Allowlists)
                .WithOne()
                .HasForeignKey(x => x.IntegrationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<IntegrationAllowlist>(entity =>
        {
            entity.ToTable("integration_allowlists");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Value).IsRequired();
            entity.Property(x => x.Kind).HasConversion<int>();
        });

        modelBuilder.Entity<ProxyAction>(entity =>
        {
            entity.ToTable("proxy_actions");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Slug).IsRequired();
            entity.Property(x => x.Method).IsRequired();
            entity.Property(x => x.PathTemplate).IsRequired();
        });

        modelBuilder.Entity<Secret>(entity =>
        {
            entity.ToTable("secrets");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).IsRequired();
            entity.Property(x => x.Ciphertext).IsRequired();
            entity.Property(x => x.Nonce).IsRequired();
            entity.Property(x => x.Tag).IsRequired();
        });

        modelBuilder.Entity<ApiKey>(entity =>
        {
            entity.ToTable("api_keys");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Prefix).IsRequired();
            entity.Property(x => x.Hash).IsRequired();
            entity.Property(x => x.Salt).IsRequired();
            entity.HasMany(x => x.Scopes)
                .WithOne()
                .HasForeignKey(x => x.ApiKeyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApiKeyScope>(entity =>
        {
            entity.ToTable("api_key_scopes");
            entity.HasKey(x => x.Id);
        });
    }
}
