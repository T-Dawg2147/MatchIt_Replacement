namespace MatchIt.MatchReplacement.Core.Keys;

public sealed class DefaultBuiltInKeyCatalog : IBuiltInKeyCatalog
{
    private static readonly IReadOnlyList<BuiltInKeyDefinition> Keys = new[]
    {
        new BuiltInKeyDefinition
        {
            KeyId = "normalized-postcode",
            DisplayName = "Normalized Postcode",
            Description = "Normalized full postcode value.",
            SourceFields = new[] { "Postcode" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "outward-postcode",
            DisplayName = "Outward Postcode",
            Description = "Outward segment of the postcode.",
            SourceFields = new[] { "Postcode" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "surname-phonetic",
            DisplayName = "Surname Phonetic",
            Description = "Phonetic encoding of surname.",
            SourceFields = new[] { "Surname" },
            SupportsContacts = true,
            SupportsSuppressions = false
        },
        new BuiltInKeyDefinition
        {
            KeyId = "company-first-word-phonetic",
            DisplayName = "Company First Word Phonetic",
            Description = "Phonetic encoding of the first business-name token.",
            SourceFields = new[] { "Company" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "street-phonetic",
            DisplayName = "Street Phonetic",
            Description = "Phonetic encoding of the normalized street/address text.",
            SourceFields = new[] { "Address1", "Address2", "Address3", "Address4", "Address5" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "address-phonetic-key",
            DisplayName = "Address Phonetic Key",
            Description = "Composite phonetic address key using locality and street.",
            SourceFields = new[] { "Address1", "Address2", "Address3", "Address4", "Address5", "Postcode" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "add-key",
            DisplayName = "ADD_KEY",
            Description = "Compact normalized address key with postcode component.",
            SourceFields = new[] { "Address1", "Address2", "Address3", "Address4", "Address5", "Postcode" },
            SupportsContacts = true,
            SupportsSuppressions = true
        },
        new BuiltInKeyDefinition
        {
            KeyId = "normalized-company",
            DisplayName = "Normalized Company",
            Description = "Normalized business name with legal suffix reduction.",
            SourceFields = new[] { "Company" },
            SupportsContacts = true,
            SupportsSuppressions = true
        }
    };

    public IReadOnlyList<BuiltInKeyDefinition> GetAll() => Keys;
}