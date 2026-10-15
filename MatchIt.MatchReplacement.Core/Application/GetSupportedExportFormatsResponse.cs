namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetSupportedExportFormatsResponse
{
    public IReadOnlyList<ExportFormat> Formats { get; init; } =
        Array.Empty<ExportFormat>();
}