using GlobeLife.Api.Domain;

namespace GlobeLife.Api.Application;

public interface IPolicyQueryService
{
    Task<IReadOnlyList<PolicySummary>> GetCustomerPoliciesAsync(string customerId, CancellationToken cancellationToken);
}

public sealed class PolicyQueryService(IPolicyRepository policyRepository) : IPolicyQueryService
{
    public Task<IReadOnlyList<PolicySummary>> GetCustomerPoliciesAsync(string customerId, CancellationToken cancellationToken)
        => policyRepository.GetPoliciesAsync(cancellationToken);
}

public sealed record ApiFailure(string Code, string Message, string CorrelationId);
