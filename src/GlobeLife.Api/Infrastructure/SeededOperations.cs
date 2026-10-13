using GlobeLife.Api.Application;
using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Infrastructure;

public interface IPolicyDetailRepository
{
    Task<PolicyDetail?> GetDetailAsync(string policyNumber, CancellationToken cancellationToken);
}

public sealed class SeededPolicyDetailRepository : IPolicyDetailRepository
{
    public Task<PolicyDetail?> GetDetailAsync(string policyNumber, CancellationToken cancellationToken)
    {
        var detail = policyNumber switch
        {
            "GL-2048-119" => new PolicyDetail("GL-2048-119", "Family Heritage Whole Life", "Active", 250000, new DateOnly(2026, 9, 15), "Redis read-through cache", DateTimeOffset.UtcNow),
            "GL-2048-120" => new PolicyDetail("GL-2048-120", "SecureTerm 20", "Active", 500000, new DateOnly(2027, 2, 1), "Redis read-through cache", DateTimeOffset.UtcNow),
            _ => null
        };
        return Task.FromResult(detail);
    }
}

public sealed class SeededSupportOperations
{
    public IReadOnlyList<object> Cases =>
    [
        new { id = "SUP-1001", priority = "High", subject = "Payment question", status = "Open", workflowId = "WF-1001", attemptCount = 0, correlationId = "corr-support-1001" },
        new { id = "SUP-1002", priority = "High", subject = "Policy document validation failed", status = "Retrying", workflowId = "WF-1002", attemptCount = 2, correlationId = "corr-doc-1002" },
        new { id = "SUP-1003", priority = "Normal", subject = "Address update", status = "Open", workflowId = "WF-1003", attemptCount = 0, correlationId = "corr-support-1003" }
    ];
}
