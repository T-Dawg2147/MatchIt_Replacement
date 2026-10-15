namespace MatchIt.MatchReplacement.Core.Profiles;

public sealed class DefaultProcessProfileResolver : IProcessProfileResolver
{
    private readonly IReadOnlyList<IProcessProfile> _profiles;

    public DefaultProcessProfileResolver()
    {
        _profiles = new IProcessProfile[]
        {
            new MonthlyNumbersProfile()
        };
    }

    public IProcessProfile Resolve(string profileName)
    {
        if (string.IsNullOrWhiteSpace(profileName))
        {
            throw new ArgumentException("Profile name must be provided.", nameof(profileName));
        }

        var profile = _profiles.FirstOrDefault(p =>
            string.Equals(p.Name, profileName, StringComparison.OrdinalIgnoreCase));

        if (profile is null)
        {
            var available = string.Join(", ", _profiles.Select(p => p.Name));
            throw new InvalidOperationException(
                $"Unknown process profile '{profileName}'. Available profiles: {available}");
        }

        return profile;
    }

    public IReadOnlyList<IProcessProfile> GetAll() => _profiles;
}