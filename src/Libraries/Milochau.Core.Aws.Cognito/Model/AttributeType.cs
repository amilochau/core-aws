﻿namespace Milochau.Core.Aws.Cognito.Model
{
    /// <summary>
    /// Specifies whether the attribute is standard or custom.
    /// </summary>
    public partial class AttributeType
    {
        /// <summary>
        /// Gets and sets the property Name. 
        /// <para>
        /// The name of the attribute.
        /// </para>
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Gets and sets the property Value. 
        /// <para>
        /// The value of the attribute.
        /// </para>
        /// </summary>
        public string? Value { get; set; }
    }
}
