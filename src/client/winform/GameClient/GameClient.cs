using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace GameClient
{
    public delegate void OnMessageDelegate(Message msg);
    public enum ClientState
    {
        Initial, Verified, Logined, Matching
    }
    public enum PlayerType
    {
        Black, White, Audience
    }
    public class GameClient
    {
        public string ID = "";
        public string Location = "";
        public PlayerType PlayerType;
        public ClientState State;
        public OnMessageDelegate OnMessage
        {
            get;
            set;
        }
        private IPEndPoint m_ServerIPEndPoint;
        private string m_ProtocolVersion;
        private Thread m_ReadingThread;
        private Thread m_WritingThread;
        private Queue<Message> m_WritingQueue;
        private TcpClient m_TcpClient;
        public string ProtocolVersion
        {
            get
            {
                return m_ProtocolVersion;
            }
        }
        public void Init()
        {
            ConfigManager.LoadConfigs();
            m_ProtocolVersion = ConfigManager.GetConfig("GameClient.ProtocolVersion");
            m_ServerIPEndPoint = new IPEndPoint(IPAddress.Parse(ConfigManager.GetConfig("GameClient.ServerIP")), Convert.ToInt32(ConfigManager.GetConfig("GameClient.ServerPort")));
        }
        public void Start()
        {
            State = ClientState.Initial;
            m_TcpClient = new TcpClient();
            m_TcpClient.Connect(m_ServerIPEndPoint);
            m_ReadingThread = new Thread(Reading);
            m_ReadingThread.Start();
            m_WritingQueue = new Queue<Message>();
            m_WritingThread = new Thread(Writing);
            m_WritingThread.Start();
            PostMessage(new Message(null, "verify", m_ProtocolVersion));
        }
        public void PostMessage(Message msg)
        {
            lock(m_WritingQueue)
            {
                m_WritingQueue.Enqueue(msg);
                Monitor.Pulse(m_WritingQueue);
            }
        }
        public void Close()
        {
            PostMessage(null);
        }
        private void Writing()
        {
            List<Message> tmp = new List<Message>();
            bool continueFlag = true;
            while (continueFlag)
            {
                lock (m_WritingQueue) 
                {
                    if (tmp.Count == 0)
                    {
                        Monitor.Wait(m_WritingQueue);
                    }
                    if (m_WritingQueue.Count > 0)
                    {
                        tmp = m_WritingQueue.ToList();
                        m_WritingQueue.Clear();
                    }
                }
                foreach (Message msg in tmp)
                {
                    if (msg == null)
                    {
                        continueFlag = false;
                        break;
                    }
                    m_TcpClient.Client.Send(Encoding.UTF8.GetBytes(msg.ToString()));
                }
                tmp.Clear();
            }
            m_TcpClient.Close();
        }
        private void Reading()
        {
            StreamReader reader = new StreamReader(m_TcpClient.GetStream(), Encoding.UTF8);
            try
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    Message msg = Message.Parse(line);
                    if (OnMessage != null)
                    {
                        OnMessage(msg);
                    }
                    line = reader.ReadLine();
                }
            }
            catch(IOException e)
            {

            }
            reader.Close();
        }
    }
}
