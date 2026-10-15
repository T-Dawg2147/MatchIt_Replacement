using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.IO;

public sealed class CsvWriterService
{
    public async Task WriteContactsAsync(string path, IEnumerable<ContactRecord> records)
    {
        var exportRows = records
            .Select(ContactExportProjection.From)
            .ToList();

        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CreateConfiguration());

        await csv.WriteRecordsAsync(exportRows);
        await writer.FlushAsync();
    }

    public async Task WriteSuppressionsAsync(string path, IEnumerable<SuppressionRecord> records)
    {
        await using var writer = new StreamWriter(path);
        await using var csv = new CsvWriter(writer, CreateConfiguration());

        await csv.WriteRecordsAsync(records);
        await writer.FlushAsync();
    }

    public async Task WriteSiteLevelAsync(string path, IEnumerable<ActiveSiteRecord> records)
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
            ShouldQuote = (args) => true
        };
    }
}