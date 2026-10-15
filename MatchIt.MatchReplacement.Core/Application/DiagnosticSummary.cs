namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DiagnosticSummary
{
    public int SuppressionInternalDuplicateCount { get; init; }
    public int ContactInternalDuplicateCount { get; init; }
    public int SuppressionOverlapCount { get; init; }
    public int SiteDedupCount { get; init; }
}