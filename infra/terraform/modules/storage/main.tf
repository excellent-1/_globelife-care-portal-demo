variable "project_name" { type = string }
output "s3_bucket_contract" { value = "s3://${var.project_name}-documents" }
output "encryption" { value = "SSE-KMS" }
output "lifecycle" { value = "transition-and-expire-by-retention-policy" }
