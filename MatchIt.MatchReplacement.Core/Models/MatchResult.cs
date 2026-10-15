namespace MatchIt.MatchReplacement.Core.Models;

public sealed class MatchResult<T>
{
    public List<MatchCandidate<T>> Candidates { get; } = new();
    public int RecordsScanned { get; set; }
    public int MatchedCount { get; set; }
}