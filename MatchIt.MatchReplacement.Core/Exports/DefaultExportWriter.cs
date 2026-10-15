using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public sealed class DefaultExportWriter
{
    private readonly IExportProviderResolver _providerResolver;

    public DefaultExportWriter(IExportProviderResolver? providerResolver = null)
    {
        _providerResolver = providerResolver ?? new DefaultExportProviderResolver();
    }

    public async Task WriteAsync<T>(
        string path,
        IEnumerable<T> records,
        ExportOptions options,
        CancellationToken cancellationToken = default)
    {
        var provider = _providerResolver.Resolve(options.Format);
        await provider.WriteAsync(path, records, options, cancellationToken);
    }
}