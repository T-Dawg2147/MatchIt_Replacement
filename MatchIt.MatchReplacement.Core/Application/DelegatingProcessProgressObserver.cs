namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DelegatingProcessProgressObserver : IProcessProgressObserver
{
    private readonly Func<ProcessProgressUpdate, CancellationToken, Task> _callback;

    public DelegatingProcessProgressObserver(
        Func<ProcessProgressUpdate, CancellationToken, Task> callback)
    {
        _callback = callback ?? throw new ArgumentNullException(nameof(callback));
    }

    public Task ReportAsync(ProcessProgressUpdate update, CancellationToken cancellationToken = default)
    {
        return _callback(update, cancellationToken);
    }
}