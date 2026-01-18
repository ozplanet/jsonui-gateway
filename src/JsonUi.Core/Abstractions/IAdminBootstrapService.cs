namespace JsonUi.Core.Abstractions;

public interface IAdminBootstrapService
{
    Task<BootstrapResult> EnsureAdminKeyAsync(CancellationToken cancellationToken = default);
}

public sealed record BootstrapResult(bool Created, string? RawKey, string? Prefix);
