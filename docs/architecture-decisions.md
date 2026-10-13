# Architecture Decision Records

## ADR-001: Use Clean Architecture and ports

The API separates domain records, application services, infrastructure adapters, and HTTP composition. AWS services enter through ports such as `ObjectStoragePort` and `WorkflowCommandBus`, allowing deterministic local substitutes and isolated tests.

## ADR-002: Use Cognito-compatible JWT claims

Production authentication is designed around Cognito-issued JWTs with `sub`, `email`, and group claims. Local demo credentials remain a development adapter only. Authorization is enforced at the API boundary by role.

## ADR-003: Use asynchronous document workflows

Document submissions are modeled as commands that can move through S3, SQS, and Step Functions. The workflow contract supports retries, dead-letter recovery, idempotency, and status polling without coupling the domain to AWS SDK types.

## ADR-004: Use Redis as a read-through cache

Policy summaries use a versioned Redis cache key and a ten-minute expiration. Durable policy data belongs in SQL or Aurora; Redis is not the system of record.
