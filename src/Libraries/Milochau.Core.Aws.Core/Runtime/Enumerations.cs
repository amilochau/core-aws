namespace Milochau.Core.Aws.Core.Runtime
{
    /// <summary>
    /// Which end of a request was responsible for a service error response.
    /// </summary>
    public enum ErrorType
    {
        /// <summary>
        /// The sender was responsible for the error, i.e. the client
        /// request failed validation or was improperly formatted.
        /// </summary>
        Sender,
        /// <summary>
        /// The error occured within the service.
        /// </summary>
        Receiver,
        /// <summary>
        /// An unrecognized error type was returned.
        /// </summary>
        Unknown
    }
}
