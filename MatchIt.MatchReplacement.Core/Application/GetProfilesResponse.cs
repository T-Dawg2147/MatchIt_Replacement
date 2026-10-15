namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetProfilesResponse
{
    public IReadOnlyList<ProcessProfileDescriptor> Profiles { get; init; } =
        Array.Empty<ProcessProfileDescriptor>();
}