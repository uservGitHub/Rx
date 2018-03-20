using System;
using System.Collections.Generic;
using System.Threading;

namespace ExcerBaseLibrary
{
    public static class DelayTimer
    {
        private static Timer timer;
        private static readonly object delayLock = new object();
        private static Tuple<Action, int> currentTask = null;
        private static Queue<Tuple<Action, int>> delayList = new Queue<Tuple<Action, int>>();

        public static void Add(Action action, int ms)
        {
            lock (delayLock)
            {
                delayList.Enqueue(new Tuple<Action, int>(action, ms));
            }
            if (timer == null)
            {
                timer = new Timer(new TimerCallback((holder) =>
                {
                    if (currentTask == null)
                    {
                        Next();
                    }
                    else
                    {
                        currentTask.Item1.Invoke();
                        lock (delayLock)
                        {
                            delayList.Dequeue();
                        }
                        Next();
                    }
                }));
            }
            if(delayList.Count == 1)
            {
                timer.Change(0, 0);
            }            
        }
        /// <summary>
        /// 假设定时器没有开启
        /// 动作：开启并清空
        /// </summary>
        private static void AutoStart()
        {
            if (timer != null && currentTask == null)
            {
                timer.Change(0, 0);
            }
        }
        private static bool Next()
        {
            lock (delayLock)
            {
                if (delayList.Count > 0)
                {
                    currentTask = delayList.Peek();
                    timer.Change(currentTask.Item2, 0);
                    return true;
                }else
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                }
                return false;
            }
        }
    }
}
