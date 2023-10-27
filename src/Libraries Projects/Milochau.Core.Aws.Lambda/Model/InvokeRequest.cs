using Milochau.Core.Aws.Core.Util;
using System.Collections.Generic;
using System.IO;

namespace Milochau.Core.Aws.Lambda.Model
{
    /// <summary>
    /// Container for the parameters to the Invoke operation.
    /// Invokes a Lambda function. You can invoke a function synchronously (and wait for the
    /// response), or asynchronously. To invoke a function asynchronously, set <code>InvocationType</code>
    /// to <code>Event</code>.
    /// 
    ///  
    /// <para>
    /// For <a href="https://docs.aws.amazon.com/lambda/latest/dg/invocation-sync.html">synchronous
    /// invocation</a>, details about the function response, including errors, are included
    /// in the response body and headers. For either invocation type, you can find more information
    /// in the <a href="https://docs.aws.amazon.com/lambda/latest/dg/monitoring-functions.html">execution
    /// log</a> and <a href="https://docs.aws.amazon.com/lambda/latest/dg/lambda-x-ray.html">trace</a>.
    /// </para>
    ///  
    /// <para>
    /// When an error occurs, your function may be invoked multiple times. Retry behavior
    /// varies by error type, client, event source, and invocation type. For example, if you
    /// invoke a function asynchronously and it returns an error, Lambda executes the function
    /// up to two more times. For more information, see <a href="https://docs.aws.amazon.com/lambda/latest/dg/invocation-retries.html">Error
    /// handling and automatic retries in Lambda</a>.
    /// </para>
    ///  
    /// <para>
    /// For <a href="https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html">asynchronous
    /// invocation</a>, Lambda adds events to a queue before sending them to your function.
    /// If your function does not have enough capacity to keep up with the queue, events may
    /// be lost. Occasionally, your function may receive the same event multiple times, even
    /// if no error occurs. To retain events that were not processed, configure your function
    /// with a <a href="https://docs.aws.amazon.com/lambda/latest/dg/invocation-async.html#invocation-dlq">dead-letter
    /// queue</a>.
    /// </para>
    ///  
    /// <para>
    /// The status code in the API response doesn't reflect function errors. Error codes are
    /// reserved for errors that prevent your function from executing, such as permissions
    /// errors, <a href="https://docs.aws.amazon.com/lambda/latest/dg/gettingstarted-limits.html">quota</a>
    /// errors, or issues with your function's code and configuration. For example, Lambda
    /// returns <code>TooManyRequestsException</code> if running the function would cause
    /// you to exceed a concurrency limit at either the account level (<code>ConcurrentInvocationLimitExceeded</code>)
    /// or function level (<code>ReservedFunctionConcurrentInvocationLimitExceeded</code>).
    /// </para>
    ///  
    /// <para>
    /// For functions with a long timeout, your client might disconnect during synchronous
    /// invocation while it waits for a response. Configure your HTTP client, SDK, firewall,
    /// proxy, or operating system to allow for long connections with timeout or keep-alive
    /// settings.
    /// </para>
    ///  
    /// <para>
    /// This operation requires permission for the <a href="https://docs.aws.amazon.com/IAM/latest/UserGuide/list_awslambda.html">lambda:InvokeFunction</a>
    /// action. For details on how to set up permissions for cross-account invocations, see
    /// <a href="https://docs.aws.amazon.com/lambda/latest/dg/access-control-resource-based.html#permissions-resource-xaccountinvoke">Granting
    /// function access to other accounts</a>.
    /// </para>
    /// </summary>
    public partial class InvokeRequest : AmazonLambdaRequest
    {
        /// <summary>
        /// Gets and sets the property ClientContextBase64. 
        /// <para>
        /// Up to 3,583 bytes of base64-encoded data about the invoking client to pass to the
        /// function in the context object.
        /// </para>
        /// </summary>
        public string? ClientContextBase64 { get; set; }

        /// <summary>
        /// Gets and sets the property FunctionName. 
        /// <para>
        /// The name of the Lambda function, version, or alias.
        /// </para>
        ///  
        /// <para>
        ///  <b>Name formats</b> 
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <b>Function name</b> – <code>my-function</code> (name-only), <code>my-function:v1</code>
        /// (with alias).
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <b>Function ARN</b> – <code>arn:aws:lambda:us-west-2:123456789012:function:my-function</code>.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <b>Partial ARN</b> – <code>123456789012:function:my-function</code>.
        /// </para>
        ///  </li> </ul> 
        /// <para>
        /// You can append a version number or alias to any of the formats. The length constraint
        /// applies only to the full ARN. If you specify only the function name, it is limited
        /// to 64 characters in length.
        /// </para>
        /// </summary>
        public required string FunctionName { get; set; }

        /// <summary>
        /// Gets and sets the property InvocationType. 
        /// <para>
        /// Choose from the following options.
        /// </para>
        ///  <ul> <li> 
        /// <para>
        ///  <code>RequestResponse</code> (default) – Invoke the function synchronously. Keep
        /// the connection open until the function returns a response or times out. The API response
        /// includes the function response and additional data.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>Event</code> – Invoke the function asynchronously. Send events that fail multiple
        /// times to the function's dead-letter queue (if one is configured). The API response
        /// only includes a status code.
        /// </para>
        ///  </li> <li> 
        /// <para>
        ///  <code>DryRun</code> – Validate parameter values and verify that the user or role
        /// has permission to invoke the function.
        /// </para>
        ///  </li> </ul>
        /// </summary>
        /// <remarks><see cref="Lambda.InvocationType"/></remarks>
        public string? InvocationType { get; set; }

        /// <summary>
        /// Gets and sets the property PayloadStream. 
        /// <para>
        /// The JSON that you want to provide to your Lambda function as input.
        /// </para>
        ///  
        /// <para>
        /// You can enter the JSON directly. For example, <code>--payload '{ "key": "value" }'</code>.
        /// You can also specify a file path. For example, <code>--payload file://payload.json</code>.
        /// </para>
        /// </summary>
        public MemoryStream? PayloadStream { get; set; }

        /// <summary>
        /// Gets and sets the property Payload. When this property is set the PayloadStream
        /// property is also set with a MemoryStream containing the contents of Payload.
        /// <para>
        /// JSON that you want to provide to your cloud function as input.
        /// </para>
        /// </summary>
        public string? Payload
        {
            get
            {
                string? content = null;
                if (PayloadStream != null)
                {
                    content = new StreamReader(PayloadStream).ReadToEnd();
                    PayloadStream.Position = 0;
                }
                return content;
            }
            set
            {
                if (value == null)
                    PayloadStream = null;
                else
                    PayloadStream = AWSSDKUtils.GenerateMemoryStreamFromString(value);
            }
        }

        /// <summary>Get request parameters for XRay</summary>
        public override Dictionary<string, object?> GetXRayRequestParameters()
        {
            return new Dictionary<string, object?>
            {
                { "function_name", FunctionName },
            };
        }
    }
}