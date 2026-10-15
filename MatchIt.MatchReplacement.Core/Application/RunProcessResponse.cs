using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class RunProcessResponse
{
    public string ProcessProfileName { get; init; } = string.Empty;
    public bool Success { get; init; }
    public ProcessRunStatus Status { get; init; }
    public RunMetrics Metrics { get; init; } = new();
    public DiagnosticSummary Diagnostics { get; init; } = new();
    public IReadOnlyList<ProcessOutputFile> OutputFiles { get; init; } = Array.Empty<ProcessOutputFile>();
}