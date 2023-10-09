using Milochau.Core.Aws.Core.Runtime.Internal.Transform;

namespace Milochau.Core.Aws.Core.Runtime.Internal
{
    /// <summary>
    /// Class containing the members used to invoke service calls
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public abstract class InvokeOptionsBase
    {
        private IMarshaller<IRequest, AmazonWebServiceRequest> _requestMarshaller;
        private ResponseUnmarshaller _responseUnmarshaller;
        
        protected InvokeOptionsBase()
        {
        }

        #region Standard Marshaller/Unmarshaller

        public virtual IMarshaller<IRequest, AmazonWebServiceRequest> RequestMarshaller
        {
            get
            {
                return _requestMarshaller;
            }
            set
            {
                _requestMarshaller = value;
            }
        }

        public virtual ResponseUnmarshaller ResponseUnmarshaller
        {
            get
            {
                return _responseUnmarshaller;
            }
            set
            {
                _responseUnmarshaller = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Class containing the members used to invoke service calls
    /// <para>
    /// This class is only intended for internal use inside the AWS client libraries.
    /// Callers shouldn't ever interact directly with objects of this class.
    /// </para>
    /// </summary>
    public class InvokeOptions : InvokeOptionsBase
    {
        public InvokeOptions() : base()
        {
        }
    }
}