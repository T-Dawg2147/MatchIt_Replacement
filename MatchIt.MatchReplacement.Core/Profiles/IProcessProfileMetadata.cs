using MatchIt.MatchReplacement.Core.Application;

namespace MatchIt.MatchReplacement.Core.Profiles;

public interface IProcessProfileMetadata
{
    ProcessProfileDescriptor GetDescriptor();
}