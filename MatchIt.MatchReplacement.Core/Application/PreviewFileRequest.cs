namespace MatchIt.MatchReplacement.Core.Application;

public sealed class PreviewFileRequest
{
    public string FilePath { get; init; } = string.Empty;
    public ExportFormat Format { get; init; } = ExportFormat.Csv;
    public bool HasHeaderRow { get; init; } = true;
    public int MaxRows { get; init; } = 100;
}