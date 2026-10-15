using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public interface IExportProvider
{
    ExportFormat Format { get; }

    bool CanHandle(ExportFormat format);

    Task WriteAsync<T>(
        string path,
        IEnumerable<T> records,
        ExportOptions options,
        CancellationToken cancellationToken = default);
}