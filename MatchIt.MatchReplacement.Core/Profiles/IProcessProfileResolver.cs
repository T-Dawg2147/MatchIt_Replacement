namespace MatchIt.MatchReplacement.Core.Profiles;

public interface IProcessProfileResolver
{
    IProcessProfile Resolve(string profileName);
    IReadOnlyList<IProcessProfile> GetAll();
}