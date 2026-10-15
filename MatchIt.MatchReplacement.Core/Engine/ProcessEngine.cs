using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Engine;

public sealed class ProcessEngine
{
    public async Task RunAsync(
        ProcessDefinition definition,
        ProcessRunContext context,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ArgumentNullException.ThrowIfNull(context);

        Console.WriteLine($"Running process definition: {definition.Name}");
        Console.WriteLine();

        for (var i = 0; i < definition.Stages.Count; i++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var stage = definition.Stages[i];
            var stageIndex = i + 1;
            var totalStages = definition.Stages.Count;

            await context.ReportProgressAsync(
                stage.Name,
                StageExecutionStatus.Started,
                stageIndex,
                totalStages,
                $"Stage '{stage.Name}' started.",
                cancellationToken);

            Console.WriteLine($"--- Stage: {stage.Name} ---");

            try
            {
                await stage.ExecuteAsync(context, cancellationToken);

                await context.ReportProgressAsync(
                    stage.Name,
                    StageExecutionStatus.Completed,
                    stageIndex,
                    totalStages,
                    $"Stage '{stage.Name}' completed.",
                    cancellationToken);
            }
            catch (Exception ex)
            {
                await context.ReportProgressAsync(
                    stage.Name,
                    StageExecutionStatus.Failed,
                    stageIndex,
                    totalStages,
                    $"Stage '{stage.Name}' failed: {ex.Message}",
                    cancellationToken);

                throw;
            }

            Console.WriteLine();
        }

        Console.WriteLine($"Process definition complete: {definition.Name}");
    }
}