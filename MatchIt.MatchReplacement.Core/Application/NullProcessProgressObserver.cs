namespace MatchIt.MatchReplacement.Core.Application;

public sealed class NullProcessProgressObserver : IProcessProgressObserver
{
    public static NullProcessProgressObserver Instance { get; } = new();

    private NullProcessProgressObserver()
    {
    }

    public Task ReportAsync(ProcessProgressUpdate update, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}