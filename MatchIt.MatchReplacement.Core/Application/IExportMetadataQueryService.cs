namespace MatchIt.MatchReplacement.Core.Application;

public interface IExportMetadataQueryService
{
    Task<GetAvailableExportsResponse> GetAvailableExportsAsync(
        CancellationToken cancellationToken = default);
}