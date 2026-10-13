import { describe, expect, it, vi } from 'vitest';
import { AuthorizedApiClient } from './api-client';

describe('AuthorizedApiClient', () => {
  it('sends bearer authorization while loading policy summaries', async () => {
    const fetchMock = vi.fn().mockResolvedValue({ ok:true, json:async()=>[{ policyNumber:'GL-TEST', productName:'Test', status:'Active', coverageAmount:1, renewalDate:'2026-01-01' }] });
    vi.stubGlobal('fetch', fetchMock);
    const policies = await new AuthorizedApiClient(()=>'test-token').getPolicySummaries();
    expect(policies[0].policyNumber).toBe('GL-TEST');
    expect(fetchMock).toHaveBeenCalledWith('/api/policies',{headers:{Authorization:'Bearer test-token'}});
  });
});
