﻿using Milochau.Core.Aws.Core.XRayRecorder.Core.Sampling;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// Represent the embedded trace header in HTTP request/response header
    /// </summary>
    public class TraceHeader
    {
        /// <summary>
        /// Field name for trace header in HTTP header
        /// </summary>
        public const string HeaderKey = "X-Amzn-Trace-Id";

        private const string RootKey = "Root";
        private const string ParentKey = "Parent";
        private const string SampledKey = "Sampled";

        private static readonly char[] _validSeparators = [';'];

        /// <summary>
        /// Gets or sets the trace id
        /// </summary>
        public string? RootTraceId { get; set; }

        /// <summary>
        /// Gets or sets the parent segment id
        /// </summary>
        public string? ParentId { get; set; }

        /// <summary>
        /// Gets or sets the sample decision
        /// </summary>
        public SampleDecision Sampled { get; set; }

        /// <summary>
        /// Convert the string representation of a trace header to an instance of <see cref="TraceHeader"/>.
        /// </summary>
        /// <param name="rawHeader">A string from HTTP request containing a trace header</param>
        /// <param name="header">When the method returns, contains the <see cref="TraceHeader"/> object converted from <paramref name="rawHeader"/>,
        /// if the conversion succeeded, or null if the conversion failed. The conversion fails if the <paramref name="rawHeader"/> is null or empty,
        /// is not of the correct format. This parameter is passed uninitialized; any value originally supplied will be overwritten.</param>
        /// <returns>true if <paramref name="rawHeader"/> converted successfully; otherwise, false. Root trace id
        /// required while parent id and sample decision is optional.</returns>
        public static bool TryParse(string? rawHeader, [NotNullWhen(true)] out TraceHeader? header)
        {
            header = null;

            try
            {
                if (string.IsNullOrEmpty(rawHeader))
                {
                    return false;
                }

                Dictionary<string, string> keyValuePairs = rawHeader.Split(_validSeparators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim().Split('='))
                    .ToDictionary(pair => pair[0], pair => pair[1]);


                // Root trace id is required
                if (!keyValuePairs.TryGetValue(RootKey, out string? tmpValue))
                {
                    return false;
                }

                if (!TraceId.IsIdValid(tmpValue))
                {
                    return false;
                }

                var result = new TraceHeader
                {
                    RootTraceId = tmpValue
                };

                // Parent id is optional
                if (keyValuePairs.TryGetValue(ParentKey, out tmpValue))
                {
                    if (!Entity.IsIdValid(tmpValue))
                    {
                        return false;
                    }

                    result.ParentId = tmpValue;
                }

                // Sample decision is optional
                if (keyValuePairs.TryGetValue(SampledKey, out tmpValue) && char.TryParse(tmpValue, out char tmpChar))
                {
                    if (Enum.IsDefined(typeof(SampleDecision), (int)tmpChar))
                    {
                        result.Sampled = (SampleDecision)tmpChar;
                    }
                }

                header = result;
                return true;
            }
            catch (IndexOutOfRangeException)
            {
                return false;
            }
        }

        /// <summary>
        /// Convert the string representation of a trace header to an instance of <see cref="TraceHeader"/>.
        /// </summary>
        /// <param name="rawHeader">A string from HTTP request containing a trace header</param>
        /// <param name="traceHeader">When the method returns, contains the <see cref="TraceHeader"/> object converted from <paramref name="rawHeader"/>,
        /// if the conversion succeeded, or null if the conversion failed. The conversion fails if the <paramref name="rawHeader"/> is null or empty,
        /// is not of the correct format. This parameter is passed uninitialized; any value originally supplied will be overwritten. RootId, ParentId 
        /// and Sampling decision are all required in valid form</param>
        /// <returns>true if <paramref name="rawHeader"/> converted successfully; otherwise, false.</returns>
        public static bool TryParseAll(string? rawHeader, out TraceHeader traceHeader)
        {
            traceHeader = FromString(rawHeader);
            return !string.IsNullOrEmpty(rawHeader) && !string.IsNullOrEmpty(traceHeader.RootTraceId) && !string.IsNullOrEmpty(traceHeader.ParentId) && traceHeader.Sampled != SampleDecision.Unknown;
        }

        /// <summary>
        /// Converts the string representation of a trace header to an instance of <see cref="TraceHeader"/>.
        /// </summary>
        /// <param name="rawHeader">A string from HTTP request containing a trace header.</param>
        /// <returns>Contains the <see cref="TraceHeader"/> object converted from <paramref name="rawHeader"/>,
        /// It only extracts non-null and valid values.</returns>
        public static TraceHeader FromString(string? rawHeader)
        {
            var result = new TraceHeader();
            try
            {
                if (string.IsNullOrEmpty(rawHeader))
                {
                    return result;
                }

                Dictionary<string, string> keyValuePairs = rawHeader.Split(_validSeparators, StringSplitOptions.RemoveEmptyEntries)
                    .Select(value => value.Trim().Split('='))
                    .ToDictionary(pair => pair[0], pair => pair[1]);


                if (keyValuePairs.TryGetValue(RootKey, out string? tmpValue) && TraceId.IsIdValid(tmpValue))
                {
                    result.RootTraceId = tmpValue;
                }

                if (keyValuePairs.TryGetValue(ParentKey, out tmpValue) && Entity.IsIdValid(tmpValue))
                {
                    result.ParentId = tmpValue;
                }

                if (keyValuePairs.TryGetValue(SampledKey, out tmpValue) && char.TryParse(tmpValue, out char tmpChar))
                {
                    if (Enum.IsDefined(typeof(SampleDecision), (int)tmpChar))
                    {
                        result.Sampled = (SampleDecision)tmpChar;
                    }
                }

                return result;
            }
            catch (IndexOutOfRangeException)
            {
                return result;
            }
        }


        /// <summary>
        /// Convert a Segment to an instance of <see cref="TraceHeader"/>.
        /// </summary>
        /// <param name="entity">A instance of <see cref="Entity"/> that will be used to convert to <see cref="TraceHeader"/>.</param>
        /// <param name="header">When the method returns, contains the <see cref="TraceHeader"/> object converted from <paramref name="entity"/>,
        /// if the conversion succeeded, or null if the conversion failed. The conversion fails if the <paramref name="entity"/> is null, or
        /// is not of the correct format. This parameter is passed uninitialized; any value originally supplied will be overwritten.</param>
        /// <returns>true if <paramref name="entity"/> converted successfully; otherwise, false.</returns>
        public static bool TryParse(Entity? entity, [NotNullWhen(true)] out TraceHeader? header)
        {
            header = null;
            if (entity == null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(entity.Id))
            {
                return false;
            }

            if (string.IsNullOrEmpty(entity.RootSegment?.TraceId))
            {
                return false;
            }

            var newHeader = new TraceHeader
            {
                // Trace id doesn't exist in subsegment, so get it from rootsegment
                RootTraceId = entity.RootSegment.TraceId,
                ParentId = entity.Id,
                Sampled = entity.Sampled
            };

            header = newHeader;
            return true;
        }

        /// <summary>
        /// Generate a string out of this instance of the class
        /// </summary>
        /// <returns>The string generated from current object</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(string.Format(CultureInfo.InvariantCulture, "{0}={1}", RootKey, RootTraceId));

            // Exclude parent id if the request is not sampled
            if (!string.IsNullOrEmpty(ParentId) && Sampled != SampleDecision.NotSampled)
            {
                sb.Append(string.Format(CultureInfo.InvariantCulture, "; {0}={1}", ParentKey, ParentId));
            }

            // Exclude sampled decision if the decision is unknown
            if (Sampled != SampleDecision.Unknown)
            {
                sb.Append(string.Format(CultureInfo.InvariantCulture, "; {0}={1}", SampledKey, Convert.ToChar(Sampled, CultureInfo.InvariantCulture)));
            }

            return sb.ToString();
        }
    }
}
