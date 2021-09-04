using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Collections
{
    /// <summary>
    /// A key-value collection that evicts keys after they haven't been
    /// written to or read from for a certain amount of ticks.
    /// </summary>
    /// <typeparam name="TK">The key type</typeparam>
    /// <typeparam name="TV">The value type</typeparam>
    public sealed class Cache<TK, TV> : IDictionary<TK, TV>, IDisposable where TK : notnull
    {
        private readonly Dictionary<TK, Entry> _store = new();
        
        private readonly ITickSource _tickSource;
        private readonly ulong _expirationDelay;
        private readonly Dictionary<ulong, HashSet<TK>> _expirationStore = new();
        private ulong _now;

        /// <summary>
        /// The amount of entries.
        /// </summary>
        public int Count => _store.Count;
        /// <summary>
        /// The keys.
        /// </summary>
        public ICollection<TK> Keys => _store.Keys;
        bool ICollection<KeyValuePair<TK, TV>>.IsReadOnly => false;
        ICollection<TV> IDictionary<TK, TV>.Values => throw new NotSupportedException();
        
        /// <summary>
        /// Fired when an entry is evicted.
        /// </summary>
        public event Action<TK, TV>? EntryEvicted;

        public Cache(ITickSource tickSource, ulong expirationDelay)
        {
            _tickSource = tickSource;
            _expirationDelay = expirationDelay;

            _tickSource.Tick += Tick;
        }

        ~Cache()
        {
            _tickSource.Tick -= Tick;
        }

        public void Dispose()
        {
            _tickSource.Tick -= Tick;
            GC.SuppressFinalize(this);
        }

        private void Tick()
        {
            if (_expirationStore.Remove(_now, out var keys))
                foreach (var key in keys)
                    if (_store.Remove(key, out var val))
                        EntryEvicted?.Invoke(key, val.Value);

            _now++;
        }
        
        private void SetNewExpirationTime(TK key, Entry value, bool remove = true)
        {
            if (value.ExpirationTime == ulong.MaxValue)
                return;
            if (remove && _expirationStore.TryGetValue(value.ExpirationTime, out var keys))
            {
                keys.Remove(key);
                if (keys.Count == 0)
                    _expirationStore.Remove(value.ExpirationTime);
            }
            value.ExpirationTime = _now + _expirationDelay;
            if (!_expirationStore.TryGetValue(value.ExpirationTime, out keys))
                _expirationStore[value.ExpirationTime] = keys = new HashSet<TK>();
            keys.Add(key);
        }
        
        /// <summary>
        /// Adds a new key-value pair, optionally persisting it indefinitely.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <param name="persistIndefinitely">Whether to persist the KV pair indefinitely or not</param>
        public void Add(TK key, TV value, bool persistIndefinitely)
        {
            if (persistIndefinitely)
            {
                _store.Add(key, new Entry(value, ulong.MaxValue));
                return;
            }

            var exp = _now + _expirationDelay;
            _store.Add(key, new Entry(value, exp));
            if (!_expirationStore.TryGetValue(exp, out var keys))
                _expirationStore[exp] = keys = new HashSet<TK>();
            keys.Add(key);
        }
        
        /// <summary>
        /// Removes a key (and its value) from the cache.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>Whether the KV pair was removed</returns>
        public bool Remove(TK key)
        {
            return Remove(key, out _);
        }
        
        /// <summary>
        /// Removes a key (and its value) from the cache.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Whether the KV pair was removed</returns>
        public bool Remove(TK key, [MaybeNullWhen(false)] out TV value)
        {
            if (!_store.Remove(key, out var val))
            {
                value = default;
                return false;
            }
            if (_expirationStore.TryGetValue(val.ExpirationTime, out var keys))
            {
                keys.Remove(key);
                if (keys.Count == 0)
                    _expirationStore.Remove(val.ExpirationTime);
            }
            value = val.Value;
            return true;
        }

        /// <summary>
        /// Clears the cache.
        /// </summary>
        public void Clear()
        {
            _store.Clear();
            _expirationStore.Clear();
        }

        /// <summary>
        /// Forces a key to be persisted indefinitely.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>Whether the key exists or not</returns>
        public bool Persist(TK key)
        {
            if (!_store.TryGetValue(key, out var val))
                return false;
            if (val.ExpirationTime == ulong.MaxValue)
                return true;
            
            if (_expirationStore.TryGetValue(val.ExpirationTime, out var keys))
            {
                keys.Remove(key);
                if (keys.Count == 0)
                    _expirationStore.Remove(val.ExpirationTime);
            }
            val.ExpirationTime = ulong.MaxValue;
            return true;
        }

        /// <summary>
        /// Allows a key to expire.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>Whether the key exists or not</returns>
        public bool UnPersist(TK key)
        {
            if (!_store.TryGetValue(key, out var val))
                return false;

            val.ExpirationTime = 0;
            SetNewExpirationTime(key, val, false);
            return true;
        }

        /// <summary>
        /// Tries to get the value for a given key.
        /// </summary>
        /// <param name="key">The key</param>
        /// <param name="value">The value</param>
        /// <returns>Whether the key exists or not</returns>
        public bool TryGetValue(TK key, [MaybeNullWhen(false)] out TV value)
        {
            if (!_store.TryGetValue(key, out var val))
            {
                value = default!;
                return false;
            }

            SetNewExpirationTime(key, val);

            value = val.Value;
            return true;
        }

        /// <summary>
        /// A value.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>The value</returns>
        public TV this[TK key]
        {
            get => TryGetValue(key, out var value) ? value : throw new ArgumentException("Key not found.", nameof(key));
            set
            {
                if (_store.TryGetValue(key, out var val))
                {
                    val.Value = value;
                    SetNewExpirationTime(key, val);
                }
                else
                {
                    _store[key] = val = new Entry(value, 0);
                    SetNewExpirationTime(key, val, false);
                }
            }
        }

        /// <summary>
        /// Gets the enumerator for KV pairs.
        /// </summary>
        /// <returns>The enumerator</returns>
        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Checks whether a key is contained in the cache.
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>Whether the key was found or not</returns>
        public bool ContainsKey(TK key) => _store.ContainsKey(key);

        void IDictionary<TK, TV>.Add(TK key, TV value) => Add(key, value, false);
        void ICollection<KeyValuePair<TK, TV>>.Add(KeyValuePair<TK, TV> item) => Add(item.Key, item.Value, false);
        bool ICollection<KeyValuePair<TK, TV>>.Remove(KeyValuePair<TK, TV> item) => Remove(item.Key);
        bool ICollection<KeyValuePair<TK, TV>>.Contains(KeyValuePair<TK, TV> item) => _store.ContainsKey(item.Key);
        void ICollection<KeyValuePair<TK, TV>>.CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex) => throw new NotSupportedException();

        private sealed class Enumerator : IEnumerator<KeyValuePair<TK, TV>>
        {
            private readonly Cache<TK, TV> _cache;
            private readonly IEnumerator<KeyValuePair<TK, Entry>> _enumerator;

            public Enumerator(Cache<TK, TV> cache)
            {
                _cache = cache;
                _enumerator = _cache._store.GetEnumerator();
            }

            public bool MoveNext()
            {
                if (!_enumerator.MoveNext())
                    return false;
                var (key, value) = _enumerator.Current;
                Current = new KeyValuePair<TK, TV>(key, value.Value);
                _cache.SetNewExpirationTime(key, value);
                return true;
            }

            public void Reset() => _enumerator.Reset();

            public KeyValuePair<TK, TV> Current { get; private set; }

            object IEnumerator.Current => ((IEnumerator) _enumerator).Current!;

            public void Dispose() => _enumerator.Dispose();
        }

        private sealed class Entry
        {
            public TV Value { get; internal set; }
            public ulong ExpirationTime { get; internal set; }

            internal Entry(TV value, ulong expirationTime)
            {
                Value = value;
                ExpirationTime = expirationTime;
            }
        }
    }
}