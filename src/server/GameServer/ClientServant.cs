using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;

namespace GameServer
{
    public enum ClientState
    {
        Initial, Verified, Logined, Matching
    }
    class ClientServant
    {
        private static Dictionary<string, ClientServant> m_onlineClients;
        private string m_id;
        public string ID
        {
            get
            {
                return m_id;
            }
        }
        private ClientState m_state;
        private TcpClient m_client;
        private Thread m_readingThread;
        private Thread m_writingThread;
        private Queue<Message> m_writingQueue;
        private IPEndPoint m_clientIPEndPoint;
        public string Location
        {
            get;
            set;
        }
        public void ClearMatchingState()
        {
            m_state = ClientState.Logined;
        }
        static ClientServant()
        {
            m_onlineClients = new Dictionary<string, ClientServant>();
        }
        public ClientServant(TcpClient client)
        {
            m_id = null;
            m_state = ClientState.Initial;
            m_client = client;
            m_readingThread = new Thread(Reading);
            m_writingQueue = new Queue<Message>();
            m_writingThread = new Thread(Writing);
            m_clientIPEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
        }
        public IPEndPoint ClientIPEndPoint
        {
            get
            {
                return m_clientIPEndPoint;
            }
        }
        public void Start()
        {
            m_readingThread.Start();
            m_writingThread.Start();
        }
        private void Writing()
        {
            NetworkStream ns = m_client.GetStream();
            StreamWriter writer = new StreamWriter(ns, Encoding.UTF8);
            bool keepOn = true;
            while(keepOn)
            {
                //if (m_writingQueue.Count == 0)
                //{
                //    Thread.Sleep(1000);
                //}
                //else
                //{
                //    Message msg;
                //    lock(m_writingQueue)
                //    {
                //        msg = m_writingQueue.Dequeue();
                //    }
                //    if (msg == null)
                //    {
                //        break;
                //    }
                //    writer.Write(msg.ToString());
                //    writer.Flush(); // 尽快将数据通过网络发送给对方而不是缓存在缓冲区里
                //}
                /*
                 * 改进等待模式。从固定的Sleep 1秒，变成等待通知唤醒。
                 * 在PostMessage中，对附加在m_writingQueue上的等待线程唤醒。
                 * 这样消息发送将没有不得已的1秒延迟了。
                 * */
                Queue<Message> tmp = new Queue<Message>();
                lock(m_writingQueue)
                {
                    if (m_writingQueue.Count == 0)
                    {
                        Monitor.Wait(m_writingQueue);
                    }
                    while(m_writingQueue.Count > 0)
                    {
                        tmp.Enqueue(m_writingQueue.Dequeue());
                    }
                }
                while(tmp.Count > 0)
                {
                    Message msg = tmp.Dequeue();
                    if(msg == null)
                    {
                        keepOn = false;
                        break;
                    }
                    writer.Write(msg.ToString());
                    writer.Flush();
                }
            }
            m_client.Close();
            if (m_state == ClientState.Logined)
            {
                lock(ClientServant.m_onlineClients)
                { 
                    ClientServant.m_onlineClients.Remove(m_id);
                }
                if (!string.IsNullOrEmpty(Location))
                {
                    Arena.PartArena(Location, this);
                }
                BroadcastMessage(new Message(m_id, "logout"));
            }
        }
        private void Reading()
        {
            NetworkStream ns = m_client.GetStream();
            StreamReader reader = new StreamReader(ns, Encoding.UTF8);
            try
            {
                string rawMessage = reader.ReadLine();
                while (rawMessage != null)
                {
                    Program.Logger.DebugFormat("REQUEST FROM {0} WITH {1}", ClientIPEndPoint, rawMessage);
                    Message msg = null;
                    try
                    {
                        msg = Message.Parse(rawMessage);

                    }
                    catch (Exception e)
                    {

                    }
                    if (msg != null)
                    {
                        ProcessMessage(msg);
                    }
                    rawMessage = reader.ReadLine();
                }
            }
            catch(IOException e)
            { }
            PostMessage(null); //通知写线程退出
            Program.Logger.DebugFormat("Connection closed by {0}", ClientIPEndPoint);
            reader.Close();
        }

