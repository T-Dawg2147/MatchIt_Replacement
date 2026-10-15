namespace MatchIt.MatchReplacement.Core.Application;

public interface IProfileAuthoringQueryService
{
    Task<GetEditableProfileResponse> GetEditableProfileAsync(
        string profileName,
        CancellationToken cancellationToken = default);
}