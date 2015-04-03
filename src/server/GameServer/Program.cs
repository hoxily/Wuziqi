using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using log4net;
[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace GameServer
{
    class Program
    {
        public static ILog Logger;
        static void Main(string[] args)
        {
            Logger = LogManager.GetLogger("default");
            try
            {
                Logger.DebugFormat("Server powered up");
                Logger.DebugFormat("Loading configuration");
                ConfigManager.LoadConfigs();
                string addr = ConfigManager.GetConfig("GameServer.ListenAddress");
                string port = ConfigManager.GetConfig("GameServer.ListenPort");
                Logger.DebugFormat("Trying to listen at {0}:{1}", addr, port);
                TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Parse(addr), Convert.ToInt32(port)));
                listener.Start();
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientServant servant = new ClientServant(client);
                    Program.Logger.DebugFormat("New client connected from: {0}", servant.ClientIPEndPoint);
                    servant.Start();
                }
            }
            catch (Exception e)
            {
                Logger.FatalFormat("Unhandled exception: {0}, stacktrace: {1}", e.Message, e.StackTrace);
                Logger.Fatal("Server shutdown");
            }
        }
    }
}
