using MatchIt.MatchReplacement.Core.Profiles;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ProfileStageDefinitionDto
{
    public ProfileStageType StageType { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public bool Enabled { get; init; } = true;
    public IReadOnlyList<string> KeyIds { get; init; } = Array.Empty<string>();
}