using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.Suppression;

public sealed class SuppressionOverlapMatch
{
    public required ContactRecord Contact { get; init; }
    public required SuppressionRecord Suppression { get; init; }
    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}