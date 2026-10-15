namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetAvailableExportsResponse
{
    public IReadOnlyList<ExportFormatDescriptor> Formats { get; init; } =
        Array.Empty<ExportFormatDescriptor>();
}