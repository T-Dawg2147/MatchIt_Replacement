using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.Common;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Matching.Suppression;

public sealed class SuppressionOverlapMatcher
{
    private readonly MatchContext _context;

    public SuppressionOverlapMatcher(MatchConfiguration configuration)
    {
        _context = new MatchContext(configuration);
    }

    public IReadOnlyList<SuppressionOverlapMatch> FindOverlaps(
        IReadOnlyList<ContactRecord> contacts,
        IReadOnlyList<SuppressionRecord> suppressions)
    {
        ArgumentNullException.ThrowIfNull(contacts);
        ArgumentNullException.ThrowIfNull(suppressions);

        var matches = new List<SuppressionOverlapMatch>();

        var suppressionByExactPostcode = suppressions
            .Where(s => !string.IsNullOrWhiteSpace(s.NormalizedPostcode))
            .GroupBy(s => s.NormalizedPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        var suppressionByOutwardPostcode = suppressions
            .Where(s => !string.IsNullOrWhiteSpace(s.OutwardPostcode))
            .GroupBy(s => s.OutwardPostcode!)
            .ToDictionary(g => g.Key, g => g.ToList(), StringComparer.Ordinal);

        foreach (var contact in contacts)
        {
            if (contact.IsRemoved)
            {
                continue;
            }

            var candidates = GetCandidatesForContact(
                contact,
                suppressionByExactPostcode,
                suppressionByOutwardPostcode);

            SuppressionOverlapMatch? best = null;

            foreach (var suppression in candidates)
            {
                var score = Score(contact, suppression);
                if (score.Value < _context.Configuration.Thresholds.SuppressionOverlapThreshold)
                {
                    continue;
                }

                var match = new SuppressionOverlapMatch
                {
                    Contact = contact,
                    Suppression = suppression,
                    Score = score.Value,
                    Reason = score.ToReasonText()
                };

                if (best is null || match.Score > best.Score)
                {
                    best = match;
                }
            }

            if (best is not null)
            {
                matches.Add(best);
            }
        }

        return matches;
    }

    private IEnumerable<SuppressionRecord> GetCandidatesForContact(
        ContactRecord contact,
        IReadOnlyDictionary<string, List<SuppressionRecord>> suppressionByExactPostcode,
        IReadOnlyDictionary<string, List<SuppressionRecord>> suppressionByOutwardPostcode)
    {
        var seen = new HashSet<Guid>();

        if (!string.IsNullOrWhiteSpace(contact.NormalizedPostcode) &&
            suppressionByExactPostcode.TryGetValue(contact.NormalizedPostcode, out var exactPostcodeMatches))
        {
            foreach (var match in exactPostcodeMatches)
            {
                if (seen.Add(match.InternalId))
                {
                    yield return match;
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(contact.OutwardPostcode) &&
            suppressionByOutwardPostcode.TryGetValue(contact.OutwardPostcode, out var outwardMatches))
        {
            foreach (var match in outwardMatches)
            {
                if (seen.Add(match.InternalId))
                {
                    yield return match;
                }
            }
        }

        if (!_context.Configuration.RequirePostcodeFamilyForSuppressionOverlap)
        {
            foreach (var fallback in suppressionByOutwardPostcode.Values.SelectMany(v => v))
            {
                if (seen.Add(fallback.InternalId))
                {
                    yield return fallback;
                }
            }
        }
    }

    private MatchScore Score(ContactRecord contact, SuppressionRecord suppression)
    {
        var w = _context.Configuration.Weights;
        var score = new MatchScore();

        if (EqualsOrdinal(contact.NormalizedPostcode, suppression.NormalizedPostcode))
        {
            score.Add(w.ExactPostcode, "Exact postcode");
        }
        else if (EqualsOrdinal(contact.OutwardPostcode, suppression.OutwardPostcode))
        {
            score.Add(w.OutwardPostcode, "Outward postcode");
        }

        if (EqualsOrdinal(contact.AddKey, suppression.AddKey))
        {
            score.Add(w.AddKey, "ADD_KEY");
        }

        if (EqualsOrdinal(contact.NormalizedCompany, suppression.NormalizedCompany) &&
            !string.IsNullOrWhiteSpace(contact.NormalizedCompany))
        {
            score.Add(w.NormalizedCompany, "Normalized company");
        }
        else if (EqualsOrdinal(contact.CompanyFirstWordPhonetic, suppression.CompanyFirstWordPhonetic) &&
                 !string.IsNullOrWhiteSpace(contact.CompanyFirstWordPhonetic))
        {
            score.Add(w.CompanyFirstWordPhonetic, "Company first-word phonetic");
        }

        if (EqualsOrdinal(contact.StreetPhonetic, suppression.StreetPhonetic) &&
            !string.IsNullOrWhiteSpace(contact.StreetPhonetic))
        {
            score.Add(w.StreetPhonetic, "Street phonetic");
        }

        if (EqualsOrdinal(contact.AddressPhoneticKey, suppression.AddressPhoneticKey) &&
            !string.IsNullOrWhiteSpace(contact.AddressPhoneticKey))
        {
            score.Add(w.AddressPhoneticKey, "Address phonetic key");
        }

        if (EqualsOrdinal(contact.NormalizedStreet, suppression.NormalizedStreet) &&
            !string.IsNullOrWhiteSpace(contact.NormalizedStreet))
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
}