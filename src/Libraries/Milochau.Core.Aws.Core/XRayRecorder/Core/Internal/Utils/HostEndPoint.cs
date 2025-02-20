﻿using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Threading;

namespace Milochau.Core.Aws.Core.XRayRecorder.Core.Internal.Utils
{
    /// <summary>
    /// Represents a endpoint on some network.
    /// The represented endpoint is identified by a hostname.
    ///
    /// Internally resolves and caches an ip for the hostname.
    /// The ip is cached to keep the normal path speedy and non-blocking.
    /// </summary>
    /// <remarks>
    /// Create a HostEndPoint.
    /// </remarks>
    public class HostEndPoint(string host, int port, int cacheTtl = 60)
    {
        private IPEndPoint? _ipCache;
        private DateTime? _timestampOfLastIPCacheUpdate;
        private readonly ReaderWriterLockSlim cacheLock = new ReaderWriterLockSlim();

        /// <summary>
        /// Get the hostname that identifies the endpoint.
        /// </summary>
        public string Host { get; } = host;
        /// <summary>
        /// Get the port of the endpoint.
        /// </summary>
        public int Port { get; } = port;


        /// <summary>
        /// Check to see if the cache is valid.
        /// A lock with at least read access MUST be held when calling this method!
        /// </summary>
        /// <returns>true if the cache is valid, false otherwise.</returns>
        private CacheState IPCacheIsValid()
        {
            if (_ipCache == null)
            {
                return CacheState.Invalid;
            }

            if (_timestampOfLastIPCacheUpdate is not DateTime lastTimestamp)
            {
                return CacheState.Invalid;
            }

            if (DateTime.Now.Subtract(lastTimestamp).TotalSeconds < cacheTtl)
            {
                return CacheState.Valid;
            }

            return CacheState.Invalid;
        }

        /// <summary>
        /// Checks to see if the cache is valid.
        /// This method is essentially a wrapper around <see cref="IPCacheIsValid"/> that acquires the required lock.
        /// </summary>
        /// <returns>true if the cache is valid, false otherwise.</returns>
        private CacheState LockedIPCacheIsValid()
        {
            // If !entered => another thread holds a write lock, i.e. updating the cache
            if (!cacheLock.TryEnterReadLock(0)) return CacheState.Updating;
            try
            {
                return IPCacheIsValid();
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Returns a cached ip resolved from the hostname.
        /// If the cached address is invalid this method will try to update it.
        /// The IP address returned is never guaranteed to be valid.
        /// An IP address may be invalid to due to several factors, including but not limited to:
        ///  * DNS record is incorrect,
        ///  * DNS record might have changed since last update.
        /// The returned IPEndPoint may also be null if no cache update has been successful.
        /// </summary>
        /// <param name="updatePerformed">set to true if an update was performed, false otherwise</param>
        /// <returns>the cached IPEndPoint, may be null</returns>
        public IPEndPoint? GetIPEndPoint(out bool updatePerformed)
        {
            // LockedIPCacheIsValid and UpdateCache will in unison perform
            // a double checked locked to ensure:
            // 1. UpdateCache only is called when it appears that the cache is invalid
            // 2. The cache will not be updated when not necessary
            if (LockedIPCacheIsValid() == CacheState.Invalid)
            {
                updatePerformed = UpdateCache();
            }
            else
            {
                updatePerformed = false;
            }

            cacheLock.EnterReadLock();
            try
            {
                return _ipCache;
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Updates the cache if invalid.
        /// Utilises an upgradable read lock, meaning only one thread at a time can enter this method.
        /// </summary>
        private bool UpdateCache()
        {
            if (!cacheLock.TryEnterUpgradeableReadLock(0))
            {
                // Another thread is already performing an update => bail and use potentially dirty cache
                return false;
            }
            try
            {
                // We hold a UpgradableReadLock so when may call IPCacheIsValid
                if (IPCacheIsValid() != CacheState.Invalid)
                {
                    // Cache no longer invalid, i.e. another thread performed the update after us seeing it invalid
                    // and before now.
                    return false;
                }

                // We have confirmed that the cache still is invalid and needs updating.
                // We know that we are the only ones that may update it because we hold an UpgradeableReadLock
                // Only one thread may hold such lock at a time, see:
                // https://docs.microsoft.com/en-gb/dotnet/api/system.threading.readerwriterlockslim?view=netframework-4.7.2#remarks

                var ipEntries = Dns.GetHostAddresses(Host);
                var newIP = ipEntries.FirstOrDefault(x => x.AddressFamily == AddressFamily.InterNetwork);

                if (newIP == null)
                {
                    return false;
                }
                // Upgrade our read lock to write mode
                cacheLock.EnterWriteLock();
                try
                {
                    _timestampOfLastIPCacheUpdate = DateTime.Now;
                    _ipCache = new IPEndPoint(newIP, Port);
                    return true;
                }
                //Error catching for IPEndPoint creation
                catch (ArgumentNullException)
                {
                }
                catch (ArgumentOutOfRangeException)
                {
                }
                finally
                {
                    // Downgrade back to read mode
                    cacheLock.ExitWriteLock();
                }
            }
            //Error catching for DNS resolve
            catch (ArgumentNullException)
            {
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (SocketException)
            {
            }
            catch (ArgumentException)
            {
            }
            finally
            {
                cacheLock.ExitUpgradeableReadLock();
            }

            return false;
        }

        private enum CacheState
        {
            Valid,
            Invalid,
            Updating
        }
    }
}
