using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.Common;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.Suppression;

public sealed class SuppressionInternalDuplicateMatcher
{
    private readonly MatchContext _context;

    public SuppressionInternalDuplicateMatcher(MatchConfiguration configuration)
    {
        _context = new MatchContext(configuration);
    }

    public IReadOnlyList<SuppressionInternalDuplicateMatch> FindDuplicates(
        IReadOnlyList<SuppressionRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        var matches = new List<SuppressionInternalDuplicateMatch>();

        var byExactPostcode = records
            .Where(r => !string.IsNullOrWhiteSpace(r.NormalizedPostcode))
            .GroupBy(r => r.NormalizedPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var byOutwardPostcode = records
            .Where(r => !string.IsNullOrWhiteSpace(r.OutwardPostcode))
            .GroupBy(r => r.OutwardPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var visitedPairs = new HashSet<string>(StringComparer.Ordinal);

        foreach (var record in records)
        {
            var candidates = GetCandidates(record, byExactPostcode, byOutwardPostcode);

            foreach (var candidate in candidates)
            {
                if (candidate.InternalId == record.InternalId)
                {
                    continue;
                }

                var pairKey = BuildPairKey(record.InternalId, candidate.InternalId);
                if (!visitedPairs.Add(pairKey))
                {
                    continue;
                }

                var score = Score(record, candidate);
                if (score.Value < _context.Configuration.Thresholds.SuppressionInternalDedupThreshold)
                {
                    continue;
                }

                matches.Add(new SuppressionInternalDuplicateMatch
                {
                    Left = record,
                    Right = candidate,
                    Score = score.Value,
                    Reason = score.ToReasonText()
                });
            }
        }

        return matches;
    }

    private IEnumerable<SuppressionRecord> GetCandidates(
        SuppressionRecord record,
        IReadOnlyDictionary<string, List<SuppressionRecord>> byExactPostcode,
        IReadOnlyDictionary<string, List<SuppressionRecord>> byOutwardPostcode)
    {
        var seen = new HashSet<Guid>();

        if (!string.IsNullOrWhiteSpace(record.NormalizedPostcode) &&
            byExactPostcode.TryGetValue(record.NormalizedPostcode, out var exact))
        {
            foreach (var candidate in exact)
            {
                if (seen.Add(candidate.InternalId))
                {
                    yield return candidate;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(record.OutwardPostcode) &&
            byOutwardPostcode.TryGetValue(record.OutwardPostcode, out var outward))
        {
            foreach (var candidate in outward)
            {
                if (seen.Add(candidate.InternalId))
                {
                    yield return candidate;
                }
            }
        }
    }

    private MatchScore Score(SuppressionRecord left, SuppressionRecord right)
    {
        var w = _context.Configuration.Weights;
        var score = new MatchScore();

        if (EqualsOrdinal(left.OutwardPostcode, right.OutwardPostcode) &&
            EqualsOrdinal(left.CompanyFirstWordPhonetic, right.CompanyFirstWordPhonetic) &&
            !string.IsNullOrWhiteSpace(left.CompanyFirstWordPhonetic))
        {
            score.Add(w.OutwardPostcodeAndCompanyPhoneticCombo, "Outward postcode + first-company-word phonetic");
        }
        else if (EqualsOrdinal(left.NormalizedPostcode, right.NormalizedPostcode))
        {
            score.Add(w.ExactPostcode, "Exact postcode");
        }
        else if (EqualsOrdinal(left.OutwardPostcode, right.OutwardPostcode))
        {
            score.Add(w.OutwardPostcode, "Outward postcode");
        }

        if (EqualsOrdinal(left.CompanyFirstWordPhonetic, right.CompanyFirstWordPhonetic) &&
            EqualsOrdinal(left.StreetPhonetic, right.StreetPhonetic) &&
            !string.IsNullOrWhiteSpace(left.CompanyFirstWordPhonetic) &&
            !string.IsNullOrWhiteSpace(left.StreetPhonetic))
        {
            score.Add(w.CompanyAndStreetPhoneticCombo, "First-company-word phonetic + street phonetic");
        }

        if (EqualsOrdinal(left.NormalizedPostcode, right.NormalizedPostcode))
        {
            score.Add(15m, "Postcode");
        }

        if (EqualsOrdinal(left.NormalizedCompany, right.NormalizedCompany) &&
            !string.IsNullOrWhiteSpace(left.NormalizedCompany))
        {
            score.Add(w.NormalizedCompany, "Normalized company");
        }

        if (EqualsOrdinal(left.AddressPhoneticKey, right.AddressPhoneticKey) &&
            !string.IsNullOrWhiteSpace(left.AddressPhoneticKey))
        {
            score.Add(w.AddressPhoneticKey, "Address phonetic key");
        }

        if (EqualsOrdinal(left.AddKey, right.AddKey) &&
            !string.IsNullOrWhiteSpace(left.AddKey))
        {
            score.Add(w.AddKey, "ADD_KEY");
        }

        return score;
    }

    private static bool EqualsOrdinal(string? left, string? right)
    {
        return !string.IsNullOrWhiteSpace(left) &&
               !string.IsNullOrWhiteSpace(right) &&
               string.Equals(left, right, StringComparison.Ordinal);
    }

    private static string BuildPairKey(Guid left, Guid right)
    {
        return left.CompareTo(right) < 0
            ? $"{left:N}|{right:N}"
            : $"{right:N}|{left:N}";
    }
}