using System;
using System.Collections.Generic;
using System.Text;
using Spider.Common;
using Spider.Data;
using Spider.Util;
using System.Net.Sockets;

namespace Spider.Network
{
    public class AsyncTcpHost
    {
        private object Root = new object();
        private bool disposed;
        private BufferManager BufferManager { get; set; }
        private ServerConfig Config
        {
            get;
            set;
        }
        private Pool<SocketAsyncEventArgs> Pool { get; set; }

        private IList<AsyncTcpListener> listeners = new LineList<AsyncTcpListener>();

        public AsyncTcpHost(string configFileName): this(ConfigurationReader.Read<ServerConfig>(Encoding.UTF8,configFileName))
        {
        }

        public AsyncTcpHost(ServerConfig config)
        {
            ArgumentValidator.Validate("config", config, c => c == null);
            Config = config;
            BufferManager = new BufferManager(config.MaxConnection * config.BufferSize, config.BufferSize);
            try
            {
                BufferManager.InitBuffer();
            }
            catch (Exception e)
            {
                Logger.Error("Failed to allocate buffer for async socket communication, may because there is no enough memory, please decrease maxConnectionNumber in configuration!", e);
            }
            Pool = new Pool<SocketAsyncEventArgs>(config.MinConnection, config.MaxConnection, p =>
            {
                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                BufferManager.SetBuffer(args);
                return args;
            });
        }


        public void Start()
        {
            if (Config.Listeners != null && !disposed)
            {
                foreach (var item in Config.Listeners)
                {
                    var listener = new AsyncTcpListener(item, this);
                    listener.Stopped += Listener_Stopped;
                    listener.Error += Listener_Error;
                    listener.NewConection += Listener_NewConection;
                    listener.Start();
                    listeners.Add(listener);
                }
            }
        }

        private void Listener_NewConection(AsyncTcpListener listener, Socket client)
        {
            SocketAsyncEventArgs arg = Pool.Acquire();
            arg.UserToken = Pool;
            SocketSession session=new SocketSession(client,arg,Config.BufferSize);
            session.Start();
        }

        private void Listener_Stopped(object sender, EventArgs e)
        {
            AsyncTcpListener listener = sender as AsyncTcpListener;
            Logger.InfoFormat("{0} closed.", listener.EndPoint.ToString());
        }

        private void Listener_Error(AsyncTcpListener listener, Exception e)
        {
            Logger.Error(string.Format("{0} occurred a error:{1}", listener.EndPoint.ToString(), e.Message), e);
        }

        public void Stop()
        {
            if (!disposed)
            {
                lock (Root)
                {
                    if (!disposed)
                    {
                        foreach (var listener in listeners)
                        {
                            listener.Stop();
                        }
                        listeners.Clear();
                        Pool.Dispose();
                        BufferManager = null;
                        disposed = true;
                    }
                }
            }
        }
    }
}
