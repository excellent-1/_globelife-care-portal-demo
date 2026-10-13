using System.Collections.Concurrent;
using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Infrastructure;

public interface IPolicyAuditRepository
{
    Task AppendAsync(PolicyAuditEvent auditEvent, CancellationToken cancellationToken);
    Task<IReadOnlyList<PolicyAuditEvent>> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken);
}

public sealed class InMemoryPolicyAuditRepository : IPolicyAuditRepository
{
    private readonly ConcurrentQueue<PolicyAuditEvent> auditEvents = new();
    public Task AppendAsync(PolicyAuditEvent auditEvent, CancellationToken cancellationToken) { auditEvents.Enqueue(auditEvent); return Task.CompletedTask; }
    public Task<IReadOnlyList<PolicyAuditEvent>> GetByPolicyNumberAsync(string policyNumber, CancellationToken cancellationToken)
        => Task.FromResult<IReadOnlyList<PolicyAuditEvent>>(auditEvents.Where(x => x.PolicyNumber.Equals(policyNumber, StringComparison.OrdinalIgnoreCase)).ToArray());
}
