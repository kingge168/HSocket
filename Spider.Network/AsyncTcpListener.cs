using System;
using System.Net;
using System.Net.Sockets;

namespace Spider.Network
{
    delegate void ErrorHandler(AsyncTcpListener listener, Exception e);
    delegate void NewConectionHandler(AsyncTcpListener listener, Socket client);
    class AsyncTcpListener
    {
        private Socket _listenSocket;
        public event EventHandler Stopped;
        public event ErrorHandler Error;
        public event NewConectionHandler NewConection;
        private ListenerConfig _listener;
        public IPEndPoint EndPoint
        {
            private set;
            get;
        }
        public AsyncTcpListener(ListenerConfig listener, AsyncTcpHost host)
        {
            _listener = listener;
        }

        public void Start()
        {
            try
            {
                EndPoint = new IPEndPoint(_listener.IP.ToLower() == "any" ? IPAddress.Any : IPAddress.Parse(_listener.IP), _listener.Port);
                _listenSocket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);
                _listenSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, true);
                _listenSocket.Bind(EndPoint);
                _listenSocket.Listen(_listener.Backlog);
                SocketAsyncEventArgs acceptArg = new SocketAsyncEventArgs();
                acceptArg.Completed += Accept_Completed;
                if (!_listenSocket.AcceptAsync(acceptArg))
                {
                    ProcessAccept(acceptArg);
                }
            }
            catch (Exception e)
            {
                OnError(e);
            }
        }

        public void Stop()
        {
            if (_listenSocket != null)
            {
                lock (this)
                {
                    if (_listenSocket != null)
                    {
                        _listenSocket.SafeClose();
                        _listenSocket = null;
                        OnStop();
                    }
                }
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs acceptArg)
        {
            Socket socket = null;
            if (acceptArg.SocketError != SocketError.Success)
            {
                var errorCode = (int)acceptArg.SocketError;
                if (errorCode == 995 || errorCode == 10004 || errorCode == 10038)
                {
                    return;
                }
                OnError(new SocketException(errorCode));
            }
            else
            {
                socket = acceptArg.AcceptSocket;
            }
            acceptArg.AcceptSocket = null;
            bool willRaiseEvent = false;
            try
            {
                willRaiseEvent = _listenSocket.AcceptAsync(acceptArg);
            }
            catch (ObjectDisposedException)
            {
                willRaiseEvent = true;
            }
            catch (NullReferenceException)
            {
                willRaiseEvent = true;
            }
            catch (Exception e)
            {
                OnError(e);
                willRaiseEvent = true;
            }
            if (socket != null)
            {
                OnNewConnection(socket);
            }
            if (!willRaiseEvent)
            {
                ProcessAccept(acceptArg);
            }
        }
        private void Accept_Completed(object sender, SocketAsyncEventArgs e)
        {
            ProcessAccept(e);
        }

        private void OnError(Exception e)
        {
            if (Error != null)
            {
                Error(this, e);
            }
        }

        private void OnStop()
        {
            if (Stopped != null)
            {
                Stopped(this, EventArgs.Empty);
            }
        }

        private void OnNewConnection(Socket socket)
        {
            if (NewConection != null)
                NewConection(this, socket);
        }
    }
}
