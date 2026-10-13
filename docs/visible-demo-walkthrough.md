# Visible architecture demo walkthrough

## Customer scenario

1. Log in as `alex.customer`.
2. Observe the policy portfolio and coverage values.
3. Open policy detail to see query-service, cache, CQRS-style read, and correlation metadata.
4. Review the document workflow timeline showing object storage, queueing, validation, and persistence stages.
5. Use keyboard navigation and inspect live-region/error behavior.

## Support scenario

1. Log in as `maya.support`.
2. Review API p95 latency, Redis hit ratio, workflow counts, and deployment version.
3. Inspect support cases with priority, status, workflow ID, attempt count, and correlation ID.
4. Inspect the retrying workflow and its completed stages.
5. Run the retry and dead-letter demo controls.
6. Use the correlation ID in logs or the support ticket to trace the request.

## Technology evidence

- Vue 3 Composition API and Pinia state stores power the role-based views.
- TypeScript contracts define policy, workflow, metrics, and support data.
- ASP.NET Core maps authenticated endpoints and ProblemDetails behavior.
- Clean Architecture ports isolate object storage and workflow delivery.
- Redis supplies read-through policy caching.
- S3, SQS, and Step Functions are represented by replaceable production adapters.
- Terraform defines Cognito, storage, and workflow resource contracts.
- GitHub Actions builds the API, frontend, Docker image, and validates Terraform.
- Correlation IDs and request timings support production triage.
