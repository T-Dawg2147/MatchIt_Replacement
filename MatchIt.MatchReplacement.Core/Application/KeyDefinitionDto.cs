namespace MatchIt.MatchReplacement.Core.Application;

public sealed class KeyDefinitionDto
{
    public string KeyId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public bool IsBuiltIn { get; init; }
    public IReadOnlyList<string> SourceFields { get; init; } = Array.Empty<string>();
}