namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class PostcodeNormalizer
{
    private readonly TextNormalizer _textNormalizer;

    public PostcodeNormalizer(TextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public string Normalize(string? postcode)
    {
        var normalized = _textNormalizer.NormalizeForComparison(postcode)
            .Replace(" ", string.Empty);

        if (normalized.Length <= 3)
        {
            return normalized;
        }

        return $"{normalized[..^3]} {normalized[^3..]}";
    }

    public string ExtractOutward(string? postcode)
    {
        var normalized = Normalize(postcode);
        var parts = normalized.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parts.Length > 0 ? parts[0] : string.Empty;
    }
}