namespace MatchIt.MatchReplacement.Core.Keys;

public interface IBuiltInKeyCatalog
{
    IReadOnlyList<BuiltInKeyDefinition> GetAll();
}