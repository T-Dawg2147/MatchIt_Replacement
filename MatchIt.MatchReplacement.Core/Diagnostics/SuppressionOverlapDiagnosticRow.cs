namespace MatchIt.MatchReplacement.Core.Diagnostics;

public sealed class SuppressionOverlapDiagnosticRow
{
    public string? ContactCid { get; init; }
    public string? ContactAccNo { get; init; }
    public string? ContactCompany { get; init; }
    public string? ContactPostcode { get; init; }

    public string? SuppressionOther1 { get; init; }
    public string? SuppressionCompany { get; init; }
    public string? SuppressionPostcode { get; init; }

    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}