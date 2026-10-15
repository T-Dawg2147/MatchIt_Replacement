namespace MatchIt.MatchReplacement.Core.Config;

public sealed class ScoringWeights
{
    // Shared/common
    public decimal ExactPostcode { get; init; } = 35m;
    public decimal OutwardPostcode { get; init; } = 15m;
    public decimal AddKey { get; init; } = 30m;
    public decimal NormalizedStreet { get; init; } = 10m;
    public decimal StreetPhonetic { get; init; } = 12m;
    public decimal AddressPhoneticKey { get; init; } = 18m;

    // Company/business
    public decimal NormalizedCompany { get; init; } = 25m;
    public decimal CompanyFirstWordPhonetic { get; init; } = 12m;
    public decimal CompanyAndStreetPhoneticCombo { get; init; } = 35m;
    public decimal OutwardPostcodeAndCompanyPhoneticCombo { get; init; } = 45m;

    // Contact/person
    public decimal ExactEmail { get; init; } = 20m;
    public decimal SurnamePhonetic { get; init; } = 12m;
    public decimal SurnameAndStreetPhoneticCombo { get; init; } = 45m;
    public decimal OutwardPostcodeAndSurnamePhoneticCombo { get; init; } = 45m;
}