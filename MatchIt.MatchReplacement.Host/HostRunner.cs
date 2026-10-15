using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Host;

public sealed class HostRunner
{
    public async Task RunAsync(RunOptions options, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("Starting MatchIt.MatchReplacement.Host");
        Console.WriteLine($"Process Profile : {options.ProcessProfileName}");
        Console.WriteLine($"Contacts CSV    : {options.ContactsCsvPath}");
        Console.WriteLine($"Suppression CSV : {options.SuppressionCsvPath}");
        Console.WriteLine($"Output Folder   : {options.OutputFolder}");
        Console.WriteLine($"Export Format   : {options.ExportFormat}");
        Console.WriteLine();

        IMatchReplacementService service = new MatchReplacementService();
        IMatchReplacementQueryService queryService = new DefaultMatchReplacementQueryService();
        IExportOptionsQueryService exportQueryService = new DefaultExportOptionsQueryService();
        IExportMetadataQueryService exportMetadataService = new DefaultExportMetadataQueryService();
        IResultPreviewService previewService = new ResultPreviewService();
        IProfileAuthoringQueryService profileAuthoringService = new DefaultProfileAuthoringQueryService();
        IKeyCatalogQueryService keyCatalogService = new DefaultKeyCatalogQueryService();

        var profiles = await queryService.GetProfilesAsync(cancellationToken);
        var supportedFormats = await exportQueryService.GetSupportedFormatsAsync(cancellationToken);
        var exportMetadata = await exportMetadataService.GetAvailableExportsAsync(cancellationToken);
        var editableProfile = await profileAuthoringService.GetEditableProfileAsync(
            options.ProcessProfileName,
            cancellationToken);
        var builtInKeys = await keyCatalogService.GetBuiltInKeysAsync(cancellationToken);

        Console.WriteLine("Available profiles:");
        foreach (var profile in profiles.Profiles)
        {
            Console.WriteLine($" - {profile.DisplayName}");
        }

        Console.WriteLine();
        Console.WriteLine("Supported export formats:");
        foreach (var format in supportedFormats.Formats)
        {
            Console.WriteLine($" - {format}");
        }

        Console.WriteLine();
        Console.WriteLine("Export format metadata:");
        foreach (var format in exportMetadata.Formats)
        {
            Console.WriteLine($" - {format.DisplayName} ({format.FileExtension})");
        }

        Console.WriteLine();
        Console.WriteLine("Editable profile stage summary:");
        foreach (var stage in editableProfile.Profile.Stages)
        {
            Console.WriteLine($" - {stage.DisplayName} | Enabled={stage.Enabled} | Keys={string.Join(", ", stage.KeyIds)}");
        }

        Console.WriteLine();
        Console.WriteLine("Built-in keys:");
        foreach (var key in builtInKeys.Keys)
        {
            Console.WriteLine($" - {key.DisplayName} ({key.KeyId})");
        }

        Console.WriteLine();

        var request = new RunProcessRequest
        {
            ProcessProfileName = options.ProcessProfileName,
            ContactsCsvPath = options.ContactsCsvPath,
            SuppressionCsvPath = options.SuppressionCsvPath,
            OutputFolder = options.OutputFolder,
            ExportOptions = new ExportOptions
            {
                Format = options.ExportFormat,
                IncludeHeaders = true,
                QuoteAllFields = true
            },
            ProgressObserver = new DelegatingProcessProgressObserver((update, _) =>
            {
                Console.WriteLine(
                    $"[Progress] {update.StageIndex}/{update.TotalStages} | {update.StageName} | {update.StageStatus} | {update.Message}");
                return Task.CompletedTask;
            })
        };

        var response = await service.RunAsync(request, cancellationToken);

        Console.WriteLine();
        Console.WriteLine("Service response summary:");
        Console.WriteLine($"Process profile: {response.ProcessProfileName}");
        Console.WriteLine($"Success        : {response.Success}");
        Console.WriteLine($"Status         : {response.Status}");
        Console.WriteLine();

        Console.WriteLine("Diagnostic summary:");
        Console.WriteLine($" - Suppression internal duplicate count: {response.Diagnostics.SuppressionInternalDuplicateCount}");
        Console.WriteLine($" - Contact internal duplicate count    : {response.Diagnostics.ContactInternalDuplicateCount}");
        Console.WriteLine($" - Suppression overlap count           : {response.Diagnostics.SuppressionOverlapCount}");
        Console.WriteLine($" - Site dedupe count                   : {response.Diagnostics.SiteDedupCount}");
        Console.WriteLine();

        Console.WriteLine("Output files:");
        foreach (var file in response.OutputFiles)
        {
            Console.WriteLine($" - {file.Name}: {file.Path}");
        }

        Console.WriteLine();
        Console.WriteLine("Run metrics:");
        Console.WriteLine(response.Metrics.ToDisplayText());

        var primaryOutput = response.OutputFiles.FirstOrDefault(f => f.Name == "Output2");
        if (primaryOutput is not null)
        {
            Console.WriteLine();
            Console.WriteLine("Preview of first output file:");
            var preview = await previewService.PreviewFileAsync(
                new PreviewFileRequest
                {
                    FilePath = primaryOutput.Path,
                    Format = options.ExportFormat,
                    HasHeaderRow = true,
                    MaxRows = 5
                },
                cancellationToken);

            Console.WriteLine(string.Join(" | ", preview.Columns));

            foreach (var row in preview.Rows)
            {
                Console.WriteLine(string.Join(" | ", preview.Columns.Select(c =>
                    row.Values.TryGetValue(c, out var value) ? value : null)));
            }
        }
    }
}