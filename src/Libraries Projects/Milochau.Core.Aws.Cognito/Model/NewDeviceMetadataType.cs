using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// The new device metadata type.
    /// </summary>
    public partial class NewDeviceMetadataType
    {
        /// <summary>
        /// Gets and sets the property DeviceGroupKey. 
        /// <para>
        /// The device group key.
        /// </para>
        /// </summary>
        public string? DeviceGroupKey { get; set; }

        /// <summary>
        /// Gets and sets the property DeviceKey. 
        /// <para>
        /// The device key.
        /// </para>
        /// </summary>
        public string? DeviceKey { get; set; }
    }
}
