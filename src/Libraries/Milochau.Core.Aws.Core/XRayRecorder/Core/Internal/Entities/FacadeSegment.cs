using Milochau.Core.Aws.Core.XRayRecorder.Core.Exceptions;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Emitters;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities
{
    /// <summary>
    /// A Facade segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace. This segment is created in AWS Lambda and only its subsegments are emitted.
    /// NOTE: This class should not be used. Its used internally by the SDK.
    /// </summary>
    public class FacadeSegment : Segment
    {
        //private static readonly string _mutationUnsupportedMessage = "FacadeSegments cannot be mutated.";

        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="name">Name of the node or service component.</param>
        /// <param name="traceId">Unique id for the trace.</param>
        /// <param name="parentId">Unique id of the upstream segment.</param>
        public FacadeSegment(string name, string? traceId, string? parentId = null) : base(name, traceId, parentId)
        {
            Id = parentId;
            RootSegment = this;
        }

        /*
        /// <summary>
        /// Unsupported for Facade segment. Returns always false.
        /// </summary>
        public new IDictionary<string, object>? Service => null;

        /// <summary>
        /// Unsupported for Facade segment. Returns always false.
        /// </summary>
        public new bool IsServiceAdded => false;

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetStartTime(decimal timestamp)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetEndTime(decimal timestamp)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetStartTime(DateTime timestamp)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetEndTime(DateTime timestamp)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void AddMetadata(string key, object value)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void AddException(Exception e)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void AddAnnotation(string key, object value)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetStartTimeToNow()
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void SetEndTimeToNow()
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated. Returns always null.
        /// </summary>
        [JsonIgnore]
        public new IDictionary<string, object>? Http => null;

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        [JsonIgnore]
        public new bool HasFault
        {
            get
            {
                return false;
            }
            set
            {
                throw new UnsupportedOperationException(_mutationUnsupportedMessage);
            }
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        [JsonIgnore]
        public new bool HasError
        {
            get
            {
                return false;
            }
            set
            {
                throw new UnsupportedOperationException(_mutationUnsupportedMessage);
            }
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        [JsonIgnore]
        public new bool IsThrottled
        {
            get
            {
                return false;
            }
            set
            {
                throw new UnsupportedOperationException(_mutationUnsupportedMessage);
            }
        }

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated. Returns always null.
        /// </summary>
        [JsonIgnore]
        public new IDictionary<string, string>? Sql => null;

        /// <summary>
        /// Unsupported as Facade segment cannot be mutated.
        /// </summary>
        /// <exception cref="UnsupportedOperationException">FacadeSegments cannot be mutated.</exception>
        public new void AddMetadata(string nameSpace, string key, object value)
        {
            throw new UnsupportedOperationException(_mutationUnsupportedMessage);
        }
        
        /// <summary>
        /// Unsupported as Facade segment cannot be mutated. Returns always false.
        /// </summary>
        public new bool IsHttpAdded => false;
        
        */

        /// <summary>
        /// Marshall the segment into JSON string
        /// </summary>
        /// <returns>The JSON string parsed from given segment</returns>
        internal override string Marshall()
        {
            var serializedEntity = JsonSerializer.Serialize(this, XRayJsonSerializerContext.Default.FacadeSegment);
            return ProtocolHeader + ProtocolDelimiter + serializedEntity;
        }
    }
}
