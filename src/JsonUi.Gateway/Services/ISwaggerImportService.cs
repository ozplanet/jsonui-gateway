using JsonUi.Gateway.Contracts.Admin;

namespace JsonUi.Gateway.Services;

public interface ISwaggerImportService
{
    Task<IReadOnlyCollection<ProxyActionResponse>> ImportAsync(Guid integrationId, Stream swaggerStream, CancellationToken cancellationToken = default);
}
