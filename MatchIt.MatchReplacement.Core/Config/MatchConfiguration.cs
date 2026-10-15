namespace MatchIt.MatchReplacement.Core.Config;

public sealed class MatchConfiguration
{
    public MatchingThresholds Thresholds { get; init; } = new();
    public ScoringWeights Weights { get; init; } = new();

    public bool RequirePostcodeFamilyForSuppressionOverlap { get; init; } = true;
    public bool PreferExactPostcodeMatches { get; init; } = true;
}