using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Host;

public sealed class RunOptions
{
    public string ProcessProfileName { get; init; } = "Monthly Numbers";
    public string ContactsCsvPath { get; init; } = string.Empty;
    public string SuppressionCsvPath { get; init; } = string.Empty;
    public string OutputFolder { get; init; } = string.Empty;
    public ExportFormat ExportFormat { get; init; } = ExportFormat.Csv;

    public static RunOptions FromArgs(string[] args)
    {
        if (args.Length < 3)
        {
            throw new ArgumentException(
                "Expected arguments: <contactsCsvPath> <suppressionCsvPath> <outputFolder> [processProfileName] [exportFormat]");
        }

        var options = new RunOptions
        {
            ContactsCsvPath = Path.GetFullPath(args[0]),
            SuppressionCsvPath = Path.GetFullPath(args[1]),
            OutputFolder = Path.GetFullPath(args[2]),
            ProcessProfileName = args.Length >= 4 ? args[3] : "Monthly Numbers",
            ExportFormat = args.Length >= 5 && Enum.TryParse<ExportFormat>(args[4], true, out var format)
                ? format
                : ExportFormat.Csv
        };

        Validate(options);
        return options;
    }

    private static void Validate(RunOptions options)
    {
        if (!File.Exists(options.ContactsCsvPath))
        {
            throw new FileNotFoundException("Contacts CSV not found.", options.ContactsCsvPath);
        }

        if (!File.Exists(options.SuppressionCsvPath))
        {
            throw new FileNotFoundException("Suppression CSV not found.", options.SuppressionCsvPath);
        }

        Directory.CreateDirectory(options.OutputFolder);
    }
}