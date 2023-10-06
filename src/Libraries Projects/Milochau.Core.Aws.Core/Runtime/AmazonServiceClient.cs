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

using Amazon.Runtime.Internal.Auth;
using Amazon.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Linq;
using ExecutionContext = Amazon.Runtime.Internal.ExecutionContext;
using Amazon.Runtime.Internal;

namespace Amazon.Runtime
{
    public abstract class AmazonServiceClient : IDisposable
    {
        private bool _disposed;
        protected RuntimePipeline RuntimePipeline { get; set; }
        protected internal AWSCredentials Credentials { get; private set; }
        public IClientConfig Config => _config;
        private readonly ClientConfig _config;

        #region Constructors

        protected AmazonServiceClient(AWSCredentials credentials, ClientConfig config)
        {
            Credentials = credentials;
            _config = config;
            Signer = new AWS4Signer();
            Initialize();
            BuildRuntimePipeline();
        }

        protected AbstractAWSSigner Signer
        {
            get;
            private set;
        }

        protected virtual void Initialize()
        {
        }

        #endregion

        #region Invoke methods

        protected System.Threading.Tasks.Task<TResponse> InvokeAsync<TResponse>(
            AmazonWebServiceRequest request,
            InvokeOptionsBase options,
            System.Threading.CancellationToken cancellationToken)
            where TResponse : AmazonWebServiceResponse, new()
        {
            ThrowIfDisposed();

            var executionContext = new ExecutionContext(
                new RequestContext(Signer)
                {
                    ClientConfig = this.Config,
                    Marshaller = options.RequestMarshaller,
                    OriginalRequest = request,
                    Unmarshaller = options.ResponseUnmarshaller,
                    IsAsync = true,
                    CancellationToken = cancellationToken,
                    Options = options
                },
                new ResponseContext()
            );
            return this.RuntimePipeline.InvokeAsync<TResponse>(executionContext);
        }

        #endregion

        #region Dispose methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (RuntimePipeline != null)
                    RuntimePipeline.Dispose();

                _disposed = true;
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }

        #endregion

        protected virtual void CustomizeRuntimePipeline(RuntimePipeline pipeline) { }

        private void BuildRuntimePipeline()
        {
            var httpRequestFactory = new HttpRequestMessageFactory();
            var httpHandler = new HttpHandler<System.Net.Http.HttpContent>(httpRequestFactory, this);

            //Determine which retry policy to use based on the retry mode
            RetryPolicy retryPolicy = new StandardRetryPolicy(Config);

            // Build default runtime pipeline.
            RuntimePipeline = new RuntimePipeline(new List<IPipelineHandler>
                {
                    httpHandler,
                    new Unmarshaller(true),
                    new ErrorHandler(),
                    new Signer(),
                    // ChecksumHandler must come after CompressionHandler because we must calculate the checksum of a payload after compression.
                    // ChecksumHandler must come after EndpointsResolver because of an upcoming project.
                    new ChecksumHandler(),
                    // CredentialsRetriever must come after RetryHandler because of any credential related changes.
                    new CredentialsRetriever(Credentials),
                    new RetryHandler(retryPolicy),
                    new Marshaller(),
                }
            );

            CustomizeRuntimePipeline(RuntimePipeline);

            // Apply global pipeline customizations
            RuntimePipelineCustomizerRegistry.Instance.ApplyCustomizations(GetType(), RuntimePipeline);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="iRequest">Request to compute Uri for</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest iRequest)
        {
            return ComposeUrl(iRequest, true);
        }

        /// <summary>
        /// Assembles the Uri for a given SDK request
        /// </summary>
        /// <param name="internalRequest">Request to compute Uri for</param>
        /// <param name="skipEncodingValidPathChars">If true the accepted path characters {/+:} are not encoded.</param>
        /// <returns>Uri for the given SDK request</returns>
        public static Uri ComposeUrl(IRequest internalRequest, bool skipEncodingValidPathChars)
        {
            Uri url = internalRequest.Endpoint;
            var resourcePath = internalRequest.ResourcePath;
            if (resourcePath == null)
                resourcePath = string.Empty;
            else
            {
                if (resourcePath.StartsWith("/", StringComparison.Ordinal))
                    resourcePath = resourcePath.Substring(1);

                // Microsoft added support for unicode bidi control characters to the Uri class in .NET 4.7.2
                // https://github.com/microsoft/dotnet/blob/master/Documentation/compatibility/uri-unicode-bidirectional-characters.md
                // However, we only want to support it on .NET Core 3.1 and higher due to not having to deal with .NET Standard support matrix.
                if (AWSSDKUtils.HasBidiControlCharacters(resourcePath) ||
                    (internalRequest.PathResources?.Any(v => AWSSDKUtils.HasBidiControlCharacters(v.Value)) == true))
                {
                    resourcePath = string.Join("/", AWSSDKUtils.SplitResourcePathIntoSegments(resourcePath, internalRequest.PathResources).ToArray());
                    throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture,
                        "Target resource path [{0}] has bidirectional characters, which are not supported" +
                        "by System.Uri and thus cannot be handled by the .NET SDK.", resourcePath));
                }

                resourcePath = AWSSDKUtils.ResolveResourcePath(resourcePath, internalRequest.PathResources, skipEncodingValidPathChars);
            }

            // Construct any sub resource/query parameter additions to append to the
            // resource path. Services like S3 which allow '?' and/or '&' in resource paths 
            // should use SubResources instead of appending them to the resource path with 
            // query string delimiters during request marshalling.

            var delim = "?";
            var sb = new StringBuilder();

            if (internalRequest.SubResources?.Count > 0)
            {
                foreach (var subResource in internalRequest.SubResources)
                {
                    sb.AppendFormat("{0}{1}", delim, subResource.Key);
                    if (subResource.Value != null)
                        sb.AppendFormat("={0}", subResource.Value);
                    delim = "&";
                }
            }

            if (internalRequest.UseQueryString && internalRequest.Parameters?.Count > 0)
            {
                var queryString = AWSSDKUtils.GetParametersAsString(internalRequest);
                sb.AppendFormat("{0}{1}", delim, queryString);
            }

            var parameterizedPath = string.Empty;
            if (internalRequest.MarshallerVersion >= 2)
            {
                parameterizedPath = string.Concat(resourcePath, sb);
            }
            else
            {
                if (AWSSDKUtils.HasBidiControlCharacters(resourcePath))
                    throw new AmazonClientException(string.Format(CultureInfo.InvariantCulture,
                        "Target resource path [{0}] has bidirectional characters, which are not supported" +
                        "by System.Uri and thus cannot be handled by the .NET SDK.", resourcePath));

                parameterizedPath = string.Concat(AWSSDKUtils.ProtectEncodedSlashUrlEncode(resourcePath, skipEncodingValidPathChars), sb);
            }

            var hasSlash = url.AbsoluteUri.EndsWith("/", StringComparison.Ordinal) || parameterizedPath.StartsWith("/", StringComparison.Ordinal);
            var uri = hasSlash
                ? new Uri(url.AbsoluteUri + parameterizedPath)
                : new Uri(url.AbsoluteUri + "/" + parameterizedPath);
            return uri;
        }
    }
}
