using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public sealed class CsvExportProvider : IExportProvider
{
    public ExportFormat Format => ExportFormat.Csv;

    public bool CanHandle(ExportFormat format) => format == ExportFormat.Csv;

    public async Task WriteAsync<T>(
        string path,
        IEnumerable<T> records,
        ExportOptions options,
        CancellationToken cancellationToken = default)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CreateConfiguration(options));

        await csv.WriteRecordsAsync(records, cancellationToken);
    }

    private static CsvConfiguration CreateConfiguration(ExportOptions options)
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = options.IncludeHeaders,
            ShouldQuote = _ => options.QuoteAllFields
        };
    }
}