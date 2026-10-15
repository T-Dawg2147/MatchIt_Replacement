using System.Text;
using System.Text.RegularExpressions;

namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class TextNormalizer
{
    private static readonly Regex MultiWhitespaceRegex =
        new(@"\s+", RegexOptions.Compiled);

    private static readonly Regex PunctuationRegex =
        new(@"[^\w\s]", RegexOptions.Compiled);

    public string NormalizeBasic(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var result = value.Trim().ToUpperInvariant();
        result = MultiWhitespaceRegex.Replace(result, " ");
        return result;
    }

    public string NormalizeForComparison(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var basic = NormalizeBasic(value);
        basic = PunctuationRegex.Replace(basic, " ");
        basic = MultiWhitespaceRegex.Replace(basic, " ").Trim();
        return basic;
    }

    public string RemoveDiacritics(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return string.Empty;
        }

        var normalized = value.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            var category = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(ch);
            if (category != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder
            .ToString()
            .Normalize(NormalizationForm.FormC)
            .ToUpperInvariant();
    }
}