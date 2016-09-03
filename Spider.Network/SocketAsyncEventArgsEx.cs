using System;
using System.Net.Sockets;

namespace Spider.Network
{
    internal static class SocketAsyncEventArgsEx
    {
        public static void CopyTo(this SocketAsyncEventArgs args, byte[] bytes)
        {
            args.CopyTo(bytes, 0, args.BytesTransferred);
        }
        public static void CopyTo(this SocketAsyncEventArgs args,byte[] bytes,int offset)
        {
            args.CopyTo(bytes, offset, args.BytesTransferred);
        }

        public static void CopyTo(this SocketAsyncEventArgs args, byte[] bytes, int offset,int count)
        {
            Buffer.BlockCopy(args.Buffer, args.Offset, bytes, offset, count);
        }

        public static void SetData(this SocketAsyncEventArgs args, byte[] bytes)
        {
            args.SetData(bytes, 0, bytes.Length);
        }

        public static void SetData(this SocketAsyncEventArgs args, byte[] bytes, int offset, int count)
        {
            if (bytes == null) throw new ArgumentNullException("bytes");
            Buffer.BlockCopy(bytes, offset, args.Buffer, args.Offset, count);
            args.SetBuffer(args.Offset, count);
        }
    }
}
