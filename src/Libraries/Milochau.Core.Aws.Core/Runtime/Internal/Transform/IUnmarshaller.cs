namespace Milochau.Core.Aws.Core.Runtime.Internal.Transform
{
    /// <summary>
    /// Interface for unmarshallers which unmarshall objects from response data.
    /// The Unmarshallers are stateless, and only encode the rules for what data 
    /// in the XML stream goes into what members of an object. 
    /// </summary>
    /// <typeparam name="T">The type of object the unmarshaller returns</typeparam>
    /// <typeparam name="R">The type of the XML unmashaller context, which contains the
    /// state during parsing of the XML stream. Usually an instance of 
    /// <c>Amazon.Runtime.Internal.Transform.UnmarshallerContext</c>.</typeparam>
    public interface IUnmarshaller<T, R>
    {
        /// <summary>
        /// Given the current position in the XML stream, extract a T.
        /// </summary>
        /// <param name="input">The XML parsing context</param>
        /// <returns>An object of type T populated with data from the XML stream.</returns>
        T Unmarshall(R input);
    }
}
