namespace MatchIt.MatchReplacement.Core.Diagnostics;

public sealed class SuppressionInternalDuplicateDiagnosticRow
{
    public string? LeftOther1 { get; init; }
    public string? LeftCompany { get; init; }
    public string? LeftPostcode { get; init; }

    public string? RightOther1 { get; init; }
    public string? RightCompany { get; init; }
    public string? RightPostcode { get; init; }

    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}