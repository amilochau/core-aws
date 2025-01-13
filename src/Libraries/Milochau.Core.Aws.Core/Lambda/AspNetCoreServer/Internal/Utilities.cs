using System;
using System.Buffers;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Core.Lambda.AspNetCoreServer.Internal
{
    /// <summary></summary>
    public static class Utilities
    {
        internal static Stream ConvertLambdaRequestBodyToAspNetCoreBody(string body, bool isBase64Encoded)
        {
            byte[] binaryBody;
            if (isBase64Encoded)
            {
                binaryBody = Convert.FromBase64String(body);
            }
            else
            {
                binaryBody = Encoding.UTF8.GetBytes(body);
            }

            return new MemoryStream(binaryBody);
        }

        internal static (string body, bool isBase64Encoded) ConvertAspNetCoreBodyToLambdaBody(Stream aspNetCoreBody, ResponseContentEncoding rcEncoding)
        {

            // Do we encode the response content in Base64 or treat it as UTF-8
            if (rcEncoding == ResponseContentEncoding.Base64)
            {
                // We want to read the response content "raw" and then Base64 encode it
                byte[] bodyBytes;
                if (aspNetCoreBody is MemoryStream stream)
                {
                    bodyBytes = stream.ToArray();
                }
                else
                {
                    using var ms = new MemoryStream();
                    aspNetCoreBody.CopyTo(ms);
                    bodyBytes = ms.ToArray();
                }
                return (body: Convert.ToBase64String(bodyBytes), isBase64Encoded: true);
            }
            else if (aspNetCoreBody is MemoryStream stream)
            {
                return (body: Encoding.UTF8.GetString(stream.ToArray()), isBase64Encoded: false);
            }
            else
            {
                aspNetCoreBody.Position = 0;
                using StreamReader reader = new StreamReader(aspNetCoreBody, Encoding.UTF8);
                return (body: reader.ReadToEnd(), isBase64Encoded: false);
            }
        }

        /// <summary>
        /// Add a '?' to the start of a non-empty query string, otherwise return null.
        /// </summary>
        /// <remarks>
        /// The ASP.NET MVC pipeline expects the query string to be URL-escaped.  Since the value in
        /// <see cref="APIGatewayHttpApiV2ProxyRequest.RawQueryString"/> should already be escaped this
        /// method does not perform any escaping itself.  This ensures identical behaviour when an MVC app
        /// is run through an API Gateway with this framework or in a standalone Kestrel instance.
        /// </remarks>
        /// <param name="queryString">URL-escaped query string without initial '?'</param>
        /// <returns></returns>
        public static string? CreateQueryStringParametersFromHttpApiV2(string? queryString)
        {
            if (string.IsNullOrEmpty(queryString))
            {
                return null;
            }

            return "?" + queryString;
        }

        // This code is taken from the Apache 2.0 licensed ASP.NET Core repo.
        // https://github.com/aspnet/AspNetCore/blob/d7bfbb5824b5f8876bcd4afaa29a611efc7aa1c9/src/Http/Shared/StreamCopyOperationInternal.cs
        internal static async Task CopyToAsync(Stream source, Stream destination, long? count, int bufferSize, CancellationToken cancel)
        {
            long? bytesRemaining = count;

            var buffer = ArrayPool<byte>.Shared.Rent(bufferSize);
            try
            {
                while (true)
                {
                    // The natural end of the range.
                    if (bytesRemaining.HasValue && bytesRemaining.GetValueOrDefault() <= 0)
                    {
                        return;
                    }

                    cancel.ThrowIfCancellationRequested();

                    int readLength = buffer.Length;
                    if (bytesRemaining.HasValue)
                    {
                        readLength = (int)Math.Min(bytesRemaining.GetValueOrDefault(), (long)readLength);
                    }
                    int read = await source.ReadAsync(buffer.AsMemory(0, readLength), cancel);

                    if (bytesRemaining.HasValue)
                    {
                        bytesRemaining -= read;
                    }

                    // End of the source stream.
                    if (read == 0)
                    {
                        return;
                    }

                    cancel.ThrowIfCancellationRequested();

                    await destination.WriteAsync(buffer.AsMemory(0, read), cancel);
                }
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(buffer);
            }
        }

        internal static string DecodeResourcePath(string resourcePath) => WebUtility.UrlDecode(resourcePath
            // Convert any + signs to percent encoding before URL decoding the path.
            .Replace("+", "%2B")
            // Double-escape any %2F (encoded / characters) so that they survive URL decoding the path.
            .Replace("%2F", "%252F")
            .Replace("%2f", "%252f"));
    }
}
