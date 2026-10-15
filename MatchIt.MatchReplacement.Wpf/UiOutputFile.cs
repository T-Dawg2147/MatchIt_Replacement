using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Wpf;

public sealed class UiOutputFile
{
    public string Name { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;

    public string DisplayName => $"{Name} - {Path}";

    public static UiOutputFile From(ProcessOutputFile file)
    {
        return new UiOutputFile
        {
            Name = file.Name,
            Path = file.Path
        };
    }
}