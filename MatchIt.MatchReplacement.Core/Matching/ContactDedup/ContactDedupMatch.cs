using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.ContactDedup;

public sealed class ContactDedupMatch
{
    public required ContactRecord Left { get; init; }
    public required ContactRecord Right { get; init; }
    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}