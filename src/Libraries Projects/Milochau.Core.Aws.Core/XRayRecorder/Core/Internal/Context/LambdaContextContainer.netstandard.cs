using Amazon.XRay.Recorder.Core.Exceptions;
using Amazon.XRay.Recorder.Core.Internal.Entities;
using System;
using System.Threading;

namespace Amazon.XRay.Recorder.Core.Internal.Context
{
    /// <summary>
    /// This context is used in AWS Lambda environment.
    /// </summary>
    public class LambdaContextContainer : TraceContextImpl
    {
        private static AsyncLocal<Entity> _entityHolder = new AsyncLocal<Entity>();

        /// <summary>
        /// Get entity (segment/subsegment) from the context.
        /// </summary>
        /// <returns>The segment get from context.</returns>
        public override Entity GetEntity()
        {
            Entity entity = _entityHolder.Value;
            if (entity == null)
            {
                AWSXRayRecorder.Instance.AddFacadeSegment();
                entity = _entityHolder.Value;
            }

            return entity;
        }

        /// <summary>
        /// Set the specified entity (segment/subsegment) into context.
        /// </summary>
        /// <param name="entity">The segment to be set.</param>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to set.</exception>
        public override void SetEntity(Entity entity)
        {
            _entityHolder.Value = entity;
        }

        /// <summary>
        /// Clear entity from trace context for cleanup.
        /// </summary>
        public override void ClearEntity()
        {
            _entityHolder.Value = null;
        }

        /// <summary>
        /// Checks whether enity is present in <see cref="AsyncLocal{T}"/>.
        /// </summary>
        /// <returns>True if entity is present in <see cref="AsyncLocal{T}"/> else false.</returns>
        public override Boolean IsEntityPresent()
        {
            Entity entity = _entityHolder.Value;

            if (entity == null)
            {
                return false;
            }

            return true;
        }
    }
}
