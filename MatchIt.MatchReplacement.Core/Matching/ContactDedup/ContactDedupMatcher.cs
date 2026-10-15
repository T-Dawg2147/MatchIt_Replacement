using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.Common;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.ContactDedup;

public sealed class ContactDedupMatcher
{
    private readonly MatchContext _context;

    public ContactDedupMatcher(MatchConfiguration configuration)
    {
        _context = new MatchContext(configuration);
    }

    public IReadOnlyList<ContactDedupMatch> FindDuplicates(IReadOnlyList<ContactRecord> records)
    {
        ArgumentNullException.ThrowIfNull(records);

        var matches = new List<ContactDedupMatch>();

        var byExactPostcode = records
            .Where(r => !r.IsRemoved && !string.IsNullOrWhiteSpace(r.NormalizedPostcode))
            .GroupBy(r => r.NormalizedPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var byOutwardPostcode = records
            .Where(r => !r.IsRemoved && !string.IsNullOrWhiteSpace(r.OutwardPostcode))
            .GroupBy(r => r.OutwardPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var visitedPairs = new HashSet<string>(StringComparer.Ordinal);

        foreach (var record in records.Where(r => !r.IsRemoved))
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
                if (score.Value < _context.Configuration.Thresholds.ContactDedupThreshold)
                {
                    continue;
                }

                matches.Add(new ContactDedupMatch
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

    private IEnumerable<ContactRecord> GetCandidates(
        ContactRecord record,
        IReadOnlyDictionary<string, List<ContactRecord>> byExactPostcode,
        IReadOnlyDictionary<string, List<ContactRecord>> byOutwardPostcode)
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

    private MatchScore Score(ContactRecord left, ContactRecord right)
    {
        var w = _context.Configuration.Weights;
        var score = new MatchScore();

        if (EqualsOrdinal(left.OutwardPostcode, right.OutwardPostcode) &&
            EqualsOrdinal(left.SurnamePhonetic, right.SurnamePhonetic) &&
            !string.IsNullOrWhiteSpace(left.SurnamePhonetic))
        {
            score.Add(w.OutwardPostcodeAndSurnamePhoneticCombo, "Outward postcode + surname phonetic");
        }
        else if (EqualsOrdinal(left.NormalizedPostcode, right.NormalizedPostcode))
        {
            score.Add(w.ExactPostcode, "Exact postcode");
        }
        else if (EqualsOrdinal(left.OutwardPostcode, right.OutwardPostcode))
        {
            score.Add(w.OutwardPostcode, "Outward postcode");
        }

        if (EqualsOrdinal(left.SurnamePhonetic, right.SurnamePhonetic) &&
            EqualsOrdinal(left.StreetPhonetic, right.StreetPhonetic) &&
            !string.IsNullOrWhiteSpace(left.SurnamePhonetic) &&
            !string.IsNullOrWhiteSpace(left.StreetPhonetic))
        {
            score.Add(w.SurnameAndStreetPhoneticCombo, "Surname phonetic + street phonetic");
        }

        if (EqualsOrdinal(left.NormalizedPostcode, right.NormalizedPostcode))
        {
            score.Add(15m, "Postcode");
        }

        if (EqualsOrdinal(left.Email, right.Email) &&
            !string.IsNullOrWhiteSpace(left.Email))
        {
            score.Add(w.ExactEmail, "Exact email");
        }

        if (EqualsOrdinal(left.CompanyFirstWordPhonetic, right.CompanyFirstWordPhonetic) &&
            !string.IsNullOrWhiteSpace(left.CompanyFirstWordPhonetic))
        {
            score.Add(w.CompanyFirstWordPhonetic, "Company first-word phonetic");
        }

        if (EqualsOrdinal(left.NormalizedStreet, right.NormalizedStreet) &&
            !string.IsNullOrWhiteSpace(left.NormalizedStreet))
        {
            score.Add(w.NormalizedStreet, "Normalized street");
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