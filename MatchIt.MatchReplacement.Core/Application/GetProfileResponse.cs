namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetProfileResponse
{
    public ProcessProfileDescriptor Profile { get; init; } = new();
}