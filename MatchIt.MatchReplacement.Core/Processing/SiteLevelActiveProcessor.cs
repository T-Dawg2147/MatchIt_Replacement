using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.SiteDedup;
using MatchIt.MatchReplacement.Core.Models;
using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Processing;

public sealed class SiteLevelActiveProcessor
{
    private readonly MatchConfiguration _configuration;

    public SiteLevelActiveProcessor(MatchConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IReadOnlyList<ContactRecord> Execute(
        IReadOnlyList<ContactRecord> cleanedContacts,
        MetricsCollector metrics)
    {
        ArgumentNullException.ThrowIfNull(cleanedContacts);
        ArgumentNullException.ThrowIfNull(metrics);

        var matcher = new SiteDedupMatcher(_configuration);
        var duplicateMatches = matcher.FindDuplicates(cleanedContacts);

        metrics.Metrics.SiteLevelPotentialDuplicates = duplicateMatches.Count;

        var groups = BuildDuplicateGroups(cleanedContacts, duplicateMatches);

        var recordsToKeep = new HashSet<Guid>();
        foreach (var group in groups)
        {
            var winner = ChooseRepresentative(group);
            recordsToKeep.Add(winner.InternalId);
        }

        foreach (var record in cleanedContacts)
        {
            if (recordsToKeep.Contains(record.InternalId))
            {
                continue;
            }

            if (groups.Any(g => g.Any(r => r.InternalId == record.InternalId)))
            {
                record.IsRemoved = true;
                record.IsDuplicateCandidate = true;
            }
        }

        var finalRecords = cleanedContacts
            .Where(r => !r.IsRemoved)
            .ToList();

        metrics.Metrics.SiteLevelOutputCount = finalRecords.Count;

        return finalRecords;
    }

    private static List<List<ContactRecord>> BuildDuplicateGroups(
        IReadOnlyList<ContactRecord> allRecords,
        IReadOnlyList<SiteDedupMatch> matches)
    {
        var adjacency = new Dictionary<Guid, HashSet<Guid>>();
        var index = allRecords.ToDictionary(r => r.InternalId);

        foreach (var match in matches)
        {
            AddEdge(adjacency, match.Left.InternalId, match.Right.InternalId);
            AddEdge(adjacency, match.Right.InternalId, match.Left.InternalId);
        }

        var visited = new HashSet<Guid>();
        var groups = new List<List<ContactRecord>>();

        foreach (var node in adjacency.Keys)
        {
            if (!visited.Add(node))
            {
                continue;
            }

            var groupIds = new List<Guid>();
            var stack = new Stack<Guid>();
            stack.Push(node);

            while (stack.Count > 0)
            {
                var current = stack.Pop();
                groupIds.Add(current);

                if (!adjacency.TryGetValue(current, out var neighbours))
                {
                    continue;
                }

                foreach (var neighbour in neighbours)
                {
                    if (visited.Add(neighbour))
                    {
                        stack.Push(neighbour);
                    }
                }
            }

            var groupRecords = groupIds
                .Where(index.ContainsKey)
                .Select(id => index[id])
                .ToList();

            if (groupRecords.Count > 0)
            {
                groups.Add(groupRecords);
            }
        }

        return groups;
    }

    private static void AddEdge(
        IDictionary<Guid, HashSet<Guid>> adjacency,
        Guid from,
        Guid to)
    {
        if (!adjacency.TryGetValue(from, out var set))
        {
            set = new HashSet<Guid>();
            adjacency[from] = set;
        }

        set.Add(to);
    }

    private static ContactRecord ChooseRepresentative(IReadOnlyList<ContactRecord> group)
    {
        return group
            .OrderByDescending(CompletenessScore)
            .ThenBy(r => ParseCid(r.Cid))
            .ThenBy(r => r.InternalId)
            .First();
    }

    private static int CompletenessScore(ContactRecord record)
    {
        var score = 0;

        score += HasValue(record.Company) ? 1 : 0;
        score += HasValue(record.Address1) ? 1 : 0;
        score += HasValue(record.Address2) ? 1 : 0;
        score += HasValue(record.Address3) ? 1 : 0;
        score += HasValue(record.Address4) ? 1 : 0;
        score += HasValue(record.Address5) ? 1 : 0;
        score += HasValue(record.Postcode) ? 1 : 0;
        score += HasValue(record.Email) ? 1 : 0;
        score += HasValue(record.Forenames) ? 1 : 0;
        score += HasValue(record.Surname) ? 1 : 0;
        score += HasValue(record.JobTitle) ? 1 : 0;
        score += HasValue(record.Dept) ? 1 : 0;
        score += HasValue(record.LastDateCr) ? 1 : 0;

        return score;
    }

    private static bool HasValue(string? value) =>
        !string.IsNullOrWhiteSpace(value);

    private static long ParseCid(string? cid)
    {
        return long.TryParse(cid, out var value)
            ? value
            : long.MaxValue;
    }
}