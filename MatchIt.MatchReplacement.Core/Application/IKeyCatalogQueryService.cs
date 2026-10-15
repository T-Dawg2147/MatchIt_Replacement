namespace MatchIt.MatchReplacement.Core.Application;

public interface IKeyCatalogQueryService
{
    Task<GetBuiltInKeysResponse> GetBuiltInKeysAsync(
        CancellationToken cancellationToken = default);
}