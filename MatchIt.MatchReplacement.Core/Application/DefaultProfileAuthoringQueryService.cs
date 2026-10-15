using MatchIt.MatchReplacement.Core.Keys;
using MatchIt.MatchReplacement.Core.Profiles;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DefaultProfileAuthoringQueryService : IProfileAuthoringQueryService
{
    private readonly IProcessProfileResolver _profileResolver;
    private readonly IBuiltInKeyCatalog _keyCatalog;

    public DefaultProfileAuthoringQueryService(
        IProcessProfileResolver? profileResolver = null,
        IBuiltInKeyCatalog? keyCatalog = null)
    {
        _profileResolver = profileResolver ?? new DefaultProcessProfileResolver();
        _keyCatalog = keyCatalog ?? new DefaultBuiltInKeyCatalog();
    }

    public Task<GetEditableProfileResponse> GetEditableProfileAsync(
        string profileName,
        CancellationToken cancellationToken = default)
    {
        var profile = _profileResolver.Resolve(profileName);

        if (profile is not IProcessProfileMetadata metadata)
        {
            throw new InvalidOperationException(
                $"Profile '{profileName}' does not expose metadata.");
        }

        var descriptor = metadata.GetDescriptor();

        var editable = new EditableProcessProfileDto
        {
            Name = descriptor.Name,
            DisplayName = descriptor.DisplayName,
            Description = descriptor.Description,
            Inputs = descriptor.Inputs,
            Outputs = descriptor.Outputs,
            Stages = CreateDefaultStageDefinitions(),
            BuiltInKeys = _keyCatalog
                .GetAll()
                .Select(k => new BuiltInKeyDefinitionDto
                {
                    KeyId = k.KeyId,
                    DisplayName = k.DisplayName,
                    Description = k.Description,
                    SourceFields = k.SourceFields,
                    SupportsContacts = k.SupportsContacts,
                    SupportsSuppressions = k.SupportsSuppressions
                })
                .OrderBy(k => k.DisplayName)
                .ToList(),
            CustomKeys = Array.Empty<CustomKeyDefinitionDto>()
        };

        return Task.FromResult(new GetEditableProfileResponse
        {
            Profile = editable
        });
    }

    private static IReadOnlyList<ProfileStageDefinitionDto> CreateDefaultStageDefinitions()
    {
        return
        [
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.LoadInputs,
                DisplayName = "Load Inputs",
                Enabled = true
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.NormalizeRecords,
                DisplayName = "Normalize Records",
                Enabled = true,
                KeyIds =
                [
                    "normalized-postcode",
                    "outward-postcode",
                    "normalized-company",
                    "company-first-word-phonetic",
                    "street-phonetic",
                    "address-phonetic-key",
                    "add-key"
                ]
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.SuppressionInternalAnalysis,
                DisplayName = "Suppression Internal Analysis",
                Enabled = true,
                KeyIds = 
                [
                    "outward-postcode",
                    "company-first-word-phonetic",
                    "street-phonetic",
                    "normalized-postcode"
                ]
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.ContactInternalAnalysis,
                DisplayName = "Contact Internal Analysis",
                Enabled = true,
                KeyIds =
                [
                    "outward-postcode",
                    "surname-phonetic",
                    "street-phonetic",
                    "normalized-postcode"
                ]
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.SuppressionOverlapPurge,
                DisplayName = "Suppression Overlap Purge",
                Enabled = true,
                KeyIds =
                [
                    "normalized-postcode",
                    "outward-postcode",
                    "normalized-company",
                    "company-first-word-phonetic",
                    "street-phonetic",
                    "address-phonetic-key",
                    "add-key"
                ]
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.SiteLevelDedupe,
                DisplayName = "Site Level Dedupe",
                Enabled = true,
                KeyIds =
                [
                    "normalized-postcode",
                    "outward-postcode",
                    "normalized-company",
                    "company-first-word-phonetic",
                    "street-phonetic",
                    "address-phonetic-key",
                    "add-key"
                ]
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.ExportOutputs,
                DisplayName = "Export Outputs",
                Enabled = true
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.ExportDiagnostics,
                DisplayName = "Export Diagnostics",
                Enabled = true
            },
            new ProfileStageDefinitionDto
            {
                StageType = ProfileStageType.PrintSummary,
                DisplayName = "Print Summary",
                Enabled = true
            }
        ];
    }
}