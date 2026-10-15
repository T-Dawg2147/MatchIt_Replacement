namespace MatchIt.MatchReplacement.Core.Application;

public interface IMatchReplacementQueryService
{
    Task<GetProfilesResponse> GetProfilesAsync(CancellationToken cancellationToken = default);

    Task<GetProfileResponse> GetProfileAsync(
        string profileName,
        CancellationToken cancellationToken = default);
}