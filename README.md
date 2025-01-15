<p align="center">
  <a href="https://github.com/amilochau/core-aws/blob/main/LICENSE">
    <img src="https://img.shields.io/github/license/amilochau/core-aws" alt="License">
  </a>
  <a href="https://github.com/amilochau/core-aws/releases">
    <img src="https://img.shields.io/github/v/release/amilochau/core-aws" alt="Release">
  </a>
  <a href="https://www.nuget.org/packages?q=Milochau.Core.Aws">
    <img src="https://img.shields.io/nuget/vpre/Milochau.Core.Aws.Abstractions" alt="Version">
  </a>
  <a href="https://www.nuget.org/packages?q=Milochau.Core.Aws">
    <img src="https://img.shields.io/nuget/dt/Milochau.Core.Aws.Abstractions" alt="Downloads">
  </a>
</p>
<h1 align="center">
  @amilochau/core-aws
</h1>

`@amilochau/core-aws` is a set of opinionated packages used to create AWS Lambda functions with .NET 9.0 native AOT. This repository comes from a fork of multiple AWS dotnet packages, rewritten to focus on performances:

- [aws/aws-sdk-dotnet](https://github.com/aws/aws-sdk-net)
- [aws/aws-lambda-dotnet](https://github.com/aws/aws-lambda-dotnet)
- [aws/aws-xray-sdk-dotnet](https://github.com/aws/aws-xray-sdk-dotnet)

## Main features

- Web: ASP.NET application libraries, optimized to run within Lambda functions
- API Gateway: authentication checks, request validations
- DynamoDB: document attribute mapping, expressions based on code generation
- Integration: multi-lambda integration application baseline

## Usage

`amilochau/core-aws` is proposed as a set of NuGet packages.

1. Install the NuGet packages

Run the following command to install the NuGet packages - use the packages you want for your use case:

```pwsh
dotnet add package Milochau.Core.Aws.Abstractions
dotnet add package Milochau.Core.Aws.ApiGateway
dotnet add package Milochau.Core.Aws.Cognito
dotnet add package Milochau.Core.Aws.Core
dotnet add package Milochau.Core.Aws.DynamoDB
dotnet add package Milochau.Core.Aws.Integration
dotnet add package Milochau.Core.Aws.Lambda
dotnet add package Milochau.Core.Aws.SESv2
dotnet add package Milochau.Core.Aws.SNS
```

2. Use the packages

See the reference projects to find usage examples:
- [ASP.NET lambda function](./src/Reference%20Projects/Milochau.Core.Aws.ReferenceProjects.AspNetCore)
- [Event-based lambda function](./src/Reference%20Projects/Milochau.Core.Aws.ReferenceProjects.LambdaFunction)
- [Integration project](./src/Reference%20Projects/Milochau.Core.Aws.ReferenceProjects.Integration), typically used to test multiple lambda functions together

--- 

## Contribute

Feel free to push your code if you agree with publishing under the [MIT license](./LICENSE).
