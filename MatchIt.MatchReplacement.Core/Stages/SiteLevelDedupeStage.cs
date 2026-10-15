using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Matching.SiteDedup;
using MatchIt.MatchReplacement.Core.Processing;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class SiteLevelDedupeStage : IProcessStage
{
    public string Name => "Site Level Dedupe";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var matcher = new SiteDedupMatcher(context.Configuration);
        var matches = matcher.FindDuplicates(context.ContactsAfterSuppressionPurge);

        context.SiteDedupDiagnostics.Clear();
        context.SiteDedupDiagnostics.AddRange(matches.Select(DiagnosticRowFactory.From));

        var processor = new SiteLevelActiveProcessor(context.Configuration);
        context.SiteLevelActiveRecords = processor
            .Execute(context.ContactsAfterSuppressionPurge, new Reporting.MetricsCollectorProxy(context.Metrics))
            .ToList();

        Console.WriteLine($"Site-level potential duplicates: {context.Metrics.SiteLevelPotentialDuplicates}");
        Console.WriteLine($"Site-level output count        : {context.Metrics.SiteLevelOutputCount}");

        return Task.CompletedTask;
    }
}