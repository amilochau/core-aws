using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using Milochau.Core.Aws.Core.XRayRecorder.Core.Strategies;
using System;
using System.Runtime.ExceptionServices;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context
{
    public abstract class TraceContextImpl : ITraceContext
    {
        /// <summary>
        /// Get entity (segment/subsegment) from the trace context.
        /// </summary>
        /// <returns>The segment get from context</returns>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to get.</exception>
        public abstract Entity GetEntity();

        /// <summary>
        /// Set the specified entity (segment/subsegment) into trace context.
        /// </summary>
        /// <param name="entity">The segment to be set</param>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to set</exception>
        public abstract void SetEntity(Entity entity);

        /// <summary>
        /// Clear entity from trace context for cleanup.
        /// </summary>
        public abstract void ClearEntity();

        /// <summary>
        /// Checks whether enity is present in trace context.
        /// </summary>
        /// <returns>True if entity is present incontext container else false.</returns>
        public abstract bool IsEntityPresent();

        /// <summary>
        /// If the entity is missing from the context, the behavior is defined using <see cref="ContextMissingStrategy"/>
        /// </summary>
        /// <param name="recorder"><see cref="IAWSXRayRecorder"/> instance</param>
        /// <param name="e">Instance of <see cref="Exception"/></param>
        /// <param name="message">String message</param>
        public void HandleEntityMissing(IAWSXRayRecorder recorder, Exception e, string message)
        {
            if (recorder.ContextMissingStrategy == ContextMissingStrategy.LOG_ERROR)
            {
            }
            else
            {
                ExceptionDispatchInfo.Capture(e).Throw();
            }
        }
    }
}
