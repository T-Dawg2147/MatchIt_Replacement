using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public sealed class DefaultExportProviderResolver : IExportProviderResolver
{
    private readonly IReadOnlyList<IExportProvider> _providers;

    public DefaultExportProviderResolver()
    {
        _providers = new IExportProvider[]
        {
            new CsvExportProvider(),
            new PipeDelimitedExportProvider(),
            new JsonExportProvider()
        };
    }

    public IExportProvider Resolve(ExportFormat format)
    {
        var provider = _providers.FirstOrDefault(p => p.CanHandle(format));
        if (provider is null)
        {
            throw new InvalidOperationException(
                $"No export provider registered for format '{format}'.");
        }

        return provider;
    }

    public IReadOnlyList<IExportProvider> GetAll() => _providers;
}