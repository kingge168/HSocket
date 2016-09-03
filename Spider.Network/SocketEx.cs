using System.Net.Sockets;

namespace Spider.Network
{
    internal static class SocketEx
    {
       public static void SafeClose(this Socket socket){
           try
           {
               socket.Shutdown(SocketShutdown.Send);
           }
           catch
           {

           }
           try
           {
               socket.Close();
           }
           catch
           {

           }
       }
    }
}
