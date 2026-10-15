namespace MatchIt.MatchReplacement.Core.Models;

public sealed class ActiveSiteRecord
{
    public Guid InternalId { get; init; } = Guid.NewGuid();

    public string? AccNo { get; set; }
    public string? Company { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? Address5 { get; set; }
    public string? Postcode { get; set; }

    public string? NormalizedCompany { get; set; }
    public string? NormalizedStreet { get; set; }
    public string? NormalizedPostcode { get; set; }
    public string? OutwardPostcode { get; set; }
    public string? StreetPhonetic { get; set; }
    public string? AddressPhoneticKey { get; set; }
    public string? AddKey { get; set; }
}