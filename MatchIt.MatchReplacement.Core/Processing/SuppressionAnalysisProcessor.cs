using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Matching.Suppression;
using MatchIt.MatchReplacement.Core.Models;
using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Processing;

public sealed class SuppressionAnalysisProcessor
{
    private readonly MatchConfiguration _configuration;

    public SuppressionAnalysisProcessor(MatchConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IReadOnlyList<SuppressionInternalDuplicateMatch> Execute(
        IReadOnlyList<SuppressionRecord> suppressions,
        MetricsCollector metrics)
    {
        ArgumentNullException.ThrowIfNull(suppressions);
        ArgumentNullException.ThrowIfNull(metrics);

        var matcher = new SuppressionInternalDuplicateMatcher(_configuration);
        var matches = matcher.FindDuplicates(suppressions);

        var distinctRecordCount = matches
            .SelectMany(m => new[] { m.Left.InternalId, m.Right.InternalId })
            .Distinct()
            .Count();

        metrics.Metrics.SuppressionPotentialDuplicates = distinctRecordCount;

        return matches;
    }
}