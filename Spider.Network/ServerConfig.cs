using System.Collections.Generic;

namespace Spider.Network
{
    public class ServerConfig
    {
        public int MinConnection { get; set; }
        public int MaxConnection { get; set; }
        public int BufferSize { get; set; }
        public List<ListenerConfig> Listeners { get; set; }
    }

    public class ListenerConfig
    {
        private int _backlog = 100;
        public string IP { get; set; }
        public int Port { get; set; }
        public int Backlog { get { return _backlog; } set { _backlog = value; } }
    }
}
