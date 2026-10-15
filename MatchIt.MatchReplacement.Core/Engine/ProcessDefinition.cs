namespace MatchIt.MatchReplacement.Core.Engine;

public sealed class ProcessDefinition
{
    public required string Name { get; init; }
    public required IReadOnlyList<IProcessStage> Stages { get; init; }
}