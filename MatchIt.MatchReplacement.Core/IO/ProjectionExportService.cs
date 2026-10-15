using MatchIt.MatchReplacement.Core.Application;
using MatchIt.MatchReplacement.Core.Exports;

namespace MatchIt.MatchReplacement.Core.IO;

public sealed class ProjectionExportService
{
    private readonly DefaultExportWriter _exportWriter;

    public ProjectionExportService(DefaultExportWriter? exportWriter = null)
    {
        _exportWriter = exportWriter ?? new DefaultExportWriter();
    }

    public async Task WriteAsync<T>(
        string path,
        IEnumerable<T> rows,
        ExportOptions options,
        CancellationToken cancellationToken = default)
    {
        await _exportWriter.WriteAsync(path, rows, options, cancellationToken);
    }
}