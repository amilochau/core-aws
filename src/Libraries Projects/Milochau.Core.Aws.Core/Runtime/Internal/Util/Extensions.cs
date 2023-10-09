namespace Milochau.Core.Aws.Core.Runtime.Internal.Util
{
    public static partial class Extensions
    {
        /// <summary>
        /// Returns true if the Content is set or there are
        /// query parameters.
        /// </summary>
        /// <param name="request">This request</param>
        /// <returns>True if data is present; false otherwise.</returns>
        public static bool HasRequestData(this IRequest request)
        {
            if (request == null)
                return false;

            if (request.ContentStream != null || request.Content != null)
                return true;

            return false;
        }
    }
}
