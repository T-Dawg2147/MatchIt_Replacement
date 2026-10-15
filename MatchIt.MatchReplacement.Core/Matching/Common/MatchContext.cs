using MatchIt.MatchReplacement.Core.Config;

namespace MatchIt.MatchReplacement.Core.Matching.Common;

public sealed class MatchContext
{
    public MatchContext(MatchConfiguration configuration)
    {
        Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public MatchConfiguration Configuration { get; }
}