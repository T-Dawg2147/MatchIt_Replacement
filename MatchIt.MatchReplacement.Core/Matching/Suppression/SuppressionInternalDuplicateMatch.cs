using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.Suppression;

public sealed class SuppressionInternalDuplicateMatch
{
    public required SuppressionRecord Left { get; init; }
    public required SuppressionRecord Right { get; init; }
    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}