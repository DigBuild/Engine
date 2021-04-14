using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Utils
{
    public sealed class Cache<TK, TV> : IDictionary<TK, TV>, IDisposable where TK : notnull
    {
        private readonly Dictionary<TK, Entry> _store = new();
        
        private readonly ITickSource _tickSource;
        private readonly ulong _expirationDelay;
        private readonly Dictionary<ulong, HashSet<TK>> _expirationStore = new();
        private ulong _now;

        public int Count => _store.Count;
        public bool IsReadOnly => false;
        public ICollection<TK> Keys => _store.Keys;
        ICollection<TV> IDictionary<TK, TV>.Values => throw new NotSupportedException();
        
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
        
        public bool Remove(TK key)
        {
            if (!_store.Remove(key, out var value))
                return false;
            if (_expirationStore.TryGetValue(value.ExpirationTime, out var keys))
            {
                keys.Remove(key);
                if (keys.Count == 0)
                    _expirationStore.Remove(value.ExpirationTime);
            }
            return true;
        }

        public void Clear()
        {
            _store.Clear();
            _expirationStore.Clear();
        }

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

        public bool UnPersist(TK key)
        {
            if (!_store.TryGetValue(key, out var val))
                return false;

            val.ExpirationTime = 0;
            SetNewExpirationTime(key, val, false);
            return true;
        }

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

        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return new Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

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