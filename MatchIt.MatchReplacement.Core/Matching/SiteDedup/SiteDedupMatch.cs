using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.SiteDedup;

public sealed class SiteDedupMatch
{
    public required ContactRecord Left { get; init; }
    public required ContactRecord Right { get; init; }
    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}