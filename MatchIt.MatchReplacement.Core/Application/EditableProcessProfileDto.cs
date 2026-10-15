namespace MatchIt.MatchReplacement.Core.Application;

public sealed class EditableProcessProfileDto
{
    public string Name { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public IReadOnlyList<ProfileInputDefinition> Inputs { get; init; } =
        Array.Empty<ProfileInputDefinition>();

    public IReadOnlyList<ProfileOutputDefinition> Outputs { get; init; } =
        Array.Empty<ProfileOutputDefinition>();

    public IReadOnlyList<ProfileStageDefinitionDto> Stages { get; init; } =
        Array.Empty<ProfileStageDefinitionDto>();

    public IReadOnlyList<BuiltInKeyDefinitionDto> BuiltInKeys { get; init; } =
        Array.Empty<BuiltInKeyDefinitionDto>();

    public IReadOnlyList<CustomKeyDefinitionDto> CustomKeys { get; init; } =
        Array.Empty<CustomKeyDefinitionDto>();
}