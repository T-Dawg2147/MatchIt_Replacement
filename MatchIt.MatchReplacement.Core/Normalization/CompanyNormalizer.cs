namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class CompanyNormalizer
{
    private static readonly HashSet<string> LegalSuffixes = new(StringComparer.OrdinalIgnoreCase)
    {
        "LTD",
        "LIMITED",
        "PLC",
        "LLP",
        "INC",
        "INCORPORATED",
        "CO",
        "COMPANY",
        "CORP",
        "CORPORATION"
    };

    private readonly TextNormalizer _textNormalizer;

    public CompanyNormalizer(TextNormalizer textNormalizer)
    {
        _textNormalizer = textNormalizer;
    }

    public string NormalizeCompany(string? company)
    {
        var normalized = _textNormalizer.NormalizeForComparison(company);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        var tokens = normalized
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Where(t => !LegalSuffixes.Contains(t))
            .ToArray();

        return string.Join(' ', tokens);
    }

    public string ExtractFirstWord(string? company)
    {
        var normalized = NormalizeCompany(company);
        if (string.IsNullOrWhiteSpace(normalized))
        {
            return string.Empty;
        }

        var firstSpace = normalized.IndexOf(' ');
        return firstSpace < 0 ? normalized : normalized[..firstSpace];
    }
}