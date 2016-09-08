using System;
using System.Collections.Generic;
using System.Threading;

namespace Spider.Common
{
    public sealed class Pool<T> : IDisposable where T : IDisposable
    {
        private bool _disposed = false;
        private Semaphore _gate;
        private Stack<T> _pool;
        private int _minSize;
        private int _total;
        private int _maxSize;
        private Func<Pool<T>, T> _creator;
        public event Predicate<T> Predicate;

        public Pool(int minPoolSize, int maxPoolSize, Func<Pool<T>, T> activator)
        {
            if (maxPoolSize <= 0)
            {
                throw new ArgumentException("maxPoolSize");
            }
            if (minPoolSize < 0)
            {
                throw new ArgumentOutOfRangeException("minPoolSize");
            }
            if (minPoolSize > maxPoolSize)
            {
                throw new ArgumentException("maxPoolSize<minPoolSize");
            }
            _minSize = minPoolSize;
            _maxSize = maxPoolSize;
            if (activator == null)
            {
                throw new ArgumentNullException("activator");
            }
            _creator = activator;
            _gate = new Semaphore(maxPoolSize, maxPoolSize);
            _pool = new Stack<T>();
            for (int i = 0; i < minPoolSize; i++)
            {
                _pool.Push(activator(this));
            }
            _total += minPoolSize;
        }

        public void IncreaseCapacity()
        {
            if (_total < _maxSize && _pool.Count == 0)
            {
                int len = CaclIncreaseLength();
                for (int i = 0; i < len; i++)
                {
                    _pool.Push(_creator(this));
                }
                _total += len;
            }
        }

        private int CaclIncreaseLength()
        {
            int remain = _maxSize - _total;
            if (_maxSize - _total > _minSize * 2)
            {
                return _minSize;
            }
            else
            {
                return remain;
            }
        }

        public T Acquire()
        {
            if (!_gate.WaitOne())
                throw new InvalidOperationException();
            lock (_pool)
            {
                IncreaseCapacity();
                return _pool.Pop();
            }
        }

        public void Release(T target)
        {
            lock (_pool)
            {
                if (Predicate != null)
                {
                    if (Predicate(target))
                    {
                        _pool.Push(target);
                        _gate.Release();
                    }
                }
                else
                {
                    if (target != null)
                    {
                        _pool.Push(target);
                        _gate.Release();
                    }
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;
            if (disposing)
            {
                _gate.Dispose();
            }
            for (int i = 0; i < _pool.Count; i++)
            {
                var t = _pool.Pop();
                t.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~Pool()
        {
            Dispose(false);
        }
    }
}
