namespace MatchIt.MatchReplacement.Core.Application;

public sealed class GetEditableProfileResponse
{
    public EditableProcessProfileDto Profile { get; init; } = new();
}