        private void ProcessMessage(Message msg)
        {
            switch(msg.Command)
            {
                case "verify": // 版本验证
                    if (m_state == ClientState.Initial && msg.Parameters.Length == 1 && ConfigManager.GetConfig("GameServer.ProtocolVersion") == msg.Parameters[0])
                    {
                        m_state = ClientState.Verified;
                        Message response = new Message(null, "verify", "yes");
                        PostMessage(response);
                    }
                    else
                    {
                        PostMessage(new Message(null, "verify", "no"));
                    }
                    break;
                case "login": // 登录验证
                    if (m_state != ClientState.Verified)
                    {
                        break;
                    }
                    if (m_state == ClientState.Logined)
                    {
                        break;
                    }
                    if (msg.Parameters.Length == 1)
                    {
                        lock (ClientServant.m_onlineClients)
                        {
                            if (ClientServant.m_onlineClients.ContainsKey(msg.Parameters[0]))
                            {
                                PostMessage(new Message(null, "login", "no", "id in use"));
                            }
                            else
                            {
                                m_id = msg.Parameters[0];
                                ClientServant.m_onlineClients.Add(m_id, this);
                                ClientServant.BroadcastMessage(new Message(m_id, "login", "yes"));
                                m_state = ClientState.Logined;
                            }
                        }
                    }
                    break;
                case "speak": // 世界范围喊话
                    if(m_state != ClientState.Logined && m_state != ClientState.Matching)
                    {
                        PostMessage(new Message(null, "speak", "no", "need logined"));
                    }
                    else
                    {
                        if (msg.Parameters.Length == 1)
                        {
                            BroadcastMessage(new Message(m_id, "speak", msg.Parameters[0]));
                        }
                    }
                    break;
                case "whisper": // 私聊密语
                    if (m_state != ClientState.Logined && m_state != ClientState.Matching)
                    {
                        PostMessage(new Message(null, "whisper", "no", "need logined"));
                    }
                    else
                    {
                        if (msg.Parameters.Length == 2)
                        {
                            string targetId = msg.Parameters[0];
                            ClientServant cs;
                            lock (ClientServant.m_onlineClients)
                            {
                                if (ClientServant.m_onlineClients.TryGetValue(targetId, out cs))
                                {
                                    cs.PostMessage(new Message(m_id, "whisper", msg.Parameters[1]));
                                }
                                else
                                {
                                    PostMessage(new Message(null, "whisper", "no", "user id not exist"));
                                }
                            }
                        }
                    }
                    break;
                case "match": // 自动匹配
                    if (m_state != ClientState.Logined)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(Location))
                    {
                        // 注意两条语句顺序，因为执行AddMatchRequest方法有可能因为匹配上而清除ClientState.Matching状态
                        m_state = ClientState.Matching;
                        Arena.AddMatchRequest(new KeyValuePair<string, ClientServant>(ID, this));
                    }
                    break;
                case "join": // 观众加入竞技场观战
                    if (m_state != ClientState.Logined)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(Location))
                    {
                        if(Arena.JoinArena(msg.Parameters[0], this))
                        {
                            PostMessage(new Message(ID, "join", msg.Parameters[0]));
                        }
                    }
                    break;
                case "part": // 观众或者参战者离开竞技场
                    if (m_state != ClientState.Logined)
                    {
                        break;
                    }
                    if (string.IsNullOrEmpty(Location))
                    {
                        break;
                    }
                    Arena.PartArena(msg.Parameters[0], this);
                    break;
                case "list": // 当前竞技场列表
                    if (m_state == ClientState.Logined && string.IsNullOrEmpty(Location))
                    {
                        Message response = new Message(null, "list", Arena.GetArenaList().ToArray());
                        PostMessage(response);
                    }
                    break;
                case "discuss": // 在竞技场内的讨论
                    if (m_state == ClientState.Logined && !string.IsNullOrEmpty(Location) && msg.Parameters.Length == 1)
                    {
                        Arena.Discuss(Location, ID, msg.Parameters[0]);
                    }
                    break;
                case "layat": // 放置棋子
                    if (m_state == ClientState.Logined && !string.IsNullOrEmpty(Location) && msg.Parameters.Length == 2)
                    {
                        int x = Convert.ToInt32(msg.Parameters[0]);
                        int y = Convert.ToInt32(msg.Parameters[1]);
                        Arena.LayAt(Location, ID, x, y);
                    }
                    break;
            }
        }

        private static void BroadcastMessage(Message message)
        {
            lock(m_onlineClients)
            {
                foreach(ClientServant c in m_onlineClients.Values)
                {
                    c.PostMessage(message);
                }
            }
        }

        public void PostMessage(Message response)
        {
            lock(m_writingQueue)
            {
                m_writingQueue.Enqueue(response);
                Program.Logger.DebugFormat("RESPONSE TO {0} WITH {1}", m_clientIPEndPoint, response);
                Monitor.Pulse(m_writingQueue);
            }
        }
    }
}
