using MatchIt.MatchReplacement.Core.Config;
using MatchIt.MatchReplacement.Core.Engine;

namespace MatchIt.MatchReplacement.Core.Profiles;

public interface IProcessProfile
{
    string Name { get; }

    MatchConfiguration CreateConfiguration();

    ProcessDefinition CreateDefinition();
}