namespace MatchIt.MatchReplacement.Core.Application;

public interface IResultPreviewService
{
    Task<PreviewFileResponse> PreviewFileAsync(
        PreviewFileRequest request,
        CancellationToken cancellationToken = default);
}