namespace MatchIt.MatchReplacement.Core.Application;

public sealed class PreviewFileResponse
{
    public string FilePath { get; init; } = string.Empty;
    public IReadOnlyList<string> Columns { get; init; } = Array.Empty<string>();
    public IReadOnlyList<PreviewRow> Rows { get; init; } = Array.Empty<PreviewRow>();
}