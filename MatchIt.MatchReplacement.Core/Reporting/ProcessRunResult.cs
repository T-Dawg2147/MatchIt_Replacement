using MatchIt.MatchReplacement.Core.Diagnostics;
using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Reporting;

public sealed class ProcessRunResult
{
    public RunMetrics Metrics { get; init; } = new();

    public IReadOnlyList<ContactRecord> ContactsAfterSuppressionPurge { get; init; } = Array.Empty<ContactRecord>();
    public IReadOnlyList<ContactRecord> SiteLevelActiveRecords { get; init; } = Array.Empty<ContactRecord>();

    public IReadOnlyList<SuppressionInternalDuplicateDiagnosticRow> SuppressionInternalDuplicateDiagnostics { get; init; }
        = Array.Empty<SuppressionInternalDuplicateDiagnosticRow>();

    public IReadOnlyList<ContactInternalDuplicateDiagnosticRow> ContactInternalDuplicateDiagnostics { get; init; }
        = Array.Empty<ContactInternalDuplicateDiagnosticRow>();

    public IReadOnlyList<SuppressionOverlapDiagnosticRow> SuppressionOverlapDiagnostics { get; init; }
        = Array.Empty<SuppressionOverlapDiagnosticRow>();

    public IReadOnlyList<SiteDedupDiagnosticRow> SiteDedupDiagnostics { get; init; }
        = Array.Empty<SiteDedupDiagnosticRow>();
}