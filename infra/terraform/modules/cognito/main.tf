variable "project_name" { type = string }
output "user_pool_contract" { value = "cognito-user-pool:${var.project_name}" }
output "oauth_flow" { value = "authorization-code-with-pkce" }
output "required_claims" { value = ["sub", "email", "cognito:groups"] }
