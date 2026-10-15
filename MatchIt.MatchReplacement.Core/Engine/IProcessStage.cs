namespace MatchIt.MatchReplacement.Core.Engine;

public interface IProcessStage
{
    string Name { get; }

    Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default);
}