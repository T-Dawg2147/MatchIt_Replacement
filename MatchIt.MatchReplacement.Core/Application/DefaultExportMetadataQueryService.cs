using MatchIt.MatchReplacement.Core.Exports;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DefaultExportMetadataQueryService : IExportMetadataQueryService
{
    public Task<GetAvailableExportsResponse> GetAvailableExportsAsync(
        CancellationToken cancellationToken = default)
    {
        var formats = Enum.GetValues<ExportFormat>()
            .Select(format => new ExportFormatDescriptor
            {
                Format = format,
                DisplayName = format switch
                {
                    ExportFormat.Csv => "CSV",
                    ExportFormat.PipeDelimited => "Pipe-delimited text",
                    ExportFormat.Json => "JSON",
                    _ => format.ToString()
                },
                FileExtension = ExportFileNameHelper.GetExtension(format)
            })
            .ToList();

        return Task.FromResult(new GetAvailableExportsResponse
        {
            Formats = formats
        });
    }
}