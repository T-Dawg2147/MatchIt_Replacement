using System.Text;

namespace MatchIt.MatchReplacement.Core.Reporting;

public sealed class RunMetrics
{
    public int SuppressionInputCount { get; set; }
    public int SuppressionPotentialDuplicates { get; set; }

    public int ContactInputCount { get; set; }
    public int ContactPotentialDuplicates { get; set; }

    public int SuppressionOverlapMatches { get; set; }
    public int ContactsAfterSuppressionRemoval { get; set; }

    public int SiteLevelPotentialDuplicates { get; set; }
    public int SiteLevelOutputCount { get; set; }

    public string ToDisplayText()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"SuppressionInputCount           : {SuppressionInputCount}");
        sb.AppendLine($"SuppressionPotentialDuplicates  : {SuppressionPotentialDuplicates}");
        sb.AppendLine($"ContactInputCount               : {ContactInputCount}");
        sb.AppendLine($"ContactPotentialDuplicates      : {ContactPotentialDuplicates}");
        sb.AppendLine($"SuppressionOverlapMatches       : {SuppressionOverlapMatches}");
        sb.AppendLine($"ContactsAfterSuppressionRemoval : {ContactsAfterSuppressionRemoval}");
        sb.AppendLine($"SiteLevelPotentialDuplicates    : {SiteLevelPotentialDuplicates}");
        sb.AppendLine($"SiteLevelOutputCount            : {SiteLevelOutputCount}");
        return sb.ToString();
    }
}