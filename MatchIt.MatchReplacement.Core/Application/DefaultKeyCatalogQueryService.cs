using MatchIt.MatchReplacement.Core.Keys;

namespace MatchIt.MatchReplacement.Core.Application;

public sealed class DefaultKeyCatalogQueryService : IKeyCatalogQueryService
{
    private readonly IBuiltInKeyCatalog _catalog;

    public DefaultKeyCatalogQueryService(IBuiltInKeyCatalog? catalog = null)
    {
        _catalog = catalog ?? new DefaultBuiltInKeyCatalog();
    }

    public Task<GetBuiltInKeysResponse> GetBuiltInKeysAsync(
        CancellationToken cancellationToken = default)
    {
        var keys = _catalog
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
            .ToList();

        return Task.FromResult(new GetBuiltInKeysResponse
        {
            Keys = keys
        });
    }
}