using System;
using System.Collections.Generic;
using System.Threading;

namespace Spider.Common
{
    public sealed class Pool<T> : IDisposable where T : IDisposable
    {
        private bool disposed = false;
        private Semaphore gate;
        private Stack<T> pool;
        private int minSize;
        private int total;
        private int maxSize;
        private Func<Pool<T>, T> creator;
        public event Predicate<T> Predicate;
        public Pool(int minPoolSize, int maxPoolSize,Func<Pool<T>,T> activator)
        {
            if (maxPoolSize <= 0)
            {
                throw new ArgumentException("maxPoolSize");
            }
            if (minPoolSize<0)
            {
                throw new ArgumentOutOfRangeException("minPoolSize");
            }
            if (minPoolSize>maxPoolSize)
            {
                throw new ArgumentException("maxPoolSize<minPoolSize");
            }
            minSize = minPoolSize;
            maxSize = maxPoolSize;
            if (activator==null)
            {
                throw new ArgumentNullException("activator");
            }
            creator = activator;
            gate = new Semaphore(minPoolSize, maxPoolSize);
            pool = new Stack<T>();
            for (int i = 0; i < minPoolSize; i++)
            {
                pool.Push(activator(this));
                total += 1;
            }
            
        }
        
        public void IncreaseCapacity(){
            if (total<maxSize&&pool.Count==0)
            {
                int len = CaclIncreaseLength();
                for (int i = 0; i < len; i++)
                {
                    pool.Push(creator(this));
                    gate.Release();
                } 
            }
        }

        private int CaclIncreaseLength()
        {
            int remain = maxSize - total;
            if (maxSize-total>minSize*2)
            {
                return minSize;
            }
            else
            {
                return remain;
            }
        }

        public T Acquire()
        {
            if (!gate.WaitOne())
                throw new InvalidOperationException();
            lock (pool)
            {
                IncreaseCapacity();
                return pool.Pop();
            }
        }

        public void Release(T target)
        {
            lock (pool)
            {
                if (Predicate!=null)
                {
                    if (Predicate(target))
                    {
                        pool.Push(target);
                        gate.Release();
                    }
                }
                else
                {
                    if (target != null)
                    {
                        pool.Push(target);
                        gate.Release();
                    }      
                }
            }
        }

        private void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                gate.Dispose();
            }
            for (int i = 0; i < pool.Count; i++)
            {
                var t = pool.Pop();
                t.Dispose();
            }
            disposed = true;
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
