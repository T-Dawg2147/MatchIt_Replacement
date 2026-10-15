namespace MatchIt.MatchReplacement.Core.Models;

public sealed class ContactRecord
{
    public Guid InternalId { get; init; } = Guid.NewGuid();

    public string? Cid { get; set; }
    public string? AccNo { get; set; }
    public string? Office { get; set; }
    public string? Prefix { get; set; }
    public string? Forenames { get; set; }
    public string? Surname { get; set; }
    public string? JobTitle { get; set; }
    public string? Dept { get; set; }
    public string? Company { get; set; }
    public string? Address1 { get; set; }
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string? Address4 { get; set; }
    public string? Address5 { get; set; }
    public string? Postcode { get; set; }
    public string? Email { get; set; }
    public string? CCreated { get; set; }
    public string? LastTypeCr { get; set; }
    public string? LastDateCr { get; set; }
    public string? CRecency { get; set; }
    public string? Bdm { get; set; }
    public string? Hierarchy { get; set; }
    public string? RollupLoy { get; set; }
    public string? RollupAcc { get; set; }
    public string? ActType { get; set; }
    public string? AccSupp { get; set; }
    public string? Rem { get; set; }

    public string? NormalizedCompany { get; set; }
    public string? NormalizedStreet { get; set; }
    public string? NormalizedPostcode { get; set; }
    public string? OutwardPostcode { get; set; }
    public string? SurnamePhonetic { get; set; }
    public string? CompanyFirstWordPhonetic { get; set; }
    public string? StreetPhonetic { get; set; }
    public string? AddressPhoneticKey { get; set; }
    public string? AddKey { get; set; }

    public bool IsDuplicateCandidate { get; set; }
    public bool IsSuppressedOverlap { get; set; }
    public bool IsRemoved { get; set; }
}