using Milochau.Core.Aws.ReferenceProjects.LambdaFunctions.Internals.Context;
using Moq;
using System;

namespace Milochau.Core.Aws.ReferenceProjects.Integration
{
    //
    // Résumé :
    //     A test implementation of the ILambdaContext interface used for writing local
    //     tests of Lambda Functions.
    public class TestLambdaContext : ILambdaContext
    {
        //
        // Résumé :
        //     The AWS request ID associated with the request.
        public string AwsRequestId { get; set; } = null!;

        //
        // Résumé :
        //     Information about the client application and device when invoked through the
        //     AWS Mobile SDK.
        public IClientContext ClientContext { get; set; } = null!;

        //
        // Résumé :
        //     Name of the Lambda function that is running.
        public string FunctionName { get; set; } = null!;

        //
        // Résumé :
        //     The Lambda function version that is executing. If an alias is used to invoke
        //     the function, then this will be the version the alias points to.
        public string FunctionVersion { get; set; } = null!;

        //
        // Résumé :
        //     Information about the Amazon Cognito identity provider when invoked through the
        //     AWS Mobile SDK.
        public ICognitoIdentity Identity { get; set; } = null!;

        //
        // Résumé :
        //     The ARN used to invoke this function.
        public string InvokedFunctionArn { get; set; } = null!;

        //
        // Résumé :
        //     Lambda logger associated with the Context object. For the TestLambdaContext this
        //     is default to the TestLambdaLogger.
        public ILambdaLogger Logger { get; set; } = new Mock<ILambdaLogger>().Object;


        //
        // Résumé :
        //     The CloudWatch log group name associated with the invoked function.
        public string LogGroupName { get; set; } = null!;

        //
        // Résumé :
        //     The CloudWatch log stream name for this function execution.
        public string LogStreamName { get; set; } = null!;

        //
        // Résumé :
        //     Memory limit, in MB, you configured for the Lambda function.
        public int MemoryLimitInMB { get; set; }

        //
        // Résumé :
        //     Remaining execution time till the function will be terminated.
        public TimeSpan RemainingTime { get; set; }
    }
}
