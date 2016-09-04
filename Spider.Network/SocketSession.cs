using Spider.Common;
using Spider.Data;
using Spider.Util;
using System;
using System.Net.Sockets;
using System.Text;

namespace Spider.Network
{
    internal class Sender
    {
        private byte[] _buffer;
        private readonly int _bufferSize;
        private int _total;
        private int _offset;
        public Sender(byte[] message, int bufferSize)
        {
            if (message == null || message.Length == 0)
            {
                throw new ArgumentException("message");
            }
            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException("bufferSize");
            }
            _bufferSize = bufferSize;
            InitBuffer(message);
        }

        private void InitBuffer(byte[] message)
        {
            int len = message.Length;
            _total = len + 10;
            var mark = Encoding.UTF8.GetBytes(len.ToString().PadLeft(10, '0'));
            _buffer = new byte[_total];
            Buffer.BlockCopy(mark, 0, _buffer, 0, 10);
            Buffer.BlockCopy(message, 0, _buffer, 10, len);
        }

        public bool IsCompleted
        {
            private set;
            get;
        }

        public void SetData(SocketAsyncEventArgs args)
        {
            if (_total > _bufferSize)
            {
                args.SetData(_buffer, _offset, _bufferSize);
                _total -= _bufferSize;
                _offset += _bufferSize;
                //IsCompleted = false;
            }
            else
            {
                args.SetData(_buffer, _offset, _total);
                IsCompleted = true;
            }
        }
    }

    internal class Receiver
    {
        public byte[] Buffer { get;private set; }

        public bool IsCompleted { get; private set; }

        private int _offset;
        public void ReceiveData(SocketAsyncEventArgs args)
        {
            if (Buffer==null)
            {
                if (args.BytesTransferred > 10)
                {
                    byte[] bytes = new byte[10];
                    args.CopyTo(bytes, 0, 10);
                    string str = Encoding.UTF8.GetString(bytes).TrimStart('0');
                    int len = 0;
                    if (int.TryParse(str, out len))
                    {
                        Buffer = new byte[len];
                        System.Buffer.BlockCopy(args.Buffer,args.Offset+10,Buffer, _offset, args.BytesTransferred - 10);
                        _offset = args.BytesTransferred - 10;
                    }
                    else
                    {
                        throw new NotSupportedException("Message format error.");
                    }                   
                }
                else
                {
                    throw new NotSupportedException("Message format error.");
                }
            }
            else
            {
                args.CopyTo(Buffer, _offset);
                _offset += args.BytesTransferred;   
            }
            IsCompleted = _offset >= Buffer.Length;
        }
    }

    internal sealed class SocketSession
    {
        private string _sessionID;
        private SocketAsyncEventArgs _args;
        private Socket _connection;
        private bool _isClosed;
        private int _bufferSize;
        Sender _sender;
        Receiver _receiver;

        public SocketSession(Socket connection, SocketAsyncEventArgs args, int bufferSize)
        {
            _sessionID = Guid.NewGuid().ToString();
            _args = args;
            _args.Completed += args_Completed;
            _bufferSize = bufferSize;
            _connection = connection;
            #if DEBUG
            Logger.InfoFormat("start a session,the sessionID is {0}.", _sessionID);
            #endif
        }

        public void Start()
        {
            _receiver = new Receiver();
            StartReceive(_args);
        }

        private void StartReceive(SocketAsyncEventArgs args)
        {
            bool willRaiseEvent = false;
            try
            {
                willRaiseEvent = _connection.ReceiveAsync(args);
            }
            catch (Exception ex)
            {
                LogError(ex);
                willRaiseEvent = true;
            }
            if (!willRaiseEvent)
            {
                ProcessReceive(args);
            }
        }

        private bool IsProcessCompleted(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                if (e.BytesTransferred > 0)
                {
                    return true;
                }
            }
            else
            {
                LogError((int)e.SocketError);
            }
            return false;
        }

        private bool IsIgnorableSocketError(int socketErrorCode)
        {
            if (socketErrorCode == 10004 //Interrupted
                || socketErrorCode == 10053 //ConnectionAborted
                || socketErrorCode == 10054 //ConnectionReset
                || socketErrorCode == 10058 //Shutdown
                || socketErrorCode == 10060 //TimedOut
                || socketErrorCode == 995 //OperationAborted
                || socketErrorCode == -1073741299)
            {
                return true;
            }

            return false;
        }

        private void LogError(int socketError)
        {
            if (!IsIgnorableSocketError(socketError))
            {
                SocketException ex = new SocketException(socketError);
                LogError(ex);
            }
        }

        private void LogError(Exception e)
        {
           Logger.Error(string.Format("the session {0} occurred a error:{1}", _sessionID, e.Message), e);
        }

        private void ProcessData()
        {
            byte[] bytes = CommandFactory.Process(SmartObject.Parse(Encoding.UTF8.GetString(_receiver.Buffer)));
            if (bytes != null)
            {
                _sender = new Sender(bytes, _bufferSize);
                #if DEBUG
                Logger.InfoFormat("sessionID:{0} process data success.", _sessionID);
                #endif
            }
            else
            {
                #if DEBUG
                Logger.InfoFormat("sessionID:{0} process data fail.", _sessionID);
                #endif
                throw new NotSupportedException("Does not match the command!");
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs args)
        {
            if (IsProcessCompleted(args))
            {
                try
                {
                    _receiver.ReceiveData(args);
                }
                catch (Exception e)
                {
                    LogError(e);
                    CloseClientSocket(args);
                }
                if (!_receiver.IsCompleted)
                {
                    StartReceive(args);
                }
                else
                {
                    try
                    {
                        ProcessData();
                        StartSend(args);
                    }
                    catch (Exception e)
                    {
                        LogError(e);
                    }  
                }
            }
            else
            {
                CloseClientSocket(args);
            }
        }


        private void StartSend(SocketAsyncEventArgs args)
        {
            bool willRaiseEvent = false;
            _sender.SetData(args);
            try
            {
                willRaiseEvent = _connection.SendAsync(args);
            }
            catch (Exception e)
            {
                willRaiseEvent = true;
                LogError(e);
                CloseClientSocket(args);
            }
            if (!willRaiseEvent)
            {
                ProcessSend(args);
            }
        }


        void args_Completed(object sender, SocketAsyncEventArgs e)
        {
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.Receive:
                    ProcessReceive(e);
                    break;
                case SocketAsyncOperation.Send:
                    ProcessSend(e);
                    break;
                default:
                    LogError(new NotSupportedException("The last operation completed on the socket was not a receive or send"));
                    CloseClientSocket(e);
                    break;
            }
        }
        private void ProcessSend(SocketAsyncEventArgs args)
        {
            if (IsProcessCompleted(args))
            {
                if (!_sender.IsCompleted)
                {
                    StartSend(args);
                }
                else
                {
                    #if DEBUG
                    Logger.InfoFormat("sessionID:{0} send success.", _sessionID);
                    #endif
                    Start();
                }
            }
            else
            {
                CloseClientSocket(args);
            }
        }

        private void CloseClientSocket(SocketAsyncEventArgs args)
        {
            if (!_isClosed)
            {
                lock (this)
                {
                    if (!_isClosed)
                    {
                        #if DEBUG
                        Logger.InfoFormat("sessionID:{0} is closing.", _sessionID);
                        #endif
                        _connection.SafeClose();
                        args.Completed -= args_Completed;
                        Pool<SocketAsyncEventArgs> pool = args.UserToken as Pool<SocketAsyncEventArgs>;
                        pool.Release(args);
                        _isClosed = true;
                    }
                }
            }
        }
    }
}
