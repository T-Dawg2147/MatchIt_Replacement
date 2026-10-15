namespace MatchIt.MatchReplacement.Core.Models;

public sealed class MatchCandidate<T>
{
    public required T Left { get; init; }
    public required T Right { get; init; }
    public decimal Score { get; init; }
    public string MatchReason { get; init; } = string.Empty;
}