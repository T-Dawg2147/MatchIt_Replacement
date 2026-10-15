namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ExportFormatDescriptor
{
    public ExportFormat Format { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string FileExtension { get; init; } = string.Empty;
}