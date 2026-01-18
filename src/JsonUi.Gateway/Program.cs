using AspNetCoreRateLimit;
using JsonUi.Gateway.Authentication;
using JsonUi.Gateway.Authorization;
using JsonUi.Gateway.Extensions;
using JsonUi.Gateway.Logging;
using JsonUi.Gateway.Services;
using JsonUi.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

var builder = WebApplication.CreateBuilder(args);

LoggingConfigurator.ConfigureSerilog(builder);

builder.Services.Configure<LoggingOptions>(builder.Configuration.GetSection("JsonUi:Logging"));
builder.Services.AddSingleton<ILoggingService, LoggingService>();
builder.Services.AddScoped<ISwaggerImportService, SwaggerImportService>();

builder.Services.AddJsonUiCore(builder.Configuration);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthenticationHandler.SchemeName;
    options.DefaultChallengeScheme = ApiKeyAuthenticationHandler.SchemeName;
}).AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
});

builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

builder.Services.AddScoped<IAuthorizationHandler, ScopeAuthorizationHandler>();
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<JsonUiDbContext>();
    db.Database.EnsureCreated();

    var bootstrap = scope.ServiceProvider.GetRequiredService<JsonUi.Core.Abstractions.IAdminBootstrapService>();
    var result = await bootstrap.EnsureAdminKeyAsync();
    if (result.Created && result.RawKey is not null)
    {
        app.Logger.LogWarning("Initial admin key created. Store securely: {Key}", result.RawKey);
    }
}

app.UseIpRateLimiting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/health/live", () => Results.Ok(new { status = "live" }));

app.Run();
