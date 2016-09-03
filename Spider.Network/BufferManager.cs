using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;

namespace Spider.Network
{
    class BufferManager
    {
        int _numBytes;
        byte[] _buffer;
        ConcurrentStack<int> _freeIndexPool;
        int _currentIndex;
        int _bufferSize;
        public BufferManager(int totalBytes, int bufferSize)
        {
            _numBytes = totalBytes;
            _currentIndex = 0;
            _bufferSize = bufferSize;
            _freeIndexPool = new ConcurrentStack<int>();
        }

        public void InitBuffer()
        {
            _buffer = new byte[_numBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            int freeIndex = -1;
            if (_freeIndexPool.TryPop(out freeIndex))
            {
                args.SetBuffer(_buffer, freeIndex, _bufferSize);
            }
            else
            {
                if ((_numBytes - _bufferSize) < _currentIndex)
                {
                    return false;
                }
                lock (this)
                {   
                    args.SetBuffer(_buffer, _currentIndex, _bufferSize);
                    _currentIndex += _bufferSize;
                }
            }
            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            _freeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }

    }

}
