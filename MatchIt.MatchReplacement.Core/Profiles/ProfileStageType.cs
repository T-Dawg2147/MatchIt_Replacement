namespace MatchIt.MatchReplacement.Core.Profiles;

public enum ProfileStageType
{
    LoadInputs = 1,
    NormalizeRecords = 2,
    SuppressionInternalAnalysis = 3,
    ContactInternalAnalysis = 4,
    SuppressionOverlapPurge = 5,
    SiteLevelDedupe = 6,
    ExportOutputs = 7,
    ExportDiagnostics = 8,
    PrintSummary = 9
}