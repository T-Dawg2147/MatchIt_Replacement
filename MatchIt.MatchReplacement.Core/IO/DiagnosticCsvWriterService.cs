using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MatchIt.MatchReplacement.Core.IO;

public sealed class DiagnosticCsvWriterService
{
    public async Task WriteAsync<T>(string path, IEnumerable<T> records)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CreateConfiguration());

        await csv.WriteRecordsAsync(records);
        await writer.FlushAsync();
    }

    private static CsvConfiguration CreateConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = true,
            ShouldQuote = _ => true
        };
    }
}