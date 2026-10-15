namespace MatchIt.MatchReplacement.Core.Application;

public sealed class RunProcessRequest
{
    public string ProcessProfileName { get; init; } = string.Empty;
    public string ContactsCsvPath { get; init; } = string.Empty;
    public string SuppressionCsvPath { get; init; } = string.Empty;
    public string OutputFolder { get; init; } = string.Empty;
    public ExportOptions ExportOptions { get; init; } = new();
    public IProcessProgressObserver ProgressObserver { get; init; } = NullProcessProgressObserver.Instance;
}