using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipelines;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Milochau.Core.Aws.Core.Lambda.Events;
using Microsoft.Extensions.Primitives;
using System.Globalization;
using Milochau.Core.Aws.Core.Lambda.Core;
using Milochau.Core.Aws.Core.References;
using Microsoft.Extensions.DependencyInjection;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal
{
    public class InvokeFeatures //(APIGatewayHttpApiV2ProxyRequest apiGatewayRequest, IServiceProvider serviceProvider, ILambdaContext lambdaContext)
        : IFeatureCollection,
          IItemsFeature,
          IHttpAuthenticationFeature,
          IHttpRequestFeature,
          IHttpResponseFeature,
          IHttpConnectionFeature,
          IServiceProvidersFeature,
          ITlsConnectionFeature,
          IHttpRequestIdentifierFeature,
          IHttpResponseBodyFeature,
          IHttpRequestBodyDetectionFeature,
          IHttpActivityFeature
    {
        /// <summary>Key to access the ILambdaContext object from the HttpContext.Items collection</summary>
        private const string LAMBDA_CONTEXT = "LambdaContext";

        /// <summary>
        /// Key to access the Lambda request object from the HttpContext.Items collection. The object
        /// can be either APIGatewayProxyRequest or ApplicationLoadBalancerRequest depending on the source of the event.
        /// </summary>
        private const string LAMBDA_REQUEST_OBJECT = "LambdaRequestObject";

        private volatile int _containerRevision;

        public InvokeFeatures(APIGatewayHttpApiV2ProxyRequest apiGatewayRequest, IServiceProvider serviceProvider, ILambdaContext lambdaContext)
        {
            this[typeof(IItemsFeature)] = this;
            this[typeof(IHttpAuthenticationFeature)] = this;
            this[typeof(IHttpRequestFeature)] = this;
            this[typeof(IHttpResponseFeature)] = this;
            this[typeof(IHttpConnectionFeature)] = this;
            this[typeof(IServiceProvidersFeature)] = this;
            this[typeof(ITlsConnectionFeature)] = this;
            this[typeof(IHttpResponseBodyFeature)] = this;
            this[typeof(IHttpRequestIdentifierFeature)] = this;
            this[typeof(IHttpRequestBodyDetectionFeature)] = this;
            this[typeof(IHttpActivityFeature)] = this;

            {
                var authFeatures = (IHttpAuthenticationFeature)this;
                var authorizer = apiGatewayRequest.RequestContext.Authorizer;

                if (authorizer != null)
                {
                    // handling claims output from cognito user pool authorizer
                    if (authorizer.Jwt?.Claims != null && authorizer.Jwt.Claims.Count != 0)
                    {
                        var identity = new ClaimsIdentity(authorizer.Jwt.Claims.Select(x => new Claim(x.Key, x.Value.ToString())), "AuthorizerIdentity");
                        authFeatures.User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        // handling claims output from custom lambda authorizer
                        var identity = new ClaimsIdentity(authorizer.Jwt?.Claims?.Select(x => new Claim(x.Key, x.Value)), "AuthorizerIdentity");
                        authFeatures.User = new ClaimsPrincipal(identity);
                    }
                }
            }

            {
                var requestFeatures = (IHttpRequestFeature)this;

                requestFeatures.Protocol = apiGatewayRequest.RequestContext.Http.Protocol;
                requestFeatures.Scheme = "https";
                requestFeatures.Method = apiGatewayRequest.RequestContext.Http.Method;

                var rawQueryString = Utilities.CreateQueryStringParametersFromHttpApiV2(apiGatewayRequest.RawQueryString);
                requestFeatures.RawTarget = apiGatewayRequest.RawPath + rawQueryString;
                requestFeatures.QueryString = rawQueryString ?? string.Empty;

                requestFeatures.Path = Utilities.DecodeResourcePath(apiGatewayRequest.RequestContext.Http.Path);
                if (!requestFeatures.Path.StartsWith('/'))
                {
                    requestFeatures.Path = "/" + requestFeatures.Path;
                }

                // If there is a stage name in the resource path strip it out and set the stage name as the base path.
                // This is required so that ASP.NET Core will route request based on the resource path without the stage name.
                requestFeatures.PathBase = string.Empty;
                if (!string.IsNullOrWhiteSpace(apiGatewayRequest.RequestContext.Stage) && requestFeatures.Path.StartsWith($"/{apiGatewayRequest.RequestContext.Stage}"))
                {
                    requestFeatures.Path = requestFeatures.Path.Substring(apiGatewayRequest.RequestContext.Stage.Length + 1);
                    requestFeatures.PathBase = $"/{apiGatewayRequest.RequestContext.Stage}";
                }

                // API Gateway HTTP API V2 format supports multiple values for headers by comma separating the values.
                if (apiGatewayRequest.Headers != null)
                {
                    foreach (var kvp in apiGatewayRequest.Headers)
                    {
                        requestFeatures.Headers[kvp.Key] = new StringValues(kvp.Value?.Split(','));
                    }
                }

                if (!requestFeatures.Headers.ContainsKey("Host"))
                {
                    requestFeatures.Headers["Host"] = apiGatewayRequest.RequestContext.DomainName;
                }

                if (apiGatewayRequest.Cookies != null)
                {
                    // Add Cookies from the event
                    requestFeatures.Headers["Cookie"] = string.Join("; ", apiGatewayRequest.Cookies);
                }

                if (!string.IsNullOrEmpty(apiGatewayRequest.Body))
                {
                    requestFeatures.Body = Utilities.ConvertLambdaRequestBodyToAspNetCoreBody(apiGatewayRequest.Body, apiGatewayRequest.IsBase64Encoded);
                }

                // Make sure the content-length header is set if header was not present.
                const string contentLengthHeaderName = "Content-Length";
                if (!requestFeatures.Headers.ContainsKey(contentLengthHeaderName))
                {
                    requestFeatures.Headers[contentLengthHeaderName] = requestFeatures.Body == null ? "0" : requestFeatures.Body.Length.ToString(CultureInfo.InvariantCulture);
                }
            }

            {
                var connectionFeatures = (IHttpConnectionFeature)this;

                connectionFeatures.ConnectionId = apiGatewayRequest.RequestContext.RequestId;

                if (!string.IsNullOrEmpty(apiGatewayRequest.RequestContext.Http.SourceIp) && IPAddress.TryParse(apiGatewayRequest.RequestContext.Http.SourceIp, out var remoteIpAddress))
                {
                    connectionFeatures.RemoteIpAddress = remoteIpAddress;
                }

                if (apiGatewayRequest.Headers?.TryGetValue("X-Forwarded-Port", out var port) == true)
                {
                    connectionFeatures.RemotePort = int.Parse(port, CultureInfo.InvariantCulture);
                }
            }

            {
                var serviceProviderFeatures = (IServiceProvidersFeature)this;

                serviceProviderFeatures.RequestServices = serviceProvider;
            }

            {
                var itemsFeatures = (IItemsFeature)this;

                itemsFeatures.Items[LAMBDA_CONTEXT] = lambdaContext;
                itemsFeatures.Items[LAMBDA_REQUEST_OBJECT] = apiGatewayRequest;
            }

            {
                var activityFeatures = (IHttpActivityFeature)this;

                activityFeatures.Activity = new Activity($"{apiGatewayRequest.RequestContext.Http.Method} {apiGatewayRequest.RequestContext.Http.Path}");
            }
        }

        #region IFeatureCollection
        public bool IsReadOnly => false;

        private readonly Dictionary<Type, object> _features = [];

        public int Revision => _containerRevision;

        public object? this[Type key]
        {
            get
            {
                if (_features.TryGetValue(key, out object? feature))
                {
                    return feature;
                }

                return null;
            }
            set
            {
                ArgumentNullException.ThrowIfNull(key);

                if (value == null)
                {
                    if (_features != null && _features.Remove(key))
                    {
                        _containerRevision++;
                    }
                    return;
                }

                _features[key] = value;
                _containerRevision++;
            }
        }

        public TFeature? Get<TFeature>()
        {
            if (_features.TryGetValue(typeof(TFeature), out object? feature))
            {
                return (TFeature)feature;
            }

            return default;
        }

        public IEnumerator<KeyValuePair<Type, object>> GetEnumerator()
        {
            return _features.GetEnumerator();
        }

        public void Set<TFeature>(TFeature? instance)
        {
            if (instance == null)
                return;

            _features[typeof(TFeature)] = instance;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _features.GetEnumerator();
        }

        #endregion

        #region IItemsFeature
        IDictionary<object, object?> IItemsFeature.Items { get; set; } = new Dictionary<object, object?>();
        #endregion

        #region IHttpAuthenticationFeature
        ClaimsPrincipal? IHttpAuthenticationFeature.User { get; set; }

        #endregion

        #region IHttpRequestFeature
        string IHttpRequestFeature.Protocol { get; set; } = string.Empty;

        string IHttpRequestFeature.Scheme { get; set; } = string.Empty;

        string IHttpRequestFeature.Method { get; set; } = string.Empty;

        string IHttpRequestFeature.PathBase { get; set; } = string.Empty;

        string IHttpRequestFeature.Path { get; set; } = string.Empty;

        string IHttpRequestFeature.QueryString { get; set; } = string.Empty;

        string IHttpRequestFeature.RawTarget { get; set; } = string.Empty;

        IHeaderDictionary IHttpRequestFeature.Headers { get; set; } = new HeaderDictionary();

        Stream IHttpRequestFeature.Body { get; set; } = new MemoryStream();

        #endregion

        #region IHttpResponseFeature
        int IHttpResponseFeature.StatusCode { get; set; } = 200;

        string? IHttpResponseFeature.ReasonPhrase { get; set; }

        bool IHttpResponseFeature.HasStarted { get; }

        IHeaderDictionary IHttpResponseFeature.Headers { get; set; } = new HeaderDictionary();

        Stream IHttpResponseFeature.Body { get; set; } = new MemoryStream();

        internal EventCallbacks? ResponseStartingEvents { get; private set; }
        void IHttpResponseFeature.OnStarting(Func<object, Task> callback, object state)
        {
            ResponseStartingEvents ??= new EventCallbacks();
            ResponseStartingEvents.Add(callback, state);
        }

        internal EventCallbacks? ResponseCompletedEvents { get; private set; }
        void IHttpResponseFeature.OnCompleted(Func<object, Task> callback, object state)
        {
            ResponseCompletedEvents ??= new EventCallbacks();
            ResponseCompletedEvents.Add(callback, state);
        }

        internal class EventCallbacks
        {
            private readonly List<EventCallback> _callbacks = [];

            internal void Add(Func<object, Task> callback, object state)
            {
                _callbacks.Add(new EventCallback(callback, state));
            }

            internal async Task ExecuteAsync()
            {
                foreach (var callback in _callbacks)
                {
                    await callback.ExecuteAsync();
                }
            }

            internal class EventCallback
            {
                internal EventCallback(Func<object, Task> callback, object state)
                {
                    Callback = callback;
                    State = state;
                }

                Func<object, Task> Callback { get; }
                object State { get; }

                internal Task ExecuteAsync()
                {
                    var task = Callback(State);
                    return task;
                }
            }
        }

        #endregion

        #region IHttpResponseBodyFeature
#pragma warning disable CS0618 // Type or member is obsolete
        Stream IHttpResponseBodyFeature.Stream => ((IHttpResponseFeature)this).Body;
#pragma warning restore CS0618 // Type or member is obsolete

        private PipeWriter? _pipeWriter;

        PipeWriter IHttpResponseBodyFeature.Writer
        {
            get
            {
                _pipeWriter ??= PipeWriter.Create(((IHttpResponseBodyFeature)this).Stream);
                return _pipeWriter;
            }
        }

        Task IHttpResponseBodyFeature.CompleteAsync() => Task.CompletedTask;

        void IHttpResponseBodyFeature.DisableBuffering()
        {
        }

        // This code is taken from the Apache 2.0 licensed ASP.NET Core repo.
        // https://github.com/aspnet/AspNetCore/blob/ab02951b37ac0cb09f8f6c3ed0280b46d89b06e0/src/Http/Http/src/SendFileFallback.cs
        async Task IHttpResponseBodyFeature.SendFileAsync(string filePath, long offset, long? count, CancellationToken cancellationToken)
        {
            var fileInfo = new FileInfo(filePath);
            if (offset < 0 || offset > fileInfo.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), offset, string.Empty);
            }
            if (count.HasValue &&
                (count.Value < 0 || count.Value > fileInfo.Length - offset))
            {
                throw new ArgumentOutOfRangeException(nameof(count), count, string.Empty);
            }

            cancellationToken.ThrowIfCancellationRequested();

            int bufferSize = 1024 * 16;

            var fileStream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read,
                FileShare.ReadWrite,
                bufferSize: bufferSize,
                options: FileOptions.Asynchronous | FileOptions.SequentialScan);

            using (fileStream)
            {
                fileStream.Seek(offset, SeekOrigin.Begin);
                await Utilities.CopyToAsync(fileStream, ((IHttpResponseBodyFeature)this).Stream, count, bufferSize, cancellationToken);
            }
        }

        Task IHttpResponseBodyFeature.StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        #endregion

        #region IHttpConnectionFeature

        string IHttpConnectionFeature.ConnectionId { get; set; } = string.Empty;

        IPAddress? IHttpConnectionFeature.RemoteIpAddress { get; set; }

        IPAddress? IHttpConnectionFeature.LocalIpAddress { get; set; }

        int IHttpConnectionFeature.RemotePort { get; set; }

        int IHttpConnectionFeature.LocalPort { get; set; }

        #endregion

        #region IServiceProvidersFeature

        IServiceProvider IServiceProvidersFeature.RequestServices { get; set; } = null!;

        #endregion

        #region ITlsConnectionFeatures

        public Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken) => Task.FromResult(ClientCertificate);

        public X509Certificate2? ClientCertificate { get; set; }

        #endregion

        #region IHttpRequestIdentifierFeature

        string? _traceIdentifier;
        string IHttpRequestIdentifierFeature.TraceIdentifier
        {
            get
            {
                if (_traceIdentifier != null)
                {
                    return _traceIdentifier;
                }

                var lambdaTraceId = EnvironmentVariables.TraceId;
                if (!string.IsNullOrEmpty(lambdaTraceId))
                {
                    return lambdaTraceId;
                }

                // If there is no Lambda trace id then fallback to the trace id that ASP.NET Core would have generated.
                _traceIdentifier = new HttpRequestIdentifierFeature().TraceIdentifier;
                return _traceIdentifier;
            }
            set { _traceIdentifier = value; }
        }

        #endregion
        bool IHttpRequestBodyDetectionFeature.CanHaveBody
        {
            get
            {
                var requestFeature = (IHttpRequestFeature)this;
                return requestFeature.Body != null && requestFeature.Body.Length > 0;
            }
        }

        Activity IHttpActivityFeature.Activity { get; set; } = null!;
    }
}
