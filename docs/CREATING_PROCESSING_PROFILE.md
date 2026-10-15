# Creating a Processing Profile

This guide describes how to add a profile that the Host and WPF applications can discover and run. Profiles are compiled into Core; the repository does not currently provide a profile-file format, runtime profile editor, or profile persistence API.

## How a profile is used

`MatchReplacementService` resolves the profile name using `IProcessProfileResolver`, reads the profile's `MatchConfiguration`, and executes the `ProcessDefinition` returned by `CreateDefinition()`. Both the Host and WPF profile lists come from the resolver's registered profiles. A descriptor returned by `IProcessProfileMetadata` supplies display information, not executable configuration.

## 1. Add a profile class

Create a class under `MatchIt.MatchReplacement.Core/Profiles/`. Implement `IProcessProfile` and, for discovery/metadata, `IProcessProfileMetadata`. `MonthlyNumbersProfile` is the current reference implementation.

```csharp
using MatchIt.MatchReplacement.Core.Application;
using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Profiles;
using MatchIt.MatchReplacement.Core.Stages;

namespace MatchIt.MatchReplacement.Core.Profiles;

public sealed class QuarterlyReviewProfile : IProcessProfile, IProcessProfileMetadata
{
    public string Name => "Quarterly Review";

    public MatchConfiguration CreateConfiguration() => new()
    {
        Thresholds = new MatchingThresholds
        {
            SuppressionOverlapThreshold = 80m,
            StrongSuppressionOverlapThreshold = 92m,
            SiteDedupThreshold = 75m,
            ContactDedupThreshold = 70m,
            SuppressionInternalDedupThreshold = 75m
        },
        Weights = new ScoringWeights(),
        RequirePostcodeFamilyForSuppressionOverlap = true,
        PreferExactPostcodeMatches = true
    };

    public ProcessDefinition CreateDefinition() => new()
    {
        Name = Name,
        Stages = new IProcessStage[]
        {
            new LoadInputsStage(),
            new NormalizeRecordsStage(),
            new SuppressionInternalAnalysisStage(),
            new ContactInternalAnalysisStage(),
            new SuppressionOverlapPurgeStage(),
            new SiteLevelDedupeStage(),
            new ExportOutputsStage(),
            new ExportDiagnosticsStage(),
            new PrintSummaryStage()
        }
    };

    public ProcessProfileDescriptor GetDescriptor() => new()
    {
        Name = Name,
        DisplayName = "Quarterly Review",
        Description = "Quarterly contact cleaning with stricter matching thresholds.",
        Inputs = new[]
        {
            new ProfileInputDefinition
            {
                Key = "contactsCsvPath",
                DisplayName = "Contacts File",
                Description = "Pipe-delimited contact input with no header row.",
                Required = true
            },
            new ProfileInputDefinition
            {
                Key = "suppressionCsvPath",
                DisplayName = "Suppression File",
                Description = "Pipe-delimited suppression input with no header row.",
                Required = true
            }
        },
        Outputs = new[]
        {
            new ProfileOutputDefinition
            {
                Key = "Output2",
                DisplayName = "Contacts after suppression removal",
                DefaultFileName = "2 LAST DATE CR.csv",
                Description = "Contacts remaining after suppression overlaps are removed."
            },
            new ProfileOutputDefinition
            {
                Key = "Output3",
                DisplayName = "Active site-level contacts",
                DefaultFileName = "3 SITE LEVEL DATA ACTIVE.csv",
                Description = "Contacts remaining after site-level deduplication."
            }
        },
        SupportedExportFormats = new[]
        {
            ExportFormat.Csv,
            ExportFormat.PipeDelimited,
            ExportFormat.Json
        }
    };
}
```

The example uses the existing built-in stages and output stems. Its file names are metadata examples; the actual built-in export stage determines file stems and extensions.

## 2. Choose thresholds and weights

`MatchingThresholds` defines separate cutoffs for suppression overlap, strong suppression overlap, site dedupe, contact internal dedupe, and suppression internal dedupe. `ScoringWeights` defines the evidence weights used by the matchers, including postcode, address, company, email, and phonetic combinations. If an omitted property should use existing defaults, construct `new ScoringWeights()` or `new MatchingThresholds()` and override only the values that should differ.

Treat weights and thresholds together: a threshold change can make the same evidence produce a different set of matches. Run the profile against representative data and inspect diagnostic outputs before using it operationally.

