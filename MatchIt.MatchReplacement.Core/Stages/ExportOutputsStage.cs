using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Exports;
using MatchIt.MatchReplacement.Core.IO;

namespace MatchIt.MatchReplacement.Core.Stages;

public sealed class ExportOutputsStage : IProcessStage
{
    public string Name => "Export Outputs";

    public async Task ExecuteAsync(ProcessRunContext context, CancellationToken cancellationToken = default)
    {
        var exportWriter = new DefaultExportWriter();
        var projectionWriter = new ProjectionExportService();

        var suppressionSnapshotPath = context.GetOutputPath("suppressions.normalized.snapshot");
        var output2Path = context.GetOutputPath("2 LAST DATE CR");
        var output3Path = context.GetOutputPath("3 SITE LEVEL DATA ACTIVE");

        await exportWriter.WriteAsync(
            suppressionSnapshotPath,
            context.Suppressions,
            context.ExportOptions,
            cancellationToken);

        var output2Rows = context.ContactsAfterSuppressionPurge
            .Select(ContactExportProjection.From)
            .ToList();

        var output3Rows = context.SiteLevelActiveRecords
            .Select(ContactExportProjection.From)
            .ToList();

        await projectionWriter.WriteAsync(
            output2Path,
            output2Rows,
            context.ExportOptions,
            cancellationToken);

        await projectionWriter.WriteAsync(
            output3Path,
            output3Rows,
            context.ExportOptions,
            cancellationToken);

        context.NamedOutputs["SuppressionSnapshot"] = suppressionSnapshotPath;
        context.NamedOutputs["Output2"] = output2Path;
        context.NamedOutputs["Output3"] = output3Path;

        Console.WriteLine("Primary outputs exported.");
        Console.WriteLine($" - {suppressionSnapshotPath}");
        Console.WriteLine($" - {output2Path}");
        Console.WriteLine($" - {output3Path}");
    }
}