namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ExportOptions
{
    public ExportFormat Format { get; init; } = ExportFormat.Csv;
    public bool IncludeHeaders { get; init; } = true;
    public bool QuoteAllFields { get; init; } = true;
}