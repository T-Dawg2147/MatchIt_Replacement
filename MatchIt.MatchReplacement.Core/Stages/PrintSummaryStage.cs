using MatchIt.MatchReplacement.Core.Engine;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class PrintSummaryStage : IProcessStage
{
    public string Name => "Print Summary";

    public Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Named outputs:");
        foreach (var output in context.NamedOutputs.OrderBy(kvp => kvp.Key))
        {
            Console.WriteLine($" - {output.Key}: {output.Value}");
        }

        Console.WriteLine();
        Console.WriteLine("Run metrics:");
        Console.WriteLine(context.Metrics.ToDisplayText());

        return Task.CompletedTask;
    }
}