namespace GlobeLife.Api.Domain;

public sealed record PolicyAuditEvent(string EventId, string PolicyNumber, string EventType, DateTimeOffset OccurredUtc, string CorrelationId);
public sealed record DocumentSubmission(string DocumentId, string CustomerId, string ObjectKey, string Status, DateTimeOffset SubmittedUtc);
public sealed record WorkflowStatus(string WorkflowId, string Status, int AttemptCount, DateTimeOffset UpdatedUtc);
public enum PolicyLifecycleStatus { Active, Pending, Lapsed, Cancelled }
