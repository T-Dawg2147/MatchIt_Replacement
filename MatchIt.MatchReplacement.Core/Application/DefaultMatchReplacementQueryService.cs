using MatchIt.MatchReplacement.Core.Profiles;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DefaultMatchReplacementQueryService : IMatchReplacementQueryService
{
    private readonly IProcessProfileResolver _profileResolver;

    public DefaultMatchReplacementQueryService(IProcessProfileResolver? profileResolver = null)
    {
        _profileResolver = profileResolver ?? new DefaultProcessProfileResolver();
    }

    public Task<GetProfilesResponse> GetProfilesAsync(CancellationToken cancellationToken = default)
    {
        var profiles = _profileResolver
            .GetAll()
            .OfType<IProcessProfileMetadata>()
            .Select(p => p.GetDescriptor())
            .OrderBy(p => p.DisplayName)
            .ToList();

        return Task.FromResult(new GetProfilesResponse
        {
            Profiles = profiles
        });
    }

    public Task<GetProfileResponse> GetProfileAsync(
        string profileName,
        CancellationToken cancellationToken = default)
    {
        var profile = _profileResolver.Resolve(profileName);

        if (profile is not IProcessProfileMetadata metadata)
        {
            throw new InvalidOperationException(
                $"Profile '{profileName}' does not expose metadata.");
        }

        return Task.FromResult(new GetProfileResponse
        {
            Profile = metadata.GetDescriptor()
        });
    }
}