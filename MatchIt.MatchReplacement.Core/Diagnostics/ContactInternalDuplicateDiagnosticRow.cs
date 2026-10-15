namespace MatchIt.MatchReplacement.Core.Diagnostics;

public sealed class ContactInternalDuplicateDiagnosticRow
{
    public string? LeftCid { get; init; }
    public string? LeftAccNo { get; init; }
    public string? LeftForenames { get; init; }
    public string? LeftSurname { get; init; }
    public string? LeftCompany { get; init; }
    public string? LeftPostcode { get; init; }

    public string? RightCid { get; init; }
    public string? RightAccNo { get; init; }
    public string? RightForenames { get; init; }
    public string? RightSurname { get; init; }
    public string? RightCompany { get; init; }
    public string? RightPostcode { get; set; }

    public decimal Score { get; init; }
    public string Reason { get; init; } = string.Empty;
}