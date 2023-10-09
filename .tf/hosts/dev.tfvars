conventions = {
  application_name = "test"
  host_name        = "dev"
}

lambda_settings = {
  base_directory = "../src/output-compressed"
  functions = {
    "test" = {
      http_triggers = [{
        method    = "GET"
        route     = "/"
        anonymous = true
      }]
    }
  }
}

dynamodb_tables_settings = {
  "test" = {
    partition_key = "id"
  }
}
