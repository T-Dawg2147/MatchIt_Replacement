using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Profiles;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class MatchReplacementService : IMatchReplacementService
{
    private readonly IProcessProfileResolver _profileResolver;
    private readonly ProcessEngine _engine;

    public MatchReplacementService(
        IProcessProfileResolver? profileResolver = null,
        ProcessEngine? engine = null)
    {
        _profileResolver = profileResolver ?? new DefaultProcessProfileResolver();
        _engine = engine ?? new ProcessEngine();
    }

    public async Task<RunProcessResponse> RunAsync(
        RunProcessRequest request,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateRequest(request);

        var profile = _profileResolver.Resolve(request.ProcessProfileName);
        var context = new ProcessRunContext
        {
            Configuration = profile.CreateConfiguration(),
            ContactsCsvPath = Path.GetFullPath(request.ContactsCsvPath),
            SuppressionCsvPath = Path.GetFullPath(request.SuppressionCsvPath),
            OutputFolder = Path.GetFullPath(request.OutputFolder),
            ExportOptions = request.ExportOptions,
            ProgressObserver = request.ProgressObserver,
            ProcessProfileName = profile.Name
        };

        Directory.CreateDirectory(context.OutputFolder);

        var definition = profile.CreateDefinition();
        await _engine.RunAsync(definition, context, cancellationToken);

        return BuildResponse(profile.Name, context);
    }

    private static void ValidateRequest(RunProcessRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.ProcessProfileName))
        {
            throw new ArgumentException("Process profile name is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.ContactsCsvPath))
        {
            throw new ArgumentException("Contacts CSV path is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.SuppressionCsvPath))
        {
            throw new ArgumentException("Suppression CSV path is required.", nameof(request));
        }

        if (string.IsNullOrWhiteSpace(request.OutputFolder))
        {
            throw new ArgumentException("Output folder is required.", nameof(request));
        }

        if (!File.Exists(request.ContactsCsvPath))
        {
            throw new FileNotFoundException("Contacts CSV not found.", request.ContactsCsvPath);
        }

        if (!File.Exists(request.SuppressionCsvPath))
        {
            throw new FileNotFoundException("Suppression CSV not found.", request.SuppressionCsvPath);
        }
    }

    private static RunProcessResponse BuildResponse(string profileName, ProcessRunContext context)
    {
        var outputFiles = context.NamedOutputs
            .OrderBy(kvp => kvp.Key)
            .Select(kvp => new ProcessOutputFile
            {
                Name = kvp.Key,
                Path = kvp.Value
            })
            .ToList();

        return new RunProcessResponse
        {
            ProcessProfileName = profileName,
            Success = true,
            Status = ProcessRunStatus.Completed,
            Metrics = context.Metrics,
            Diagnostics = new DiagnosticSummary
            {
                SuppressionInternalDuplicateCount = context.SuppressionInternalDuplicateDiagnostics.Count,
                ContactInternalDuplicateCount = context.ContactInternalDuplicateDiagnostics.Count,
                SuppressionOverlapCount = context.SuppressionOverlapDiagnostics.Count,
                SiteDedupCount = context.SiteDedupDiagnostics.Count
            },
            OutputFiles = outputFiles
        };
    }
}