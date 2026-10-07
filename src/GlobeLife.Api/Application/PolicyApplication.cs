using GlobeLife.Api.Domain;
using StackExchange.Redis;
using System.Text.Json;

namespace GlobeLife.Api.Application;

public interface IPolicyRepository { Task<IReadOnlyList<PolicySummary>> GetPoliciesAsync(CancellationToken token); }
public sealed class PolicyRepository(IConnectionMultiplexer redis) : IPolicyRepository
{
    public async Task<IReadOnlyList<PolicySummary>> GetPoliciesAsync(CancellationToken token)
    {
        var database = redis.GetDatabase();
        const string cacheKey = "globelife:policy-summary:v1";
        var cachedJson = await database.StringGetAsync(cacheKey);
        if (cachedJson.HasValue) return JsonSerializer.Deserialize<List<PolicySummary>>(cachedJson!) ?? [];
        var policies = new List<PolicySummary>
        {
            new("GL-2048-119", "Family Heritage Whole Life", "Active", 250000, new DateOnly(2026, 9, 15)),
            new("GL-2048-120", "SecureTerm 20", "Active", 500000, new DateOnly(2027, 2, 1))
        };
        await database.StringSetAsync(cacheKey, JsonSerializer.Serialize(policies), TimeSpan.FromMinutes(10));
        return policies;
    }
}

public sealed class GetPolicyDashboardQueryHandler(IPolicyRepository policyRepository)
{
    public Task<IReadOnlyList<PolicySummary>> HandleAsync(CancellationToken token) => policyRepository.GetPoliciesAsync(token);
}
