using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.IO;

public sealed class CsvReaderService
{
    public async Task<List<ContactRecord>> ReadContactsAsync(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CreatePipeNoHeaderConfiguration());

        var records = new List<ContactRecord>();

        while (await csv.ReadAsync())
        {
            var record = new ContactRecord
            {
                Cid = GetFieldSafe(csv, 0),
                AccNo = GetFieldSafe(csv, 1),
                Office = GetFieldSafe(csv, 2),
                Prefix = GetFieldSafe(csv, 3),
                Forenames = GetFieldSafe(csv, 4),
                Surname = GetFieldSafe(csv, 5),
                JobTitle = GetFieldSafe(csv, 6),
                Dept = GetFieldSafe(csv, 7),
                Company = GetFieldSafe(csv, 8),
                Address1 = GetFieldSafe(csv, 9),
                Address2 = GetFieldSafe(csv, 10),
                Address3 = GetFieldSafe(csv, 11),
                Address4 = GetFieldSafe(csv, 12),
                Address5 = GetFieldSafe(csv, 13),
                Postcode = GetFieldSafe(csv, 14),
                Email = GetFieldSafe(csv, 15),
                CCreated = GetFieldSafe(csv, 16),
                LastTypeCr = GetFieldSafe(csv, 17),
                LastDateCr = GetFieldSafe(csv, 18),
                CRecency = GetFieldSafe(csv, 19),
                Bdm = GetFieldSafe(csv, 20),
                Hierarchy = GetFieldSafe(csv, 21),
                RollupLoy = GetFieldSafe(csv, 22),
                RollupAcc = GetFieldSafe(csv, 23),
                ActType = GetFieldSafe(csv, 24),
                AccSupp = GetFieldSafe(csv, 25),
                Rem = GetFieldSafe(csv, 26)
            };

            records.Add(record);
        }

        return records;
    }

    public async Task<List<SuppressionRecord>> ReadSuppressionsAsync(string path)
    {
        using var reader = new StreamReader(path);
        using var csv = new CsvReader(reader, CreatePipeNoHeaderConfiguration());

        var records = new List<SuppressionRecord>();

        while (await csv.ReadAsync())
        {
            var record = new SuppressionRecord
            {
                Other1 = GetFieldSafe(csv, 0),
                Company = GetFieldSafe(csv, 1),
                Address1 = GetFieldSafe(csv, 2),
                Address2 = GetFieldSafe(csv, 3),
                Address3 = GetFieldSafe(csv, 4),
                Address4 = GetFieldSafe(csv, 5),
                Address5 = GetFieldSafe(csv, 6),
                Postcode = GetFieldSafe(csv, 7),
                Other2 = GetFieldSafe(csv, 8),
                Other3 = GetFieldSafe(csv, 9)
            };

            records.Add(record);
        }

        return records;
    }

    private static string? GetFieldSafe(CsvReader csv, int index)
    {
        try
        {
            return csv.GetField(index);
        }
        catch
        {
            return null;
        }
    }

    private static CsvConfiguration CreatePipeNoHeaderConfiguration()
    {
        return new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false,
            Delimiter = "|",
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null
        };
    }
}