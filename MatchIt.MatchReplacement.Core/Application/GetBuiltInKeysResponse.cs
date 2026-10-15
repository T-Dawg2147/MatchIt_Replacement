namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetBuiltInKeysResponse
{
    public IReadOnlyList<BuiltInKeyDefinitionDto> Keys { get; init; } =
        Array.Empty<BuiltInKeyDefinitionDto>();
}