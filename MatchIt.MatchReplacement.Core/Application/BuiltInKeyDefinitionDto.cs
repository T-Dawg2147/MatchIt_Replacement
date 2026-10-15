namespace MatchIt.MatchReplacement.Core.Application;

public sealed class BuiltInKeyDefinitionDto
{
    public string KeyId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<string> SourceFields { get; init; } = Array.Empty<string>();
    public bool SupportsContacts { get; init; }
    public bool SupportsSuppressions { get; init; }
}