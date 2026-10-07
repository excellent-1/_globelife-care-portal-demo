terraform {
  required_version = ">= 1.6.0"
}

variable "app_name" {
  type    = string
  default = "globelife-care-portal"
}

variable "container_image" {
  type    = string
  default = "replace-with-registry-image"
}

variable "redis_connection_string" {
  type      = string
  sensitive = true
  default   = "replace-with-hosted-redis-url"
}

variable "jwt_signing_key" {
  type      = string
  sensitive = true
  default   = "replace-with-secret-key"
}

# Render-style deployment contract. Supply these values to a selected free-tier
# container host, or replace this provider-neutral module with its Terraform provider.
output "deployment_name" {
  value = var.app_name
}

output "container_image" {
  value = var.container_image
}

output "required_port" {
  value = 8080
}

output "redis_connection_string" {
  value     = var.redis_connection_string
  sensitive = true
}

output "environment_contract" {
  value = {
    Redis__ConnectionString = "secret"
    Jwt__SigningKey         = "secret"
  }
}
