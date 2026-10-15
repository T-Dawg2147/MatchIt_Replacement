namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class AddressNormalizer
{
    private readonly TextNormalizer _textNormalizer;

    public AddressNormalizer(TextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public string BuildFullAddress(params string?[] parts)
    {
        return string.Join(' ',
            parts
                .Select(_textNormalizer.NormalizeForComparison)
                .Where(p => !string.IsNullOrWhiteSpace(p)));
    }

    public string NormalizeStreet(params string?[] parts)
    {
        var full = BuildFullAddress(parts);
        if (string.IsNullOrWhiteSpace(full))
        {
            return string.Empty;
        }

        // First-pass simple version:
        // keep address text as a single normalized comparison field.
        return full;
    }
}