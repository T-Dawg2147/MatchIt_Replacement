using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ResultPreviewService : IResultPreviewService
{
    public async Task<PreviewFileResponse> PreviewFileAsync(
        PreviewFileRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (string.IsNullOrWhiteSpace(request.FilePath))
        {
            throw new ArgumentException("File path is required.", nameof(request));
        }

        if (!File.Exists(request.FilePath))
        {
            throw new FileNotFoundException("Preview file not found.", request.FilePath);
        }

        if (request.Format == ExportFormat.Json)
        {
            return await PreviewJsonAsync(request, cancellationToken);
        }

        return await PreviewDelimitedAsync(request, cancellationToken);
    }

    private static async Task<PreviewFileResponse> PreviewDelimitedAsync(
        PreviewFileRequest request,
        CancellationToken cancellationToken)
    {
        var delimiter = request.Format == ExportFormat.PipeDelimited ? "|" : ",";

        using var reader = new StreamReader(request.FilePath);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = request.HasHeaderRow,
            Delimiter = delimiter,
            IgnoreBlankLines = true,
            TrimOptions = TrimOptions.Trim,
            BadDataFound = null,
            MissingFieldFound = null,
            HeaderValidated = null
        });

        var rows = new List<PreviewRow>();
        var columns = new List<string>();

        if (request.HasHeaderRow)
        {
            await csv.ReadAsync();
            csv.ReadHeader();
            columns.AddRange(csv.HeaderRecord ?? Array.Empty<string>());
        }

        var rowCount = 0;

        while (await csv.ReadAsync())
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (!columns.Any())
            {
                for (var i = 0; i < csv.Parser.Count; i++)
                {
                    columns.Add($"Column{i + 1}");
                }
            }

            var values = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            for (var i = 0; i < columns.Count; i++)
            {
                values[columns[i]] = i < csv.Parser.Count ? csv.GetField(i) : null;
            }

            rows.Add(new PreviewRow
            {
                Values = values
            });

            rowCount++;
            if (rowCount >= request.MaxRows)
            {
                break;
            }
        }

        return new PreviewFileResponse
        {
            FilePath = request.FilePath,
            Columns = columns,
            Rows = rows
        };
    }

    private static async Task<PreviewFileResponse> PreviewJsonAsync(
        PreviewFileRequest request,
        CancellationToken cancellationToken)
    {
        var json = await File.ReadAllTextAsync(request.FilePath, cancellationToken);

        // Lightweight preview fallback:
        // return raw JSON snippets as single-column rows.
        var lines = json
            .Split(Environment.NewLine, StringSplitOptions.None)
            .Take(request.MaxRows)
            .ToList();

        var rows = lines
            .Select(line => new PreviewRow
            {
                Values = new Dictionary<string, string?>
                {
                    ["Json"] = line
                }
            })
            .ToList();

        return new PreviewFileResponse
        {
            FilePath = request.FilePath,
            Columns = new[] { "Json" },
            Rows = rows
        };
    }
}