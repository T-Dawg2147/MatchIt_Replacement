namespace MatchIt.MatchReplacement.Core.Application;

public sealed class CustomKeyDefinitionDto
{
    public string KeyId { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public IReadOnlyList<string> SourceFields { get; init; } = Array.Empty<string>();
    public string Expression { get; init; } = string.Empty;
}