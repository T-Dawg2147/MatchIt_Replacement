namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ProfileOutputDefinition
{
    public string Key { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string DefaultFileName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
}