`RequirePostcodeFamilyForSuppressionOverlap` controls whether overlap candidates must share an exact or outward postcode family (default `true`). `PreferExactPostcodeMatches` is also part of configuration and defaults to `true`.

## 3. Define the executable stages

Return the stages in the order required by the data they consume. The built-in pipeline order is shown above. In particular, loading must precede normalization; matching analysis must run after normalization; suppression purge precedes site dedupe; and exports must run after their source data and diagnostics have been generated.

The stage list is executable behavior. Omitting `ExportOutputsStage` or `ExportDiagnosticsStage` means those output files are not generated. The current built-in stages use the settings on `ProcessRunContext`, including its paths, configuration, export options, metrics, records, and named-output collection.

If a new business operation is required, implement `IProcessStage` and add it to the profile's list. Do not assume that editable-stage metadata adds, removes, or reorders runtime stages.

## 4. Describe the profile

`GetDescriptor()` should provide a unique `Name` matching the executable profile name, a user-facing `DisplayName`, and a clear description. List input definitions, outputs, and supported formats accurately.

The current default authoring query service asks the resolved profile for its descriptor but supplies a fixed list of default stage definitions and the built-in key catalog. It initializes custom keys as empty. This DTO is not used by the pipeline to construct stages or to select matcher behavior. The built-in export stage also uses fixed logical names and stems, so changing `ProfileOutputDefinition.DefaultFileName` alone does not rename runtime files.

The two current inputs should be described as pipe-delimited files without a header row. The Core reader maps columns by fixed position (27 contact fields and 10 suppression fields); it does not map by descriptor keys.

## 5. Register the profile

Add the profile to the profile collection in `DefaultProcessProfileResolver`:

```csharp
_profiles = new IProcessProfile[]
{
    new MonthlyNumbersProfile(),
    new QuarterlyReviewProfile()
};
```

The resolver matches profile names case-insensitively. A unique, stable name is required. Registration makes the profile available to `DefaultMatchReplacementQueryService` (and therefore the WPF selector) and to calls through `MatchReplacementService`.

## 6. Run it

For the console Host, pass the profile name as the fourth argument:

```text
MatchIt.MatchReplacement.Host <contacts.psv> <suppressions.psv> <output-folder> "Quarterly Review" Csv
```

The profile argument is optional only because Host defaults it to `"Monthly Numbers"`. The format argument is also optional; valid values are `Csv`, `PipeDelimited`, and `Json`.

In WPF, the newly registered profile should appear in the profile selector after startup. Select it, choose both input files and an output folder, select a format, then run the process. WPF currently supports profile selection only; it does not edit or persist profile definitions.

## 7. Verify the profile

Before relying on the profile:

1. Build the Core project and the Host/WPF projects that consume it.
2. Confirm the new name appears in the profile query results and, when appropriate, the WPF profile list.
3. Run representative contacts and suppression files through the profile.
4. Review progress, output file names, metrics, and all diagnostic outputs.
5. Confirm the selected export format and output headers/quoting are appropriate for downstream use.
6. Check behavior when input files are missing, rows have short/malformed data, and output files already exist.

The service verifies that input paths exist and creates the output directory, but it does not roll back output files if a later stage fails.

## Relevant implementation files

- `MatchIt.MatchReplacement.Core/Profiles/IProcessProfile.cs` — executable profile contract.
- `MatchIt.MatchReplacement.Core/Profiles/IProcessProfileMetadata.cs` — profile descriptor contract.
- `MatchIt.MatchReplacement.Core/Profiles/MonthlyNumbersProfile.cs` — built-in profile example.
- `MatchIt.MatchReplacement.Core/Profiles/DefaultProcessProfileResolver.cs` — profile registration and name lookup.
- `MatchIt.MatchReplacement.Core/Application/MatchReplacementService.cs` — resolves and executes the profile.
- `MatchIt.MatchReplacement.Core/Application/DefaultProfileAuthoringQueryService.cs` — read-only authoring metadata response.
- `MatchIt.MatchReplacement.Core/Engine/ProcessDefinition.cs` and `ProcessEngine.cs` — stage pipeline definition and execution.
- `MatchIt.MatchReplacement.Core/Config/MatchConfiguration.cs`, `MatchingThresholds.cs`, and `ScoringWeights.cs` — matching behavior configuration.
