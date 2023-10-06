/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 * 
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 * 
 *  http://aws.amazon.com/apache2.0
 * 
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

/*
 * Do not modify this file. This file is generated from the lambda-2015-03-31.normal.json service model.
 */


using System.Threading;
using System.Threading.Tasks;

using Amazon.Lambda.Model;
using Amazon.Lambda.Model.Internal.MarshallTransformations;
using Amazon.Lambda.Internal;
using Amazon.Runtime;
using Amazon.Runtime.Internal;

namespace Amazon.Lambda
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
    ///  
    /// <para>
    ///  <b>CA certificates</b> 
    /// </para>
    ///  
    /// <para>
    /// Because Amazon Web Services SDKs use the CA certificates from your computer, changes
    /// to the certificates on the Amazon Web Services servers can cause connection failures
    /// when you attempt to use an SDK. You can prevent these failures by keeping your computer's
    /// CA certificates and operating system up-to-date. If you encounter this issue in a
    /// corporate environment and do not manage your own computer, you might need to ask an
    /// administrator to assist with the update process. The following list shows minimum
    /// operating system and Java versions:
    /// </para>
    ///  <ul> <li> 
    /// <para>
    /// Microsoft Windows versions that have updates from January 2005 or later installed
    /// contain at least one of the required CAs in their trust list. 
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Mac OS X 10.4 with Java for Mac OS X 10.4 Release 5 (February 2007), Mac OS X 10.5
    /// (October 2007), and later versions contain at least one of the required CAs in their
    /// trust list. 
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Red Hat Enterprise Linux 5 (March 2007), 6, and 7 and CentOS 5, 6, and 7 all contain
    /// at least one of the required CAs in their default trusted CA list. 
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Java 1.4.2_12 (May 2006), 5 Update 2 (March 2005), and all later versions, including
    /// Java 6 (December 2006), 7, and 8, contain at least one of the required CAs in their
    /// default trusted CA list. 
    /// </para>
    ///  </li> </ul> 
    /// <para>
    /// When accessing the Lambda management console or Lambda API endpoints, whether through
    /// browsers or programmatically, you will need to ensure your client machines support
    /// any of the following CAs: 
    /// </para>
    ///  <ul> <li> 
    /// <para>
    /// Amazon Root CA 1
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Starfield Services Root Certificate Authority - G2
    /// </para>
    ///  </li> <li> 
    /// <para>
    /// Starfield Class 2 Certification Authority
    /// </para>
    ///  </li> </ul> 
    /// <para>
    /// Root certificates from the first two authorities are available from <a href="https://www.amazontrust.com/repository/">Amazon
    /// trust services</a>, but keeping your computer up-to-date is the more straightforward
    /// solution. To learn more about ACM-provided certificates, see <a href="http://aws.amazon.com/certificate-manager/faqs/#certificates">Amazon
    /// Web Services Certificate Manager FAQs.</a> 
    /// </para>
    /// </summary>
    public partial class AmazonLambdaClient : AmazonServiceClient, IAmazonLambda
    {
        #region Constructors

        /// <summary>
        /// Constructs AmazonLambdaClient with the credentials loaded from the application's
        /// default configuration, and if unsuccessful from the Instance Profile service on an EC2 instance.
        /// 
        /// Example App.config with credentials set. 
        /// <code>
        /// &lt;?xml version="1.0" encoding="utf-8" ?&gt;
        /// &lt;configuration&gt;
        ///     &lt;appSettings&gt;
        ///         &lt;add key="AWSProfileName" value="AWS Default"/&gt;
        ///     &lt;/appSettings&gt;
        /// &lt;/configuration&gt;
        /// </code>
        ///
        /// </summary>
        public AmazonLambdaClient()
            : base(FallbackCredentialsFactory.GetCredentials(), new AmazonLambdaConfig()) { }

        #endregion

        #region Overrides

        /// <summary>
        /// Customizes the runtime pipeline.
        /// </summary>
        /// <param name="pipeline">Runtime pipeline for the current client.</param>
        protected override void CustomizeRuntimePipeline(RuntimePipeline pipeline)
        {
            pipeline.RemoveHandler<Amazon.Runtime.Internal.EndpointResolver>();
            pipeline.AddHandlerAfter<Amazon.Runtime.Internal.Marshaller>(new AmazonLambdaEndpointResolver());
        }

        #endregion

        #region Dispose

        /// <summary>
        /// Disposes the service client.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        /// <summary>
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
        /// <param name="request">Container for the necessary parameters to execute the Invoke service method.</param>
        /// <param name="cancellationToken">
        ///     A cancellation token that can be used by other objects or threads to receive notice of cancellation.
        /// </param>
        /// 
        /// <returns>The response from the Invoke service method, as returned by Lambda.</returns>
        /// <exception cref="Amazon.Lambda.Model.EC2AccessDeniedException">
        /// Need additional permissions to configure VPC settings.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EC2ThrottledException">
        /// Amazon EC2 throttled Lambda during Lambda function initialization using the execution
        /// role provided for the function.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EC2UnexpectedException">
        /// Lambda received an unexpected Amazon EC2 client exception while setting up for the
        /// Lambda function.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EFSIOException">
        /// An error occurred when reading from or writing to a connected file system.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EFSMountConnectivityException">
        /// The Lambda function couldn't make a network connection to the configured file system.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EFSMountFailureException">
        /// The Lambda function couldn't mount the configured file system due to a permission
        /// or configuration issue.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.EFSMountTimeoutException">
        /// The Lambda function made a network connection to the configured file system, but the
        /// mount operation timed out.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.ENILimitReachedException">
        /// Lambda couldn't create an elastic network interface in the VPC, specified as part
        /// of Lambda function configuration, because the limit for network interfaces has been
        /// reached. For more information, see <a href="https://docs.aws.amazon.com/lambda/latest/dg/gettingstarted-limits.html">Lambda
        /// quotas</a>.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidParameterValueException">
        /// One of the parameters in the request is not valid.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidRequestContentException">
        /// The request body could not be parsed as JSON.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidRuntimeException">
        /// The runtime or runtime version specified is not supported.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidSecurityGroupIDException">
        /// The security group ID provided in the Lambda function VPC configuration is not valid.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidSubnetIDException">
        /// The subnet ID provided in the Lambda function VPC configuration is not valid.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.InvalidZipFileException">
        /// Lambda could not unzip the deployment package.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.KMSAccessDeniedException">
        /// Lambda couldn't decrypt the environment variables because KMS access was denied. Check
        /// the Lambda function's KMS permissions.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.KMSDisabledException">
        /// Lambda couldn't decrypt the environment variables because the KMS key used is disabled.
        /// Check the Lambda function's KMS key settings.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.KMSInvalidStateException">
        /// Lambda couldn't decrypt the environment variables because the state of the KMS key
        /// used is not valid for Decrypt. Check the function's KMS key settings.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.KMSNotFoundException">
        /// Lambda couldn't decrypt the environment variables because the KMS key was not found.
        /// Check the function's KMS key settings.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.RecursiveInvocationException">
        /// Lambda has detected your function being invoked in a recursive loop with other Amazon
        /// Web Services resources and stopped your function's invocation.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.RequestTooLargeException">
        /// The request payload exceeded the <code>Invoke</code> request body JSON input quota.
        /// For more information, see <a href="https://docs.aws.amazon.com/lambda/latest/dg/gettingstarted-limits.html">Lambda
        /// quotas</a>.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.ResourceConflictException">
        /// The resource already exists, or another operation is in progress.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.ResourceNotFoundException">
        /// The resource specified in the request does not exist.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.ResourceNotReadyException">
        /// The function is inactive and its VPC connection is no longer available. Wait for the
        /// VPC connection to reestablish and try again.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.ServiceException">
        /// The Lambda service encountered an internal error.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.SnapStartException">
        /// The <code>afterRestore()</code> <a href="https://docs.aws.amazon.com/lambda/latest/dg/snapstart-runtime-hooks.html">runtime
        /// hook</a> encountered an error. For more information, check the Amazon CloudWatch logs.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.SnapStartNotReadyException">
        /// Lambda is initializing your function. You can invoke the function when the <a href="https://docs.aws.amazon.com/lambda/latest/dg/functions-states.html">function
        /// state</a> becomes <code>Active</code>.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.SnapStartTimeoutException">
        /// Lambda couldn't restore the snapshot within the timeout limit.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.SubnetIPAddressLimitReachedException">
        /// Lambda couldn't set up VPC access for the Lambda function because one or more configured
        /// subnets has no available IP addresses.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.TooManyRequestsException">
        /// The request throughput limit was exceeded. For more information, see <a href="https://docs.aws.amazon.com/lambda/latest/dg/gettingstarted-limits.html#api-requests">Lambda
        /// quotas</a>.
        /// </exception>
        /// <exception cref="Amazon.Lambda.Model.UnsupportedMediaTypeException">
        /// The content type of the <code>Invoke</code> request body is not JSON.
        /// </exception>
        /// <seealso href="http://docs.aws.amazon.com/goto/WebAPI/lambda-2015-03-31/Invoke">REST API Reference for Invoke Operation</seealso>
        public virtual Task<InvokeResponse> InvokeAsync(InvokeRequest request, CancellationToken cancellationToken)
        {
            var options = new InvokeOptions();
            options.RequestMarshaller = InvokeRequestMarshaller.Instance;
            options.ResponseUnmarshaller = InvokeResponseUnmarshaller.Instance;

            return InvokeAsync<InvokeResponse>(request, options, cancellationToken);
        }
    }
}