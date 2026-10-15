using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Processing;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class ContactInternalAnalysisStage : IProcessStage
{
    public string Name => "Contact Internal Analysis";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var processor = new ContactImportProcessor(context.Configuration);
        var matches = processor.Execute(context.Contacts, new Reporting.MetricsCollectorProxy(context.Metrics));

        context.ContactInternalDuplicateDiagnostics.Clear();
        context.ContactInternalDuplicateDiagnostics.AddRange(matches.Select(DiagnosticRowFactory.From));

        Console.WriteLine($"Contact potential duplicates: {context.Metrics.ContactPotentialDuplicates}");
        Console.WriteLine($"Contact duplicate pairs     : {matches.Count}");

        return Task.CompletedTask;
    }
}