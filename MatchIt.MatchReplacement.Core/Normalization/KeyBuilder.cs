using MatchIt.MatchReplacement.Core.Models;

namespace MatchIt.MatchReplacement.Core.Normalization;

public sealed class KeyBuilder
{
    private readonly TextNormalizer _textNormalizer;
    private readonly PostcodeNormalizer _postcodeNormalizer;
    private readonly CompanyNormalizer _companyNormalizer;
    private readonly AddressNormalizer _addressNormalizer;
    private readonly PhoneticEncoder _phoneticEncoder;

    public KeyBuilder(
        TextNormalizer textNormalizer,
        PostcodeNormalizer postcodeNormalizer,
        CompanyNormalizer companyNormalizer,
        AddressNormalizer addressNormalizer,
        PhoneticEncoder phoneticEncoder)
    {
        _textNormalizer = textNormalizer;
        _postcodeNormalizer = postcodeNormalizer;
        _companyNormalizer = companyNormalizer;
        _addressNormalizer = addressNormalizer;
        _phoneticEncoder = phoneticEncoder;
    }

    public void Enrich(ContactRecord record)
    {
        record.NormalizedCompany = _companyNormalizer.NormalizeCompany(record.Company);
        record.NormalizedStreet = _addressNormalizer.NormalizeStreet(
            record.Address1, record.Address2, record.Address3, record.Address4, record.Address5);
        record.NormalizedPostcode = _postcodeNormalizer.Normalize(record.Postcode);
        record.OutwardPostcode = _postcodeNormalizer.ExtractOutward(record.Postcode);
        record.SurnamePhonetic = _phoneticEncoder.Encode(record.Surname);
        record.CompanyFirstWordPhonetic = _phoneticEncoder.Encode(
            _companyNormalizer.ExtractFirstWord(record.Company));
        record.StreetPhonetic = _phoneticEncoder.Encode(record.NormalizedStreet);
        record.AddressPhoneticKey = BuildAddressPhoneticKey(
            record.Address4,
            record.Address5,
            record.NormalizedStreet);
        record.AddKey = BuildAddKey(
            record.Address1,
            record.Address2,
            record.Address3,
            record.Address4,
            record.Address5,
            record.Postcode);
    }

    public void Enrich(SuppressionRecord record)
    {
        record.NormalizedCompany = _companyNormalizer.NormalizeCompany(record.Company);
        record.NormalizedStreet = _addressNormalizer.NormalizeStreet(
            record.Address1, record.Address2, record.Address3, record.Address4, record.Address5);
        record.NormalizedPostcode = _postcodeNormalizer.Normalize(record.Postcode);
        record.OutwardPostcode = _postcodeNormalizer.ExtractOutward(record.Postcode);
        record.CompanyFirstWordPhonetic = _phoneticEncoder.Encode(
            _companyNormalizer.ExtractFirstWord(record.Company));
        record.StreetPhonetic = _phoneticEncoder.Encode(record.NormalizedStreet);
        record.AddressPhoneticKey = BuildAddressPhoneticKey(
            record.Address4,
            record.Address5,
            record.NormalizedStreet);
        record.AddKey = BuildAddKey(
            record.Address1,
            record.Address2,
            record.Address3,
            record.Address4,
            record.Address5,
            record.Postcode);
    }

    private string BuildAddressPhoneticKey(string? town, string? city, string? normalizedStreet)
    {
        var locality = _textNormalizer.NormalizeForComparison($"{town} {city}");
        var localityPhonetic = _phoneticEncoder.Encode(locality);
        var streetPhonetic = _phoneticEncoder.Encode(normalizedStreet);

        return $"{localityPhonetic}|{streetPhonetic}";
    }

    private string BuildAddKey(
        string? address1,
        string? address2,
        string? address3,
        string? address4,
        string? address5,
        string? postcode)
    {
        var normalizedAddress = _addressNormalizer.BuildFullAddress(
            address1, address2, address3, address4, address5);

        var normalizedPostcode = _postcodeNormalizer.Normalize(postcode).Replace(" ", string.Empty);

        var compactAddress = normalizedAddress.Replace(" ", string.Empty);

        if (compactAddress.Length > 24)
        {
            compactAddress = compactAddress[..24];
        }

        return $"{compactAddress}|{normalizedPostcode}";
    }
}