using System;
using System.Collections.Generic;
using System.Threading;

namespace DigBuild.Engine.Collections
{
    /// <summary>
    /// A key-based semaphore manager.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    public sealed class LockStore<TKey> where TKey : notnull
    {
        private readonly Dictionary<TKey, LockHandle> _locks = new();

        /// <summary>
        /// Obtains a lock for the specified key. Waits if unavailable.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The lock handle</returns>
        public LockHandle Lock(TKey key)
        {
            LockHandle? handle;
            lock (_locks)
            {
                if (!_locks.TryGetValue(key, out handle))
                    _locks[key] = handle = new LockHandle(_locks, key);
            }
            handle.Wait();
            return handle;
        }

        /// <summary>
        /// A lock handle. Released when disposed of.
        /// </summary>
        public sealed class LockHandle : IDisposable
        {
            private readonly Dictionary<TKey, LockHandle> _locks;
            private readonly TKey _key;
            private readonly SemaphoreSlim _semaphore = new(1);

            internal LockHandle(Dictionary<TKey, LockHandle> locks, TKey key)
            {
                _locks = locks;
                _key = key;
            }

            internal void Wait()
            {
                _semaphore.Wait();
            }

            public void Dispose()
            {
                lock (_locks)
                {
                    var v = _semaphore.Release();
                    if (v == 0)
                        _locks.Remove(_key);
                }
            }
        }
    }
}