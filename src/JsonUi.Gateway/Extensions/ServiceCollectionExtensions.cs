using JsonUi.Core.Abstractions;
using JsonUi.Infrastructure.Execution;
using JsonUi.Infrastructure.Persistence;
using JsonUi.Infrastructure.Repositories;
using JsonUi.Infrastructure.Security;
using JsonUi.Infrastructure.Ssrf;
using Microsoft.EntityFrameworkCore;

namespace JsonUi.Gateway.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddJsonUiCore(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SecretOptions>(options => options.MasterKey = configuration["JSONUI_MASTER_KEY"] ?? string.Empty);

        services.AddDbContext<JsonUiDbContext>(options =>
        {
            var dbPath = configuration["JSONUI_DB_PATH"] ?? "/app/data/jsonui.db";
            options.UseSqlite($"Data Source={dbPath}");
        });

        services.AddSingleton<ISecretProtector, SecretProtector>();
        services.AddSingleton<IApiKeyHasher, ApiKeyHasher>();
        services.AddScoped<ISsrfGuard, SsrfGuard>();
        services.AddHttpClient("proxy");
        services.AddScoped<IToolExecutor, HttpToolExecutor>();
        services.AddScoped<IIntegrationRepository, IntegrationRepository>();
        services.AddScoped<IProxyActionRepository, ProxyActionRepository>();
        services.AddScoped<ISecretRepository, SecretRepository>();
        services.AddScoped<IApiKeyRepository, ApiKeyRepository>();
        services.AddScoped<IAdminBootstrapService, AdminBootstrapService>();
        return services;
    }
}
