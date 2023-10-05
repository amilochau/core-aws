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
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime.Internal.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Security;
using System.Threading;

namespace Amazon.Runtime
{
    // Credentials fallback mechanism
    public static class FallbackCredentialsFactory
    {
        // Lock to control caching credentials across multiple threads.
        private static ReaderWriterLockSlim cachedCredentialsLock = new ReaderWriterLockSlim();
    
        internal const string AWS_PROFILE_ENVIRONMENT_VARIABLE = "AWS_PROFILE";
        internal const string DefaultProfileName = "default";

        static FallbackCredentialsFactory()
        {
            Reset();
        }

        public delegate AWSCredentials CredentialsGenerator();

        public static void Reset()
        {
            Reset(null);
        }

        public static void Reset(IWebProxy proxy)
        {
            try
            {
                cachedCredentialsLock.EnterWriteLock();
                cachedCredentials = null;
            }
            finally
            {
                cachedCredentialsLock.ExitWriteLock();
            }
        }

        internal static string GetProfileName()
        {
            // @todo here to simplify
            return DefaultProfileName;
        }

        private static AWSCredentials cachedCredentials;
        public static AWSCredentials GetCredentials()
        {
            try
            {
                cachedCredentialsLock.EnterReadLock();
                if (cachedCredentials != null)
                {
                    return cachedCredentials;
                }
            }
            finally
            {
                cachedCredentialsLock.ExitReadLock();
            }
            
            try
            {
                cachedCredentialsLock.EnterWriteLock();
                if (cachedCredentials != null)
                {
                    return cachedCredentials;
                }
                
                List<Exception> errors = new List<Exception>();
                try
                {
                    cachedCredentials = new EnvironmentVariablesAWSCredentials();
                }
                // Breaking the FallbackCredentialFactory chain in case a ProcessAWSCredentialException exception 
                // is encountered. ProcessAWSCredentialException is thrown by the ProcessAWSCredential provider
                // when an exception is encountered when running a user provided process to obtain Basic/Session 
                // credentials. The motivation behind this is that, if the user has provided a process to be run
                // he expects to use the credentials obtained by running the process. Therefore the exception is
                // surfaced to the user.
                catch (ProcessAWSCredentialException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    cachedCredentials = null;

                    errors.Add(e);
                }

                if (cachedCredentials == null)
                {
                    throw new AmazonServiceException("Unable to find credentials");
                }

                return cachedCredentials;
            }
            finally
            {
                cachedCredentialsLock.ExitWriteLock();
            }
        }
    }
}
