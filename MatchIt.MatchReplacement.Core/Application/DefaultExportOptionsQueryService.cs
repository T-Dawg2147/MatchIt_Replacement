using MatchIt.MatchReplacement.Core.Exports;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DefaultExportOptionsQueryService : IExportOptionsQueryService
{
    private readonly IExportProviderResolver _resolver;

    public DefaultExportOptionsQueryService(IExportProviderResolver? resolver = null)
    {
        _resolver = resolver ?? new DefaultExportProviderResolver();
    }

    public Task<GetSupportedExportFormatsResponse> GetSupportedFormatsAsync(
        CancellationToken cancellationToken = default)
    {
        var formats = _resolver
            .GetAll()
            .Select(p => p.Format)
            .Distinct()
            .OrderBy(f => (int)f)
            .ToList();

        return Task.FromResult(new GetSupportedExportFormatsResponse
        {
            Formats = formats
        });
    }
}