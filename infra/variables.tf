variable "context" {
  description = "Context to use"
  type = object({
    organization_name = string
    application_name = string
    host_name        = string
  })
}

variable "assume_roles" {
  description = "Roles to be assumed"
  type = object({
    workloads      = string
  })
}

variable "aws_provider_settings" {
  description = "Settings to configure the AWS provider"
  type = object({
    region = optional(string, "eu-west-3")
  })
  default = {}
}

variable "lambda_settings" {
  description = "Lambda settings"
  type = object({
    base_directory = string
    functions = map(object({
      memory_size_mb        = optional(number, 512)
      timeout_s             = optional(number, 10)
      package_file          = optional(string, "bootstrap.zip")
      handler               = optional(string, "bootstrap")
      environment_variables = optional(map(string), {})
      http_triggers = optional(list(object({
        description = optional(string, null)
        method      = string
        route       = string
        anonymous   = optional(bool, false)
        enable_cors = optional(bool, false)
      })), [])
      sns_triggers = optional(list(object({
        description = optional(string, null)
        topic_name  = string
      })), [])
      scheduler_triggers = optional(list(object({
        description         = optional(string, null)
        schedule_expression = string
        enabled             = optional(bool, true)
      })), [])
      ses_accesses = optional(list(object({
        domain = string
      })), [])
      lambda_accesses = optional(list(object({
        arn = string
      })), [])
    }))
  })
}

variable "dynamodb_tables_settings" {
  description = "Settings to configure DynamoDB tables for the API"
  type = map(object({
    partition_key = string
    sort_key      = optional(string, null)
    attributes = optional(map(object({
      type = string
    })), {})
    ttl = optional(object({
      enabled        = bool
      attribute_name = optional(string, "ttl")
      }), {
      enabled = false
    })
    global_secondary_indexes = optional(map(object({
      partition_key      = string
      sort_key           = string
      non_key_attributes = list(string)
    })), {})
  }))
  default = {}
}
