variable "project_name" { type = string }
output "sqs_queue_contract" { value = "${var.project_name}-document-commands" }
output "dead_letter_queue_contract" { value = "${var.project_name}-document-commands-dlq" }
output "step_functions_contract" { value = "${var.project_name}-document-workflow" }
