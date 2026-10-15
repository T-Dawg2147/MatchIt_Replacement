using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.IO;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class LoadInputsStage : IProcessStage
{
    public string Name => "Load Inputs";

    public async Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var reader = new CsvReaderService();

        Console.WriteLine("Loading suppression records...");
        context.Suppressions = await reader.ReadSuppressionsAsync(context.SuppressionCsvPath);
        context.Metrics.SuppressionInputCount = context.Suppressions.Count;

        Console.WriteLine("Loading contact records...");
        context.Contacts = await reader.ReadContactsAsync(context.ContactsCsvPath);
        context.Metrics.ContactInputCount = context.Contacts.Count;

        Console.WriteLine($"Suppression input count: {context.Suppressions.Count}");
        Console.WriteLine($"Contact input count    : {context.Contacts.Count}");
    }
}