using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Exports;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class ExportDiagnosticsStage : IProcessStage
{
    public string Name => "Export Diagnostics";

    public async Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var exportWriter = new DefaultExportWriter();

        var suppressionInternalDiagnosticsPath =
            context.GetOutputPath("diag.suppression.internal.duplicates");

        var contactInternalDiagnosticsPath =
            context.GetOutputPath("diag.contact.internal.duplicates");

        var suppressionOverlapDiagnosticsPath =
            context.GetOutputPath("diag.suppression.overlaps");

        var siteDedupDiagnosticsPath =
            context.GetOutputPath("diag.site.dedup");

        await exportWriter.WriteAsync(
            suppressionInternalDiagnosticsPath,
            context.SuppressionInternalDuplicateDiagnostics,
            context.ExportOptions,
            cancellationToken);

        await exportWriter.WriteAsync(
            contactInternalDiagnosticsPath,
            context.ContactInternalDuplicateDiagnostics,
            context.ExportOptions,
            cancellationToken);

        await exportWriter.WriteAsync(
            suppressionOverlapDiagnosticsPath,
            context.SuppressionOverlapDiagnostics,
            context.ExportOptions,
            cancellationToken);

        await exportWriter.WriteAsync(
            siteDedupDiagnosticsPath,
            context.SiteDedupDiagnostics,
            context.ExportOptions,
            cancellationToken);

        context.NamedOutputs["DiagSuppressionInternal"] = suppressionInternalDiagnosticsPath;
        context.NamedOutputs["DiagContactInternal"] = contactInternalDiagnosticsPath;
        context.NamedOutputs["DiagSuppressionOverlap"] = suppressionOverlapDiagnosticsPath;
        context.NamedOutputs["DiagSiteDedup"] = siteDedupDiagnosticsPath;

        Console.WriteLine("Diagnostic outputs exported.");
        Console.WriteLine($" - {suppressionInternalDiagnosticsPath}");
        Console.WriteLine($" - {contactInternalDiagnosticsPath}");
        Console.WriteLine($" - {suppressionOverlapDiagnosticsPath}");
        Console.WriteLine($" - {siteDedupDiagnosticsPath}");
    }
}