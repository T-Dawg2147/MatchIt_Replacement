namespace MatchIt.MatchReplacement.Core.Application;

public interface IMatchReplacementService
{
    Task<RunProcessResponse> RunAsync(
        RunProcessRequest request,
        CancellationToken cancellationToken = default);
}