using MatchIt.MatchReplacement.Core.Application;
using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Engine;
using MatchIt.MatchReplacement.Core.Stages;

namespace MatchIt.MatchReplacement.Core.Profiles;

public sealed class MonthlyNumbersProfile : IProcessProfile, IProcessProfileMetadata
{
    public string Name => "Monthly Numbers";

    public MatchConfiguration CreateConfiguration()
    {
        return new MatchConfiguration
        {
            Thresholds = new MatchingThresholds
            {
                SuppressionOverlapThreshold = 75m,
                StrongSuppressionOverlapThreshold = 90m,
                SiteDedupThreshold = 70m,
                ContactDedupThreshold = 65m,
                SuppressionInternalDedupThreshold = 70m
            },
            Weights = new ScoringWeights(),
            RequirePostcodeFamilyForSuppressionOverlap = true,
            PreferExactPostcodeMatches = true
        };
    }

    public ProcessDefinition CreateDefinition()
    {
        return new ProcessDefinition
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
    }

    public ProcessProfileDescriptor GetDescriptor()
    {
        return new ProcessProfileDescriptor
        {
            Name = Name,
            DisplayName = "Monthly Numbers",
            Description =
                "Imports suppression and contacts files, performs internal duplicate analysis, suppression overlap removal, site-level dedupe, and exports diagnostics and output files.",
            Inputs = new[]
            {
                new ProfileInputDefinition
                {
                    Key = "contactsCsvPath",
                    DisplayName = "Contacts File",
                    Description = "The input contacts file for the process.",
                    Required = true
                },
                new ProfileInputDefinition
                {
                    Key = "suppressionCsvPath",
                    DisplayName = "Suppression File",
                    Description = "The suppression input file for the process.",
                    Required = true
                }
            },
            Outputs = new[]
            {
                new ProfileOutputDefinition
                {
                    Key = "Output2",
                    DisplayName = "2 LAST DATE CR",
                    DefaultFileName = "2 LAST DATE CR.csv",
                    Description = "Contacts after suppression overlap removal."
                },
                new ProfileOutputDefinition
                {
                    Key = "Output3",
                    DisplayName = "3 SITE LEVEL DATA ACTIVE",
                    DefaultFileName = "3 SITE LEVEL DATA ACTIVE.csv",
                    Description = "Site-level active output after dedupe."
                },
                new ProfileOutputDefinition
                {
                    Key = "DiagSuppressionInternal",
                    DisplayName = "Suppression Internal Diagnostics",
                    DefaultFileName = "diag.suppression.internal.duplicates.csv",
                    Description = "Detailed suppression internal duplicate diagnostics."
                },
                new ProfileOutputDefinition
                {
                    Key = "DiagContactInternal",
                    DisplayName = "Contact Internal Diagnostics",
                    DefaultFileName = "diag.contact.internal.duplicates.csv",
                    Description = "Detailed contact internal duplicate diagnostics."
                },
                new ProfileOutputDefinition
                {
                    Key = "DiagSuppressionOverlap",
                    DisplayName = "Suppression Overlap Diagnostics",
                    DefaultFileName = "diag.suppression.overlaps.csv",
                    Description = "Detailed suppression overlap diagnostics."
                },
                new ProfileOutputDefinition
                {
                    Key = "DiagSiteDedup",
                    DisplayName = "Site Dedupe Diagnostics",
                    DefaultFileName = "diag.site.dedup.csv",
                    Description = "Detailed site-level dedupe diagnostics."
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
}