using System.Threading;
using System.Threading.Tasks;
using Milochau.Core.Aws.Core.Runtime;
using Milochau.Core.Aws.Core.Runtime.Credentials;
using Milochau.Core.Aws.Lambda.Model;
using Milochau.Core.Aws.Lambda.Model.MarshallTransformations;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Implementation for accessing Lambda
    ///
    /// Lambda 
    /// <para>
    ///  <b>Overview</b> 
    /// </para>
    ///  
    /// <para>
    /// Lambda is a compute service that lets you run code without provisioning or managing
    /// servers. Lambda runs your code on a high-availability compute infrastructure and performs
    /// all of the administration of the compute resources, including server and operating
    /// system maintenance, capacity provisioning and automatic scaling, code monitoring and
    /// logging. With Lambda, you can run code for virtually any type of application or backend
    /// service. For more information about the Lambda service, see <a href="https://docs.aws.amazon.com/lambda/latest/dg/welcome.html">What
    /// is Lambda</a> in the <b>Lambda Developer Guide</b>.
    /// </para>
    ///  
    /// <para>
    /// The <i>Lambda API Reference</i> provides information about each of the API methods,
    /// including details about the parameters in each API request and response. 
    /// </para>
    ///   
    /// <para>
    /// You can use Software Development Kits (SDKs), Integrated Development Environment (IDE)
    /// Toolkits, and command line tools to access the API. For installation instructions,
    /// see <a href="http://aws.amazon.com/tools/">Tools for Amazon Web Services</a>. 
    /// </para>
    ///  
    /// <para>
    /// For a list of Region-specific endpoints that Lambda supports, see <a href="https://docs.aws.amazon.com/general/latest/gr/lambda-service.html/">Lambda
    /// endpoints and quotas </a> in the <i>Amazon Web Services General Reference.</i>. 
    /// </para>
    ///  
    /// <para>
    /// When making the API calls, you will need to authenticate your request by providing
    /// a signature. Lambda supports signature version 4. For more information, see <a href="https://docs.aws.amazon.com/general/latest/gr/signature-version-4.html">Signature
    /// Version 4 signing process</a> in the <i>Amazon Web Services General Reference.</i>.
    /// 
    /// </para>
    /// </summary>
    public partial class AmazonLambdaClient : AmazonServiceClient, IAmazonLambda
    {
        #region Constructors

        /// <summary>
        /// Constructs AmazonLambdaClient
        /// </summary>
        public AmazonLambdaClient(IAWSCredentials credentials)
            : base(credentials, new ClientConfig
            {
                AuthenticationServiceName = "lambda",
                MonitoringServiceName = "Lambda",
            })
        { }

        #endregion

        /// <summary>
        /// Invokes a Lambda function. You can invoke a function synchronously (and wait for the
        /// response), or asynchronously. To invoke a function asynchronously, set <c>InvocationType</c>
        /// to <c>Event</c>.
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
        /// returns <c>TooManyRequestsException</c> if running the function would cause
        /// you to exceed a concurrency limit at either the account level (<c>ConcurrentInvocationLimitExceeded</c>)
        /// or function level (<c>ReservedFunctionConcurrentInvocationLimitExceeded</c>).
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
        /// <param name="request">Container for the necessary parameters to execute the Invoke service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the Invoke service method, as returned by Lambda.</returns>
        /// <exception cref="AmazonLambdaException">
        /// Ar error occurred.
        /// </exception>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/lambda-2015-03-31/Invoke">REST API Reference for Invoke Operation</seealso>
        public virtual Task<InvokeResponse> InvokeAsync(InvokeRequest request, CancellationToken cancellationToken) => InvokeAsync(request, new InvokeInvokeOptions(), cancellationToken);
    }
}