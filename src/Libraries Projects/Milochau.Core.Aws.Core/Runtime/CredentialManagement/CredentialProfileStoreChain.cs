/*
 * Copyright Amazon.com, Inc. or its affiliates. All Rights Reserved.
 *
 * Licensed under the Apache License, Version 2.0 (the "License").
 * You may not use this file except in compliance with the License.
 * A copy of the License is located at
 *
 *  http://aws.amazon.com/apache2.0
 *
 * or in the "license" file accompanying this file. This file is distributed
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
 * express or implied. See the License for the specific language governing
 * permissions and limitations under the License.
 */

using System.Collections.Generic;
using Amazon.Util.Internal;
using Amazon.Runtime.Internal.Settings;
using System.Linq;

namespace Amazon.Runtime.CredentialManagement
{
    /// <summary>
    /// Class to abstract the combined use of NetSDKCredentialsFile and SharedCredentialsFile where possible.
    /// </summary>
    /// <returns></returns>
    public class CredentialProfileStoreChain : ICredentialProfileSource
    {
        /// <summary>
        /// The location of the shared credentials file, or null to use the default location.
        /// </summary>
        public string ProfilesLocation { get; private set; }

        /// <summary>
        /// Construct a CredentialProfileChain.
        /// </summary>
        public CredentialProfileStoreChain()
            : this(null)
        {
        }

        /// <summary>
        /// Construct a CredentialProfileChain.
        /// </summary>
        /// <param name="profilesLocation">The path to the aws credentials file to look at.</param>
        public CredentialProfileStoreChain(string profilesLocation)
        {
            ProfilesLocation = profilesLocation;
        }

        /// <summary>
        /// <para>
        /// Try to get a <see cref="CredentialProfile"/> 
        /// </para>
        /// <para>
        /// If ProfilesLocation is non-null and non-empty search the shared credentials
        /// file at the disk path in the ProfilesLocation property.
        /// </para>
        /// <para>
        /// If ProfilesLocation is null or empty and the platform supports the .NET SDK credentials file
        /// search the SDK credentials file.  If the profile is not found search the shared credentials file in the default location.
        /// </para>
        ///<para>
        /// If ProfilesLocation is null or empty and the platform doesn't support the .NET SDK credentials file
        /// search the shared credentials file in the default location.
        /// </para>
        /// </summary>
        /// <param name="profileName">The name of the profile to get.</param>
        /// <param name="profile">The profile, if found</param>
        /// <returns>True if the profile was found, false otherwise.</returns>
        public bool TryGetProfile(string profileName, out CredentialProfile profile)
        {
            if (string.IsNullOrEmpty(ProfilesLocation) && UserCrypto.IsUserCryptAvailable)
            {
                var netCredentialsFile = new NetSDKCredentialsFile();
                if (netCredentialsFile.TryGetProfile(profileName, out profile))
                {
                    return true;
                }
            }

            var sharedCredentialsFile = new SharedCredentialsFile(ProfilesLocation);
            if (sharedCredentialsFile.TryGetProfile(profileName, out profile))
            {
                return true;
            }

            profile = null;
            return false;
        }
    }
}
