namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ProfileInputDefinition
{
    public string Key { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool Required { get; init; }
}