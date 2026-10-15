using MatchIt.MatchReplacement.Core.Application;
using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Models;
using MatchIt.MatchReplacement.Core.Reporting;

namespace MatchIt.MatchReplacement.Core.Engine;

public sealed class ProcessRunContext
{
    public required MatchConfiguration Configuration { get; init; }
    public required string ContactsCsvPath { get; init; }
    public required string SuppressionCsvPath { get; init; }
    public required string OutputFolder { get; init; }
    public required string ProcessProfileName { get; init; }
    public ExportOptions ExportOptions { get; init; } = new();
    public IProcessProgressObserver ProgressObserver { get; init; } = NullProcessProgressObserver.Instance;

    public RunMetrics Metrics { get; } = new();

    public List<ContactRecord> Contacts { get; set; } = new();
    public List<SuppressionRecord> Suppressions { get; set; } = new();

    public List<ContactRecord> ContactsAfterSuppressionPurge { get; set; } = new();
    public List<ContactRecord> SiteLevelActiveRecords { get; set; } = new();

    public List<SuppressionInternalDuplicateDiagnosticRow> SuppressionInternalDuplicateDiagnostics { get; } = new();
    public List<ContactInternalDuplicateDiagnosticRow> ContactInternalDuplicateDiagnostics { get; } = new();
    public List<SuppressionOverlapDiagnosticRow> SuppressionOverlapDiagnostics { get; } = new();
    public List<SiteDedupDiagnosticRow> SiteDedupDiagnostics { get; } = new();

    public Dictionary<string, string> NamedOutputs { get; } = new(StringComparer.OrdinalIgnoreCase);

    public string GetOutputPath(string logicalFileNameWithoutExtension)
    {
        Directory.CreateDirectory(OutputFolder);
        var extension = GetExtensionForFormat(ExportOptions.Format);
        return Path.Combine(OutputFolder, $"{logicalFileNameWithoutExtension}{extension}");
    }

    public Task ReportProgressAsync(
        string stageName,
        StageExecutionStatus stageStatus,
        int stageIndex,
        int totalStages,
        string message,
        CancellationToken cancellationToken = default)
    {
        return ProgressObserver.ReportAsync(
            new ProcessProgressUpdate
            {
                ProcessProfileName = ProcessProfileName,
                StageName = stageName,
                StageStatus = stageStatus,
                StageIndex = stageIndex,
                TotalStages = totalStages,
                Message = message,
                TimestampUtc = DateTimeOffset.UtcNow
            },
            cancellationToken);
    }

    private static string GetExtensionForFormat(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => ".csv",
            ExportFormat.PipeDelimited => ".txt",
            ExportFormat.Json => ".json",
            _ => ".dat"
        };
    }
}