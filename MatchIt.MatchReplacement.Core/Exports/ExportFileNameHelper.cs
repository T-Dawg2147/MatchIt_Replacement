using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Exports;

public static class ExportFileNameHelper
{
    public static string GetExtension(ExportFormat format)
    {
        return format switch
        {
            ExportFormat.Csv => ".csv",
            ExportFormat.PipeDelimited => ".txt",
            ExportFormat.Json => ".json",
            _ => ".dat"
        };
    }

    public static string BuildFileName(string baseNameWithoutExtension, ExportFormat format)
    {
        return $"{baseNameWithoutExtension}{GetExtension(format)}";
    }
}