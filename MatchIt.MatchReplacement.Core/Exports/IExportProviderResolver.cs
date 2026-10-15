using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public interface IExportProviderResolver
{
    IExportProvider Resolve(ExportFormat format);
    IReadOnlyList<IExportProvider> GetAll();
}