namespace MatchIt.MatchReplacement.Core.Models;

public sealed class DuplicateGroup<T>
{
    public Guid GroupId { get; init; } = Guid.NewGuid();
    public List<T> Records { get; } = new();
    public string Reason { get; set; } = string.Empty;
}