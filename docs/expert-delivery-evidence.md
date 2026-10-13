# Expert-level delivery evidence

## Vertical slice: policy document submission

- Vue UI requests an upload URL.
- A customer-scoped API validates the request.
- `ObjectStoragePort` represents S3 presigned upload behavior.
- `WorkflowCommandBus` represents SQS delivery.
- `DocumentWorkflowOrchestrator` represents Step Functions coordination.
- Workflow state supports retries and operator recovery.
- Correlation IDs connect UI, API, queue, and workflow logs.

## Quality gates

- `dotnet build` must pass without warnings treated as errors.
- Frontend production build must pass.
- Terraform validation must pass.
- Docker image build must pass.
- API authorization tests must cover anonymous, Customer, and Support roles.
- Playwright tests should cover login, policies, support queue, and failure states.
- SonarQube/SonarCloud should enforce the configured quality gate.

## Interview demonstration

Show the feature from requirement to operation: explain the ADR, implement the command, validate authorization, queue work, observe the correlation ID, inspect retry behavior, and execute the rollback runbook.
