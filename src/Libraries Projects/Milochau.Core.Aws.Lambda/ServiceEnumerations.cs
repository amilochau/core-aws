using Milochau.Core.Aws.Core.Runtime;

namespace Milochau.Core.Aws.Lambda
{
    /// <summary>
    /// Constants used for properties of type InvocationType.
    /// </summary>
    public class InvocationType : ConstantClass
    {

        /// <summary>
        /// Constant DryRun for InvocationType
        /// </summary>
        public static readonly InvocationType DryRun = new InvocationType("DryRun");
        /// <summary>
        /// Constant Event for InvocationType
        /// </summary>
        public static readonly InvocationType Event = new InvocationType("Event");
        /// <summary>
        /// Constant RequestResponse for InvocationType
        /// </summary>
        public static readonly InvocationType RequestResponse = new InvocationType("RequestResponse");

        /// <summary>
        /// This constant constructor does not need to be called if the constant
        /// you are attempting to use is already defined as a static instance of 
        /// this class.
        /// This constructor should be used to construct constants that are not
        /// defined as statics, for instance if attempting to use a feature that is
        /// newer than the current version of the SDK.
        /// </summary>
        public InvocationType(string value)
            : base(value)
        {
        }
    }
}