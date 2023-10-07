using Amazon.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.SESv2.Model.Internal.MarshallTransformations
{
    /// <summary>
    /// Response Unmarshaller for GenericException Object
    /// </summary>  
    public class GenericExceptionUnmarshaller : IErrorResponseUnmarshaller<GenericException, JsonUnmarshallerContext>
    {
        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <param name="context"></param>
        /// <returns></returns>
        public GenericException Unmarshall(JsonUnmarshallerContext context)
        {
            return this.Unmarshall(context, new Amazon.Runtime.Internal.ErrorResponse());
        }

        /// <summary>
        /// Unmarshaller the response from the service to the response class.
        /// </summary>  
        /// <returns></returns>
        public GenericException Unmarshall(JsonUnmarshallerContext context, Amazon.Runtime.Internal.ErrorResponse errorResponse)
        {
            context.Read();

            GenericException unmarshalledObject = new GenericException(errorResponse.Message, errorResponse.InnerException,
                errorResponse.Type, errorResponse.Code, errorResponse.RequestId, errorResponse.StatusCode);
        
            int targetDepth = context.CurrentDepth;
            while (context.ReadAtDepth(targetDepth))
            {
            }
          
            return unmarshalledObject;
        }

        /// <summary>
        /// Gets the singleton.
        /// </summary>  
        public static GenericExceptionUnmarshaller Instance { get; } = new GenericExceptionUnmarshaller();
    }
}