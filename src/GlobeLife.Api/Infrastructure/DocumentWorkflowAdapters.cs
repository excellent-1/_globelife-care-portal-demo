using GlobeLife.Api.Application;
using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Infrastructure;

public sealed class LocalObjectStoragePort : ObjectStoragePort
{
    public Task<string> CreateUploadUrlAsync(string objectKey, string contentType, CancellationToken cancellationToken)
        => Task.FromResult($"/api/documents/uploads/{Uri.EscapeDataString(objectKey)}?contentType={Uri.EscapeDataString(contentType)}");
}

public sealed class LocalWorkflowCommandBus(ILogger<LocalWorkflowCommandBus> logger) : WorkflowCommandBus
{
    public Task EnqueueAsync(DocumentSubmission submission, CancellationToken cancellationToken)
    {
        logger.LogInformation("Document workflow queued: {DocumentId} for {CustomerId} with object {ObjectKey}", submission.DocumentId, submission.CustomerId, submission.ObjectKey);
        return Task.CompletedTask;
    }
}

// Replace these adapters with AWS SDK implementations in the production module:
// S3ObjectStoragePort, SqsWorkflowCommandBus, and StepFunctionsWorkflowOrchestrator.
public sealed record AwsDocumentWorkflowPorts(string S3BucketName, string SqsQueueUrl, string StepFunctionsStateMachineArn);
