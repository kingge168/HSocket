using Spider.Network;
using System;

namespace AsyncSocketHost
{
    class Program
    {
        static void Main(string[] args)
        {
            AsyncTcpHost host = new AsyncTcpHost();
            host.Start();
            Console.WriteLine("Press any key to terminate the server process....");
            Console.ReadKey();
            host.Stop();
        }
    }
}
