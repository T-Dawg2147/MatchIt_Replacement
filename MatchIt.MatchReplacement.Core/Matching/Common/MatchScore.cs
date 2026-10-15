namespace MatchIt.MatchReplacement.Core.Matching.Common;

public sealed class MatchScore
{
    private readonly List<string> _reasons = new();

    public decimal Value { get; private set; }

    public IReadOnlyList<string> Reasons => _reasons;

    public void Add(decimal points, string reason)
    {
        if (points <= 0)
        {
            return;
        }

        Value += points;
        _reasons.Add(reason);
    }

    public string ToReasonText()
    {
        return string.Join("; ", _reasons);
    }
}