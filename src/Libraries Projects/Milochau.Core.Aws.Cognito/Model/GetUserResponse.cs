using Milochau.Core.Aws.Core.Runtime.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Represents the response from the server from the request to get information about
    /// the user.
    /// </summary>
    public partial class GetUserResponse : AmazonWebServiceResponse
    {
        ///// <summary>
        ///// Gets and sets the property MFAOptions. 
        ///// <para>
        /////  <i>This response parameter is no longer supported.</i> It provides information only
        ///// about SMS MFA configurations. It doesn't provide information about time-based one-time
        ///// password (TOTP) software token MFA configurations. To look up information about either
        ///// type of MFA configuration, use UserMFASettingList instead.
        ///// </para>
        ///// </summary>
        //public List<MFAOptionType>? MFAOptions { get; set; }

        ///// <summary>
        ///// Gets and sets the property PreferredMfaSetting. 
        ///// <para>
        ///// The user's preferred MFA setting.
        ///// </para>
        ///// </summary>
        //public string? PreferredMfaSetting { get; set; }

        /// <summary>
        /// Gets and sets the property UserAttributes. 
        /// <para>
        /// An array of name-value pairs representing user attributes.
        /// </para>
        ///  
        /// <para>
        /// For custom attributes, you must prepend the <code>custom:</code> prefix to the attribute
        /// name.
        /// </para>
        /// </summary>
        public List<AttributeType> UserAttributes { get; set; } = null!;

        ///// <summary>
        ///// Gets and sets the property UserMFASettingList. 
        ///// <para>
        ///// The MFA options that are activated for the user. The possible values in this list
        ///// are <code>SMS_MFA</code> and <code>SOFTWARE_TOKEN_MFA</code>.
        ///// </para>
        ///// </summary>
        //public List<string>? UserMFASettingList { get; set; }

        /// <summary>
        /// Gets and sets the property Username. 
        /// <para>
        /// The username of the user that you requested.
        /// </para>
        /// </summary>
        public string Username { get; set; } = null!;
    }
}
