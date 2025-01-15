using Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Entities;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Context
{
    /// <summary>
    /// This context is used in AWS Lambda environment.
    /// </summary>
    public class LambdaContextContainer
    {
        private static readonly AsyncLocal<Entity?> _entityHolder = new();

        /// <summary>
        /// Get entity (segment/subsegment) from the context.
        /// </summary>
        /// <returns>The segment get from context.</returns>
        public Entity GetEntity()
        {
            Entity? entity = _entityHolder.Value;
            if (entity == null)
            {
                AWSXRayRecorder.Instance.AddFacadeSegment();
                entity = _entityHolder.Value;
            }

            return entity!;
        }

        /// <summary>
        /// Set the specified entity (segment/subsegment) into context.
        /// </summary>
        /// <param name="entity">The segment to be set.</param>
        /// <exception cref="EntityNotAvailableException">Thrown when the entity is not available to set.</exception>
        public void SetEntity(Entity entity)
        {
            _entityHolder.Value = entity;
        }

        /// <summary>
        /// Clear entity from trace context for cleanup.
        /// </summary>
        public void ClearEntity()
        {
            _entityHolder.Value = null;
        }

        /// <summary>
        /// Checks whether enity is present in <see cref="AsyncLocal{T}"/>.
        /// </summary>
        /// <returns>True if entity is present in <see cref="AsyncLocal{T}"/> else false.</returns>
        public bool IsEntityPresent()
        {
            return _entityHolder.Value != null;
        }
    }
}
