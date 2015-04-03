using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    class TimeStampedQueue<T>
    {
        private Queue<Tuple<DateTime, T>> timeStampedQueue;
        public TimeStampedQueue()
        {
            timeStampedQueue = new Queue<Tuple<DateTime, T>>();
        }
        public TimeStampedQueue(int capacity)
        {
            timeStampedQueue = new Queue<Tuple<DateTime, T>>(capacity);
        }
        public TimeStampedQueue(IEnumerable<Tuple<DateTime,T>> collection)
        {
            timeStampedQueue = new Queue<Tuple<DateTime, T>>(collection);
        }
        /// <summary>
        /// 进入队列（自动加上时间戳）
        /// </summary>
        /// <param name="item"></param>
        public void Enqueue(T item)
        {
            timeStampedQueue.Enqueue(new Tuple<DateTime, T>(DateTime.Now, item));
        }
        public Tuple<DateTime, T> Dequeue()
        {
            return timeStampedQueue.Dequeue();
        }
        public int Count
        {
            get
            {
                return timeStampedQueue.Count;
            }
        }
        public void Clear()
        {
            timeStampedQueue.Clear();
        }
        public Tuple<DateTime, T> Peek()
        {
            return timeStampedQueue.Peek();
        }
    }
}
