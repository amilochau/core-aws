//-----------------------------------------------------------------------------
// <copyright file="IPEndPointExtension.cs" company="Amazon.com">
//      Copyright 2017 Amazon.com, Inc. or its affiliates. All Rights Reserved.
//
//      Licensed under the Apache License, Version 2.0 (the "License").
//      You may not use this file except in compliance with the License.
//      A copy of the License is located at
//
//      http://aws.amazon.com/apache2.0
//
//      or in the "license" file accompanying this file. This file is distributed
//      on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either
//      express or implied. See the License for the specific language governing
//      permissions and limitations under the License.
// </copyright>
//-----------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.RegularExpressions;
using Amazon.Runtime.Internal.Util;

namespace Amazon.XRay.Recorder.Core.Internal.Utils
{
    /// <summary>
    /// Provides extension function to <see cref="System.Net.IPEndPoint"/>.
    /// </summary>
    public static class IPEndPointExtension
    {
        private const string Ipv4Address = @"^\d{1,3}.\d{1,3}.\d{1,3}.\d{1,3}:\d{1,5}$";
        private const char _addressDelimiter = ' '; // UDP and TCP address 
        private const char _addressPortDelimiter = ':';
        private const string _udpKey = "udp";
        private const string _tcpKey = "tcp";

        /// <summary>
        /// Validates that <paramref name="input"/> is an IP.
        /// </summary>
        /// <param name="input">Sting to be validated.</param>
        /// <returns>true if <paramref name="input"/> is an IP, false otherwise.</returns>
        public static bool IsIPAddress(string input)
        {
            try
            {
                // Validate basic format of IPv4 address
                return Regex.IsMatch(input, Ipv4Address, RegexOptions.None, TimeSpan.FromMinutes(1));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Tries to parse a string to <see cref="System.Net.IPEndPoint"/>.
        /// </summary>
        /// <param name="input">The input string. Must be able to be validated by <see cref="IsIPAddress"/>.</param>
        /// <param name="endPoint">The parsed IPEndPoint</param>
        /// <returns>true if <paramref name="input"/> converted successfully; otherwise, false.</returns>
        public static bool TryParse(string input, out IPEndPoint endPoint)
        {
            endPoint = null;

            string[] ep = input.Split(':');
            if (ep.Length != 2)
            {
                return false;
            }

            // Validate IP address is in valid range
            if (!IPAddress.TryParse(ep[0], out IPAddress ip))
            {
                return false;
            }

            if (!int.TryParse(ep[1], NumberStyles.Integer, NumberFormatInfo.InvariantInfo, out int port))
            {
                return false;
            }

            try
            {
                // Validate port number is in valid range
                endPoint = new IPEndPoint(ip, port);
                return true;
            }
            catch (ArgumentOutOfRangeException)
            {
                return false;
            }
        }
        
        
        /// <summary>
        /// Tries to parse a string to <see cref="HostEndPoint"/>
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="hostEndpoint">The parsed HostEndPoint</param>
        /// <returns>true if <paramref name="input"/> converted successfully; otherwise, false.</returns>
        public static bool TryParse(string input, out HostEndPoint hostEndpoint)
        {
            var entries = input.Split(':');
            if (entries.Length != 2)
            {
                hostEndpoint = null;
                return false;
            }
            if (!int.TryParse(entries[1], out var port))
            {
                hostEndpoint = null;
                return false;
            }
            if (port < 0 || 65535 < port)
            {
                hostEndpoint = null;
                return false;
            }
            
            /*
             * Almost anything can be a hostname which makes further validation here hard.
             * Accept any string in entries[0] and let it fail in the DNS lookup instead.
             */
            
            hostEndpoint = new HostEndPoint(entries[0], port);
            return true;
        }

        /// <summary>
        /// Tries to parse a string to <see cref="EndPoint"/>
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <param name="endpoint">The parsed EndPoint</param>
        /// <returns>true if <paramref name="input"/> converted successfully; otherwise, false.</returns>
        public static bool TryParse(string input, out EndPoint endpoint)
        {
            endpoint = null;
            if (IsIPAddress(input))
            {
                if (TryParse(input, out IPEndPoint ipEndPoint))
                {
                    endpoint = EndPoint.Of(ipEndPoint);
                    return true;
                }
            }
            else
            {
                if (TryParse(input, out HostEndPoint hostEndPoint))
                {
                    endpoint = EndPoint.Of(hostEndPoint);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Tries to parse a string to <see cref="DaemonConfig"/>.
        /// </summary>
        /// <param name="daemonAddress">The input string.</param>
        /// <param name="daemonEndPoint">The parsed <see cref="DaemonConfig"/> instance.</param>
        /// <returns></returns>
        public static bool TryParse(string daemonAddress, out DaemonConfig daemonEndPoint)
        {
            daemonEndPoint = null;

            if (string.IsNullOrEmpty(daemonAddress))
            {
                return false;
            }

            try
            {
              string[] ep = daemonAddress.Split(_addressDelimiter);
              return TryParseDaemonAddress(ep, out daemonEndPoint);
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static bool TryParseDaemonAddress(string[] daemonAddress, out DaemonConfig endPoint)
        {
            endPoint = null;
            if (daemonAddress.Length == 1)
            {
                return ParseSingleForm(daemonAddress, out endPoint);
            }
            else if (daemonAddress.Length == 2)
            {
                return ParseDoubleForm(daemonAddress, out endPoint);
            }

            return false;
        }

        private static bool ParseSingleForm(string[] daemonAddress, out DaemonConfig endPoint)
        {
            endPoint = new DaemonConfig();

            if (TryParse(daemonAddress[0], out EndPoint udpEndpoint))
            {
                endPoint._udpEndpoint = udpEndpoint;
                endPoint._tcpEndpoint = udpEndpoint;
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool ParseDoubleForm(string[] daemonAddress, out DaemonConfig endPoint)
        {
            endPoint = new DaemonConfig();
            IDictionary<string, string> addressMap = new Dictionary<string, string>();
            string[] address1 = daemonAddress[0].Split(_addressPortDelimiter); // tcp:<hostname or address>:2000 udp:<hostname or address>:2001
            string[] address2 = daemonAddress[1].Split(_addressPortDelimiter);

            addressMap[address1[0]] = address1[1] + _addressPortDelimiter + address1[2];
            addressMap[address2[0]] = address2[1] + _addressPortDelimiter + address2[2];
            string udpAddress = addressMap[_udpKey];
            string tcpAddress = addressMap[_tcpKey];
            if (TryParse(udpAddress, out EndPoint udpEndpoint) && TryParse(tcpAddress, out EndPoint tcpEndpoint))
            {
                endPoint._udpEndpoint = udpEndpoint;
                endPoint._tcpEndpoint = tcpEndpoint;
                return true;
            }

            return false;
        }
    }
}
