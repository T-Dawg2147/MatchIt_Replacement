namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ProcessProfileDescriptor
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public IReadOnlyList<ProfileInputDefinition> Inputs { get; init; } =
        Array.Empty<ProfileInputDefinition>();

    public IReadOnlyList<ProfileOutputDefinition> Outputs { get; init; } =
        Array.Empty<ProfileOutputDefinition>();

    public IReadOnlyList<ExportFormat> SupportedExportFormats { get; init; } =
        Array.Empty<ExportFormat>();
}