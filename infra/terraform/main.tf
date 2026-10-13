terraform {
  required_version = ">= 1.6.0"
}

variable "project_name" {
  type    = string
  default = "globelife-care-portal"
}

variable "aws_region" {
  type    = string
  default = "us-east-1"
}

variable "container_image" {
  type    = string
  default = "replace-with-registry-image"
}

variable "jwt_secret_arn" {
  type    = string
  default = "replace-with-secrets-manager-arn"
}

module "identity" {
  source       = "./modules/cognito"
  project_name = var.project_name
}

module "workflow" {
  source       = "./modules/workflow"
  project_name = var.project_name
}

module "storage" {
  source       = "./modules/storage"
  project_name = var.project_name
}

output "aws_region" {
  value = var.aws_region
}

output "container_image" {
  value = var.container_image
}

output "jwt_secret_arn" {
  value     = var.jwt_secret_arn
  sensitive = true
}

output "required_runtime_port" {
  value = 8080
}
