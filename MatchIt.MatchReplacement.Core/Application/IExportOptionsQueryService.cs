namespace MatchIt.MatchReplacement.Core.Application;

public interface IExportOptionsQueryService
{
    Task<GetSupportedExportFormatsResponse> GetSupportedFormatsAsync(
        CancellationToken cancellationToken = default);
}