namespace MatchIt.MatchReplacement.Core.Config;

public sealed class MatchingThresholds
{
    public decimal SuppressionOverlapThreshold { get; init; } = 75m;
    public decimal StrongSuppressionOverlapThreshold { get; init; } = 90m;

    public decimal SiteDedupThreshold { get; init; } = 70m;

    public decimal ContactDedupThreshold { get; init; } = 65m;
    public decimal SuppressionInternalDedupThreshold { get; init; } = 70m;
}