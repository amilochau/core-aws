namespace Amazon.XRay.Recorder.Core.Internal.Entities
{
    /// <summary>
    /// A Facade segment tracks a period of time associated with a computation or action, along with annotations and key / value data.
    /// A set of trace segments all of which share the same tracing ID form a trace. This segment is created in AWS Lambda and only its subsegments are emitted.
    /// NOTE: This class should not be used. Its used internally by the SDK.
    /// </summary>
    /// <seealso cref="Entity" />
    public class FacadeSegment : Segment
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Segment"/> class.
        /// </summary>
        /// <param name="name">Name of the node or service component.</param>
        /// <param name="traceId">Unique id for the trace.</param>
        /// <param name="parentId">Unique id of the upstream segment.</param>
        public FacadeSegment(string name, string traceId, string parentId = null) : base(name,traceId,parentId)
        {
            Id = parentId;
            RootSegment = this;
        }
    }
}
