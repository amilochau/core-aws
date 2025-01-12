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

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal
{
    public class InvokeFeatures : IFeatureCollection,
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
                var authorizer = apiGatewayRequest.RequestContext.Authorizer;

                if (authorizer != null)
                {
                    // handling claims output from cognito user pool authorizer
                    if (authorizer.Jwt?.Claims != null && authorizer.Jwt.Claims.Count != 0)
                    {
                        var identity = new ClaimsIdentity(authorizer.Jwt.Claims.Select(x => new Claim(x.Key, x.Value.ToString())), "AuthorizerIdentity");
                        User = new ClaimsPrincipal(identity);
                    }
                    else
                    {
                        // handling claims output from custom lambda authorizer
                        var identity = new ClaimsIdentity(authorizer.Jwt?.Claims?.Select(x => new Claim(x.Key, x.Value)), "AuthorizerIdentity");
                        User = new ClaimsPrincipal(identity);
                    }
                }
            }

            {
                Protocol = apiGatewayRequest.RequestContext.Http.Protocol;
                Scheme = "https";
                Method = apiGatewayRequest.RequestContext.Http.Method;

                var rawQueryString = Utilities.CreateQueryStringParametersFromHttpApiV2(apiGatewayRequest.RawQueryString);
                RawTarget = apiGatewayRequest.RawPath + rawQueryString;
                QueryString = rawQueryString ?? string.Empty;

                Path = Utilities.DecodeResourcePath(apiGatewayRequest.RequestContext.Http.Path);
                if (!Path.StartsWith("/"))
                {
                    Path = "/" + Path;
                }

                // If there is a stage name in the resource path strip it out and set the stage name as the base path.
                // This is required so that ASP.NET Core will route request based on the resource path without the stage name.
                PathBase = string.Empty;
                var stageName = apiGatewayRequest.RequestContext.Stage;
                if (!string.IsNullOrWhiteSpace(stageName))
                {
                    if (Path.StartsWith($"/{stageName}"))
                    {
                        Path = Path.Substring(stageName.Length + 1);
                        PathBase = $"/{stageName}";
                    }
                }

                // API Gateway HTTP API V2 format supports multiple values for headers by comma separating the values.
                if (apiGatewayRequest.Headers != null)
                {
                    foreach (var kvp in apiGatewayRequest.Headers)
                    {
                        Headers[kvp.Key] = new StringValues(kvp.Value?.Split(','));
                    }
                }

                if (!Headers.ContainsKey("Host"))
                {
                    Headers["Host"] = apiGatewayRequest.RequestContext.DomainName;
                }

                if (apiGatewayRequest.Cookies != null)
                {
                    // Add Cookies from the event
                    Headers["Cookie"] = string.Join("; ", apiGatewayRequest.Cookies);
                }

                if (!string.IsNullOrEmpty(apiGatewayRequest.Body))
                {
                    Body = Utilities.ConvertLambdaRequestBodyToAspNetCoreBody(apiGatewayRequest.Body, apiGatewayRequest.IsBase64Encoded);
                }

                // Make sure the content-length header is set if header was not present.
                const string contentLengthHeaderName = "Content-Length";
                if (!Headers.ContainsKey(contentLengthHeaderName))
                {
                    Headers[contentLengthHeaderName] = Body == null ? "0" : Body.Length.ToString(CultureInfo.InvariantCulture);
                }
            }

            {
                ConnectionId = apiGatewayRequest.RequestContext.RequestId;

                if (!string.IsNullOrEmpty(apiGatewayRequest.RequestContext.Http.SourceIp) &&
                    IPAddress.TryParse(apiGatewayRequest.RequestContext.Http.SourceIp, out var remoteIpAddress))
                {
                    RemoteIpAddress = remoteIpAddress;
                }

                if (apiGatewayRequest.Headers?.TryGetValue("X-Forwarded-Port", out var port) == true)
                {
                    RemotePort = int.Parse(port, CultureInfo.InvariantCulture);
                }
            }

            {
                RequestServices = serviceProvider;
            }

            {
                Items[LAMBDA_CONTEXT] = lambdaContext;
                Items[LAMBDA_REQUEST_OBJECT] = apiGatewayRequest;
            }

            {
                Activity = new Activity($"{apiGatewayRequest.RequestContext.Http.Method} {apiGatewayRequest.RequestContext.Http.Path}");
            }
        }

        #region IFeatureCollection
        public bool IsReadOnly => false;

        private readonly IDictionary<Type, object> _features = new Dictionary<Type, object>();

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
        public IDictionary<object, object?> Items { get; set; } = new Dictionary<object, object?>();
        #endregion

        #region IHttpAuthenticationFeature
        public ClaimsPrincipal? User { get; set; }

        #endregion

        #region IHttpRequestFeature
        public string Protocol { get; set; }

        public string Scheme { get; set; }

        public string Method { get; set; }

        public string PathBase { get; set; }

        public string Path { get; set; }

        public string QueryString { get; set; }

        public string RawTarget { get; set; }

        public IHeaderDictionary Headers { get; set; } = new HeaderDictionary();

        public Stream Body { get; set; } = new MemoryStream();

        #endregion

        #region IHttpResponseFeature
        public int StatusCode { get; set; } = 200;

        public string? ReasonPhrase { get; set; }

        public bool HasStarted { get; }

        internal EventCallbacks? ResponseStartingEvents { get; private set; }
        public void OnStarting(Func<object, Task> callback, object state)
        {
            ResponseStartingEvents ??= new EventCallbacks();
            ResponseStartingEvents.Add(callback, state);
        }

        internal EventCallbacks? ResponseCompletedEvents { get; private set; }
        public void OnCompleted(Func<object, Task> callback, object state)
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
        public Stream Stream => Body;

        private PipeWriter? _pipeWriter;

        public PipeWriter Writer
        {
            get
            {
                _pipeWriter ??= PipeWriter.Create(((IHttpResponseBodyFeature)this).Stream);
                return _pipeWriter;
            }
        }

        public Task CompleteAsync() => Task.CompletedTask;

        public void DisableBuffering()
        {
        }

        // This code is taken from the Apache 2.0 licensed ASP.NET Core repo.
        // https://github.com/aspnet/AspNetCore/blob/ab02951b37ac0cb09f8f6c3ed0280b46d89b06e0/src/Http/Http/src/SendFileFallback.cs
        public async Task SendFileAsync(string filePath, long offset, long? count, CancellationToken cancellationToken)
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

        public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        #endregion

        #region IHttpConnectionFeature

        public string ConnectionId { get; set; }

        public IPAddress? RemoteIpAddress { get; set; }

        public IPAddress? LocalIpAddress { get; set; }

        public int RemotePort { get; set; }

        public int LocalPort { get; set; }

        #endregion

        #region IServiceProvidersFeature

        public IServiceProvider RequestServices { get; set; }

        #endregion

        #region ITlsConnectionFeatures

        public Task<X509Certificate2?> GetClientCertificateAsync(CancellationToken cancellationToken) => Task.FromResult(ClientCertificate);

        public X509Certificate2? ClientCertificate { get; set; }

        #endregion

        #region IHttpRequestIdentifierFeature

        string? _traceIdentifier;
        public string TraceIdentifier
        {
            get
            {
                if (_traceIdentifier != null)
                {
                    return _traceIdentifier;
                }

                var lambdaTraceId = Environment.GetEnvironmentVariable("_X_AMZN_TRACE_ID");
                if (!string.IsNullOrEmpty(lambdaTraceId))
                {
                    return lambdaTraceId;
                }

                // If there is no Lambda trace id then fallback to the trace id that ASP.NET Core would have generated.
                _traceIdentifier = new Microsoft.AspNetCore.Http.Features.HttpRequestIdentifierFeature().TraceIdentifier;
                return _traceIdentifier;
            }
            set { _traceIdentifier = value; }
        }

        #endregion
        public bool CanHaveBody => Body != null && Body.Length > 0;

        public Activity Activity { get; set; }
    }
}
