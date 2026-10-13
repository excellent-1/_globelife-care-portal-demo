namespace GlobeLife.Api.Application;

public sealed record PlatformMetrics(string Environment, string Version, string Commit, string CacheStatus, int CacheHitPercent, int ActiveWorkflows, int CompletedWorkflows, int RetryingWorkflows, int DeadLetterWorkflows, int P95LatencyMilliseconds, string LastCorrelationId);
public sealed record WorkflowTimeline(string WorkflowId, string DocumentId, string Status, int AttemptCount, IReadOnlyList<string> CompletedStages, DateTimeOffset UpdatedUtc);
public sealed record PolicyDetail(string PolicyNumber, string ProductName, string Status, decimal CoverageAmount, DateOnly RenewalDate, string CacheSource, DateTimeOffset RetrievedUtc);
public sealed record DemoControlResult(string Scenario, string Status, string CorrelationId, DateTimeOffset CreatedUtc);

public interface IPlatformTelemetryService
{
    PlatformMetrics GetMetrics(string correlationId);
    WorkflowTimeline GetWorkflow(string workflowId);
    DemoControlResult Simulate(string scenario, string correlationId);
}

public sealed class PlatformTelemetryService : IPlatformTelemetryService
{
    public PlatformMetrics GetMetrics(string correlationId) => new("Production", "1.5.0", "visible-demo-build", "Connected", 82, 3, 42, 1, 0, 184, correlationId);
    public WorkflowTimeline GetWorkflow(string workflowId) => new(workflowId, "DOC-1002", "Retrying", 2, ["Created", "UploadedToObjectStorage", "Queued", "ValidationFailed"], DateTimeOffset.UtcNow);
    public DemoControlResult Simulate(string scenario, string correlationId) => new(scenario, scenario.Contains("dead", StringComparison.OrdinalIgnoreCase) ? "DeadLettered" : "Simulated", correlationId, DateTimeOffset.UtcNow);
}
