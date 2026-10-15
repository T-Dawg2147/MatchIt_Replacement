namespace MatchIt.MatchReplacement.Core.Reporting;

public sealed class MetricsCollectorProxy : MetricsCollector
{
    public MetricsCollectorProxy(RunMetrics metrics)
    {
        Metrics = metrics ?? throw new ArgumentNullException(nameof(metrics));
    }

    public new RunMetrics Metrics { get; }
}