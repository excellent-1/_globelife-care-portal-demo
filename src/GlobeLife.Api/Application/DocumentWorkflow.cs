using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Application;

public interface IDocumentWorkflowOrchestrator
{
    Task<WorkflowStatus> SubmitAsync(DocumentSubmission submission, CancellationToken cancellationToken);
}

public interface ObjectStoragePort
{
    Task<string> CreateUploadUrlAsync(string objectKey, string contentType, CancellationToken cancellationToken);
}

public interface WorkflowCommandBus
{
    Task EnqueueAsync(DocumentSubmission submission, CancellationToken cancellationToken);
}

public sealed class DocumentWorkflowOrchestrator(ObjectStoragePort objectStorage, WorkflowCommandBus commandBus) : IDocumentWorkflowOrchestrator
{
    public async Task<WorkflowStatus> SubmitAsync(DocumentSubmission submission, CancellationToken cancellationToken)
    {
        var uploadUrl = await objectStorage.CreateUploadUrlAsync(submission.ObjectKey, "application/octet-stream", cancellationToken);
        await commandBus.EnqueueAsync(submission, cancellationToken);
        return new WorkflowStatus(submission.DocumentId, $"Queued:{uploadUrl}", 0, DateTimeOffset.UtcNow);
    }
}
