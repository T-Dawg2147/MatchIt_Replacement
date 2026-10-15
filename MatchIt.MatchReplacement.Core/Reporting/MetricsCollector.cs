namespace MatchIt.MatchReplacement.Core.Reporting;

public class MetricsCollector
{
    public virtual RunMetrics Metrics { get; } = new();
}