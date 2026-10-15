namespace MatchIt.MatchReplacement.Core.Application;

public sealed class PreviewRow
{
    public IReadOnlyDictionary<string, string?> Values { get; init; } =
        new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
}