using System.Text.Json;
using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public sealed class JsonExportProvider : IExportProvider
{
    public ExportFormat Format => ExportFormat.Json;

    public bool CanHandle(ExportFormat format) => format == ExportFormat.Json;

    public async Task WriteAsync<T>(
        string path,
        IEnumerable<T> records,
        ExportOptions options,
        CancellationToken cancellationToken = default)
    {
        await using var stream = File.Create(path);

        await JsonSerializer.SerializeAsync(
            stream,
            records,
            new JsonSerializerOptions
            {
                WriteIndented = true
            },
            cancellationToken);
    }
}