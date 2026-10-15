namespace MatchIt.MatchReplacement.Core.Application;

public interface IProcessProgressObserver
{
    Task ReportAsync(ProcessProgressUpdate update, CancellationToken cancellationToken = default);
}