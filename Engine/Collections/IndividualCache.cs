using System;
using DigBuild.Engine.Ticking;

namespace DigBuild.Engine.Collections
{
    public sealed class IndividualCache<T> : IDisposable
    {
        private readonly ITickSource _tickSource;
        private readonly ulong _expirationDelay;
        private ulong _now, _expiration;

        private T? _value;

        public bool HasValue { get; private set; } = false;

        public T Value
        {
            get
            {
                if (!HasValue)
                    throw new NullReferenceException();
                _expiration = _now + _expirationDelay;
                return _value!;
            }
            set
            {
                if (value != null)
                {
                    HasValue = true;
                    _value = value;
                    _expiration = _now + _expirationDelay;
                }
                else
                {
                    HasValue = false;
                    _expiration = 0;
                }
            }
        }

        public event Action<T>? Evicted;

        public IndividualCache(ITickSource tickSource, ulong expirationDelay)
        {
            _tickSource = tickSource;
            _expirationDelay = expirationDelay;

            _tickSource.Tick += Tick;
        }

        ~IndividualCache()
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
            if (_expiration == _now && HasValue)
            {
                HasValue = false;
                Evicted?.Invoke(_value!);
                _value = default!;
            }
            _now++;
        }
    }
}