﻿using System;
using System.Collections.Generic;
using System.IO;
using Milochau.Core.Aws.Core.Runtime.Internal.Util;
using Milochau.Core.Aws.Core.Util;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>
    /// Default implementation of the IRequest interface.
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public class DefaultRequest : IRequest
    {
        Stream contentStream;
        string contentStreamHash;
        bool useQueryString = false;

        /// <summary>
        /// Constructs a new DefaultRequest with the specified service name and the
        /// original, user facing request object.
        /// </summary>
        /// <param name="request">The orignal request that is being wrapped</param>
        /// <param name="serviceName">The service name</param>
        public DefaultRequest(AmazonWebServiceRequest request, string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName)) throw new ArgumentNullException(nameof(serviceName));

            ServiceName = serviceName;
            OriginalRequest = request ?? throw new ArgumentNullException(nameof(request));
        }

        /// <summary>
        /// Gets and sets the type of http request to make, whether it should be POST,GET or DELETE
        /// </summary>
        public string HttpMethod { get; set; } = "POST";

        /// <summary>
        /// Gets and sets a flag that indicates whether the request is sent as a query string instead of the request body.
        /// </summary>
        public bool UseQueryString
        {
            get
            {
                if (HttpMethod == "GET")
                    return true;
                return useQueryString;
            }
            set
            {
                useQueryString = value;
            }
        }

        /// <summary>
        /// Returns the original, user facing request object which this internal
        /// request object is representing.
        /// </summary>
        public AmazonWebServiceRequest OriginalRequest { get; }

        /// <summary>
        /// Returns a dictionary of the headers included in this request.
        /// </summary>
        public IDictionary<string, string> Headers { get; } = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Gets and Sets the endpoint for this request.
        /// </summary>
        public Uri Endpoint { get; set; }

        /// <summary>
        /// Gets and Sets the resource path added on to the endpoint.
        /// </summary>
        public string ResourcePath { get; set; }

        /// <summary>
        /// Returns the path resources that should be used within the resource path.
        /// This is used for services where path keys can contain '/'
        /// characters, making string-splitting of a resource path potentially 
        /// hazardous.
        /// </summary>
        public IDictionary<string, string> PathResources { get; } = new Dictionary<string, string>(StringComparer.Ordinal);

        /// <summary>
        /// Adds a new entry to the PathResources collection for the request
        /// </summary>
        /// <param name="key">The name of the pathresource with potential greedy syntax: {key+}</param>
        /// <param name="value">Value of the entry</param>
        public void AddPathResource(string key, string value)
        {
            PathResources.Add(key, value);
        }

        /// <summary>
        /// Gets and Sets the content for this request.
        /// </summary>
        public byte[] Content { get; set; }

        /// <summary>
        /// Flag that signals that Content was and should be set
        /// from the Parameters collection.
        /// </summary>
        public bool SetContentFromParameters { get; set; }

        /// <summary>
        /// Gets and sets the content stream.
        /// </summary>
        public Stream ContentStream
        {
            get { return contentStream; }
            set
            {
                contentStream = value;
                OriginalStreamPosition = -1;
                if (contentStream != null)
                {
                    Stream baseStream = WrapperStream.GetNonWrapperBaseStream(contentStream);
                    if (baseStream.CanSeek)
                        OriginalStreamPosition = baseStream.Position;
                }
            }
        }

        /// <summary>
        /// Gets and sets the original stream position.
        /// If ContentStream is null or does not support seek, this propery
        /// should be equal to -1.
        /// </summary>
        public long OriginalStreamPosition { get; set; }

        /// <summary>
        /// Computes the SHA 256 hash of the content stream. If the stream is not
        /// seekable, it searches the parent stream hierarchy to find a seekable
        /// stream prior to computation. Once computed, the hash is cached for future
        /// use. If a suitable stream cannot be found to use, null is returned.
        /// </summary>
        public string ComputeContentStreamHash()
        {
            if (contentStream == null)
                return null;

            if (contentStreamHash == null)
            {
                var seekableStream = WrapperStream.SearchWrappedStream(contentStream, s => s.CanSeek);
                if (seekableStream != null)
                {
                    var position = seekableStream.Position;
                    byte[] payloadHashBytes = CryptoUtilFactory.CryptoInstance.ComputeSHA256Hash(seekableStream);
                    contentStreamHash = AWSSDKUtils.ToHex(payloadHashBytes, true);
                    seekableStream.Seek(position, SeekOrigin.Begin);
                }
            }

            return contentStreamHash;
        }

        /// <summary>
        /// The name of the service to which this request is being sent.
        /// </summary>
        public string ServiceName { get; }

        /// <summary>
        /// Checks if the request stream can be rewinded.
        /// </summary>
        /// <returns>Returns true if the request stream can be rewinded ,
        /// else false.</returns>
        public bool IsRequestStreamRewindable()
        {
            var stream = ContentStream;
            // Retries may not be possible with a stream
            if (stream != null)
            {
                // Pull out the underlying non-wrapper stream
                stream = WrapperStream.GetNonWrapperBaseStream(stream);

                // Retry is possible if stream is seekable
                return stream.CanSeek;
            }
            return true;
        }

        /// <summary>
        /// Returns true if the request can contain a request body, else false.
        /// </summary>
        /// <returns>Returns true if the currect request can contain a request body, else false.</returns>
        public bool MayContainRequestBody()
        {
            return HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH";
        }

        /// <summary>
        /// Returns true if the request has a body, else false.
        /// </summary>
        /// <returns>Returns true if the request has a body, else false.</returns>
        public bool HasRequestBody()
        {
            var isPutPost = HttpMethod == "POST" || HttpMethod == "PUT" || HttpMethod == "PATCH";
            var hasContent = this.HasRequestData();
            return isPutPost && hasContent;
        }
    }
}
