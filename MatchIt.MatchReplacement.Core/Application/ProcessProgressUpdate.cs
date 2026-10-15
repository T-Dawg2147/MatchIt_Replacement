namespace MatchIt.MatchReplacement.Core.Application;

public sealed class ProcessProgressUpdate
{
    public string ProcessProfileName { get; init; } = string.Empty;
    public string StageName { get; init; } = string.Empty;
    public StageExecutionStatus StageStatus { get; init; }
    public int StageIndex { get; init; }
    public int TotalStages { get; init; }
    public string Message { get; init; } = string.Empty;
    public DateTimeOffset TimestampUtc { get; init; } = DateTimeOffset.UtcNow;
}