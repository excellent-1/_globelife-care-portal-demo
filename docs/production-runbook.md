# Production support runbook

## API 5xx response

1. Check the service health endpoint.
2. Inspect logs using the Render or CloudWatch service logs.
3. Capture the `X-Correlation-ID` response header.
4. Search structured logs by correlation ID.
5. Confirm Redis, database, SQS, and downstream status.
6. Roll back the immutable image tag if the failure follows a deployment.

## Redis unavailable

The API should return a controlled ProblemDetails response rather than an unhandled exception. Confirm the TLS URL, hostname, secret rotation, network egress, and Redis provider status. Clear or rebuild the versioned cache only after the system of record is healthy.

## Document workflow backlog

Inspect SQS queue depth and message age. Review the dead-letter queue. Re-drive only idempotent messages after fixing the root cause. Verify workflow status transitions and correlation IDs.

## Authentication failures

Confirm Cognito issuer, audience, signing keys, group claims, clock skew, and API role mapping. Never disable signature or lifetime validation to recover a deployment.

## Deployment rollback

1. Stop promotion.
2. Identify the last healthy image tag.
3. Deploy the previous immutable image.
4. Run health, login, policy, and support smoke tests.
5. Record the incident and create a remediation item.
