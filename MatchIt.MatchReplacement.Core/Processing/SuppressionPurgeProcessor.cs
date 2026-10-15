using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.Suppression;
using MatchIt.MatchReplacement.Core.Models;
using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Processing;

public sealed class SuppressionPurgeProcessor
{
    private readonly MatchConfiguration _configuration;

    public SuppressionPurgeProcessor(MatchConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IReadOnlyList<ContactRecord> Execute(
        IReadOnlyList<ContactRecord> contacts,
        IReadOnlyList<SuppressionRecord> suppressions,
        MetricsCollector metrics)
    {
        ArgumentNullException.ThrowIfNull(contacts);
        ArgumentNullException.ThrowIfNull(suppressions);
        ArgumentNullException.ThrowIfNull(metrics);

        var matcher = new SuppressionOverlapMatcher(_configuration);
        var overlapMatches = matcher.FindOverlaps(contacts, suppressions);

        var matchedContactIds = overlapMatches
            .Select(m => m.Contact.InternalId)
            .ToHashSet();

        foreach (var contact in contacts)
        {
            if (matchedContactIds.Contains(contact.InternalId))
            {
                contact.IsSuppressedOverlap = true;
                contact.IsRemoved = true;
            }
        }

        metrics.Metrics.SuppressionOverlapMatches = matchedContactIds.Count;

        var remaining = contacts
            .Where(c => !c.IsRemoved)
            .ToList();

        metrics.Metrics.ContactsAfterSuppressionRemoval = remaining.Count;

        return remaining;
    }
}