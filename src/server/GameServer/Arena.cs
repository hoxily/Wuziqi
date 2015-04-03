using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace GameServer
{
    class Arena
    {
        public static readonly int AutoMatchWaitingTime;
        public static readonly int AutoMatchCheckInterval;
        private string m_player1;
        private string m_player2;
        private Dictionary<string, ClientServant> m_present;
        private Arena()
        {
            m_present = new Dictionary<string, ClientServant>();
        }
        static Arena()
        {
            AutoMatchCheckInterval = Convert.ToInt32(ConfigManager.GetConfig("GameServer.AutoMatchCheckInterval"));
            AutoMatchWaitingTime = Convert.ToInt32(ConfigManager.GetConfig("GameServer.AutoMatchWaitingTime"));
            m_waiting_queue_timer = new Timer(TimerEventCallback, m_waiting_queue_timer, TimeSpan.Zero, TimeSpan.FromSeconds(AutoMatchCheckInterval));
        }
        private static void TimerEventCallback(object state)
        {
            Queue<KeyValuePair<string, ClientServant>> outOfDate = new Queue<KeyValuePair<string, ClientServant>>();
            Queue<KeyValuePair<string, ClientServant>> matched = new Queue<KeyValuePair<string, ClientServant>>();
            lock(m_waiting_queue)
            {
                DateTime now = DateTime.Now;
                while(m_waiting_queue.Count > 0)
                {
                    Tuple<DateTime, KeyValuePair<string, ClientServant>> item = m_waiting_queue.Peek();
                    TimeSpan span = now - item.Item1;
                    if (span.TotalSeconds >= AutoMatchWaitingTime)
                    {
                        outOfDate.Enqueue(item.Item2);
                        m_waiting_queue.Dequeue();
                    }
                    else
                    {
                        break;
                    }
                }
                // 有可能残余1个等待请求在本次检查中没有匹配上。
                while(m_waiting_queue.Count >= 2)
                {
                    matched.Enqueue(m_waiting_queue.Dequeue().Item2);
                    matched.Enqueue(m_waiting_queue.Dequeue().Item2);
                }
            }
            foreach (KeyValuePair<string, ClientServant> kv in outOfDate)
            {
                kv.Value.ClearMatchingState();
                kv.Value.Location = "";
                kv.Value.PostMessage(new Message(null, "match", "no"));
            }
            outOfDate.Clear();
            while (matched.Count >= 2)
            {
                KeyValuePair<string, ClientServant> player1 = matched.Dequeue();
                KeyValuePair<string, ClientServant> player2 = matched.Dequeue();
                CreateArena(player1.Key, player1.Value, player2.Key, player2.Value);
            }
        }
        public static void Discuss(string arena_id, string uid, string message)
        {
            Arena arena;
            lock (Arenas)
            {
                if (Arenas.TryGetValue(arena_id, out arena))
                {
                    foreach (ClientServant cs in arena.m_present.Values)
                    {
                        cs.PostMessage(new Message(uid, "discuss", message));
                    }
                }
            }
        }
        public static void LayAt(string arena_id, string uid, int x, int y)
        {
            Arena arena;
            lock (Arenas)
            {
                if (Arenas.TryGetValue(arena_id, out arena))
                {
                    foreach (ClientServant cs in arena.m_present.Values)
                    {
                        cs.PostMessage(new Message(uid, "layat", x.ToString(), y.ToString()));
                    }
                }
            }
        }
        private static Timer m_waiting_queue_timer;
        private static TimeStampedQueue<KeyValuePair<string, ClientServant>> m_waiting_queue = new TimeStampedQueue<KeyValuePair<string,ClientServant>>();
        private static Dictionary<string, Arena> Arenas = new Dictionary<string, Arena>();
        public static void AddMatchRequest(KeyValuePair<string,ClientServant> request)
        {
            lock(m_waiting_queue)
            {
                m_waiting_queue.Enqueue(request);
            }
        }
        public static List<string> GetArenaList()
        {
            lock(Arenas)
            {
                return new List<string>(Arenas.Keys);
            }
        }
        public static void CreateArena(string player1_id, ClientServant player1_cs, string player2_id, ClientServant player2_cs)
        {
            Arena arena = new Arena();
            arena.m_player1 = player1_id;
            arena.m_player2 = player2_id;
            arena.m_present.Add(player1_id, player1_cs);
            arena.m_present.Add(player2_id, player2_cs);
            string arena_id = player1_id + "-vs-" + player2_id;
            player1_cs.Location = arena_id;
            player2_cs.Location = arena_id;
            player1_cs.ClearMatchingState();
            player2_cs.ClearMatchingState();
            lock(Arenas)
            {
                Arenas.Add(arena_id, arena);
            }
            player1_cs.PostMessage(new Message(player1_id, "participate", arena_id, "player1"));
            player2_cs.PostMessage(new Message(player2_id, "participate", arena_id, "player2")); 
        }
        public static bool JoinArena(string arena_id, ClientServant audience)
        {
            bool success = true;
            lock (Arenas)
            {
                if (Arenas.ContainsKey(arena_id))
                { 
                    foreach(ClientServant cs in Arenas[arena_id].m_present.Values)
                    {
                        cs.PostMessage(new Message(audience.ID, "join", arena_id));
                    }
                    Arenas[arena_id].m_present.Add(audience.ID, audience);
                    audience.Location = arena_id;
                }
                else
                {
                    success = false;
                }
            }
            return success;
        }
        public static void PartArena(string arena_id, ClientServant audience)
        {
            lock (Arenas)
            {
                Arena arena;
                if (Arenas.TryGetValue(arena_id, out arena))
                {
                    if (arena.m_player1 == audience.ID || arena.m_player2 == audience.ID)
                    {
                        foreach(KeyValuePair<string, ClientServant> kv in arena.m_present)
                        {
                            kv.Value.PostMessage(new Message(kv.Key, "part", arena_id, "arena shutdown"));
                            kv.Value.Location = "";
                        }
                        arena.m_present.Clear();
                        Arenas.Remove(arena_id);
                    }
                    else
                    {
                        if (arena.m_present.Remove(audience.ID))
                        {
                            audience.Location = "";
                            foreach (ClientServant cs in arena.m_present.Values)
                            {
                                cs.PostMessage(new Message(audience.ID, "part", arena_id));
                            }
                        }
                    }
                }
            }
        }
    }
}
