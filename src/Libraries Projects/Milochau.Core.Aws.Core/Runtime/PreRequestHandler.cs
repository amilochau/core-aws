using Milochau.Core.Aws.Core.Runtime.Internal;
using System;

namespace Milochau.Core.Aws.Core.Runtime
{
    public class PreRequestEventArgs : EventArgs
    {
        #region Constructor

        protected PreRequestEventArgs() { }
        
        #endregion

        #region Properties

        public AmazonWebServiceRequest Request { get; protected set; }

        #endregion

        #region Creator method

        internal static PreRequestEventArgs Create(AmazonWebServiceRequest request)
        {
            PreRequestEventArgs args = new PreRequestEventArgs
            {
                Request = request
            };
            return args;
        }

        #endregion
    }

    public delegate void PreRequestEventHandler(object sender, PreRequestEventArgs e);
}
