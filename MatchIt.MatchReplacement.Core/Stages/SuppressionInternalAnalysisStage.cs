using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Processing;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class SuppressionInternalAnalysisStage : IProcessStage
{
    public string Name => "Suppression Internal Analysis";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var processor = new SuppressionAnalysisProcessor(context.Configuration);
        var matches = processor.Execute(context.Suppressions, new Reporting.MetricsCollectorProxy(context.Metrics));

        context.SuppressionInternalDuplicateDiagnostics.Clear();
        context.SuppressionInternalDuplicateDiagnostics.AddRange(matches.Select(DiagnosticRowFactory.From));

        Console.WriteLine($"Suppression potential duplicates: {context.Metrics.SuppressionPotentialDuplicates}");
        Console.WriteLine($"Suppression duplicate pairs     : {matches.Count}");

        return Task.CompletedTask;
    }
}