context = {
  organization_name = "mil"
  application_name  = "test"
  host_name         = "dev"
  temporary         = true
}

assume_roles = {
  workloads = "arn:aws:iam::381492034295:role/administrator-access"
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
      lambda_accesses = [{
        arn = "arn:aws:lambda:eu-west-3:266302224431:function:emails-dev-fn-async-send-emails"
      }]
    }
  }
}

dynamodb_tables_settings = {
  "test" = {
    partition_key = "id"
  }
}
