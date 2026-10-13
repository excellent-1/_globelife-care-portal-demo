export type PolicySummary = { policyNumber:string; productName:string; status:'Active'|'Pending'|'Lapsed'|'Cancelled'; coverageAmount:number; renewalDate:string };
export type SupportCase = { id:string; priority:'High'|'Normal'|'Low'; subject:string; status:'Open'|'Closed' };

export class AuthorizedApiClient {
  constructor(private readonly tokenProvider: () => string) {}
  private async getJson<T>(path:string):Promise<T>{ const response=await fetch(path,{headers:{Authorization:`Bearer ${this.tokenProvider()}`}}); if(!response.ok) throw new Error(`API request failed: ${response.status}`); return response.json() as Promise<T>; }
  getPolicySummaries(){ return this.getJson<PolicySummary[]>('/api/policies'); }
  getSupportQueue(){ return this.getJson<SupportCase[]>('/api/support/queue'); }
}
