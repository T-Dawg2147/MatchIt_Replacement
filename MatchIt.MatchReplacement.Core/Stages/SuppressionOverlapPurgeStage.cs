using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Matching.Suppression;
using MatchIt.MatchReplacement.Core.Processing;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class SuppressionOverlapPurgeStage : IProcessStage
{
    public string Name => "Suppression Overlap Purge";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        // Capture detailed matches before purge
        var matcher = new SuppressionOverlapMatcher(context.Configuration);
        var overlapMatches = matcher.FindOverlaps(context.Contacts, context.Suppressions);

        context.SuppressionOverlapDiagnostics.Clear();
        context.SuppressionOverlapDiagnostics.AddRange(overlapMatches.Select(DiagnosticRowFactory.From));

        var processor = new SuppressionPurgeProcessor(context.Configuration);
        context.ContactsAfterSuppressionPurge = processor
            .Execute(context.Contacts, context.Suppressions, new Reporting.MetricsCollectorProxy(context.Metrics))
            .ToList();

        Console.WriteLine($"Suppression overlap matches      : {context.Metrics.SuppressionOverlapMatches}");
        Console.WriteLine($"Contacts after suppression purge : {context.Metrics.ContactsAfterSuppressionRemoval}");

        return Task.CompletedTask;
    }
}