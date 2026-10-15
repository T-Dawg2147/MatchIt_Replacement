using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.ContactDedup;
using MatchIt.MatchReplacement.Core.Models;
using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Processing;

public sealed class ContactImportProcessor
{
    private readonly MatchConfiguration _configuration;

    public ContactImportProcessor(MatchConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IReadOnlyList<ContactDedupMatch> Execute(
        IReadOnlyList<ContactRecord> contacts,
        MetricsCollector metrics)
    {
        ArgumentNullException.ThrowIfNull(contacts);
        ArgumentNullException.ThrowIfNull(metrics);

        var matcher = new ContactDedupMatcher(_configuration);
        var matches = matcher.FindDuplicates(contacts);

        var duplicateRecordIds = matches
            .SelectMany(m => new[] { m.Left.InternalId, m.Right.InternalId })
            .Distinct()
            .ToHashSet();

        foreach (var contact in contacts)
        {
            if (duplicateRecordIds.Contains(contact.InternalId))
            {
                contact.IsDuplicateCandidate = true;
            }
        }

        metrics.Metrics.ContactPotentialDuplicates = duplicateRecordIds.Count;

        return matches;
    }
}