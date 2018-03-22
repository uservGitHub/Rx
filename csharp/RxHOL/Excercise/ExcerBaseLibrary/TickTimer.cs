using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace ExcerBaseLibrary
{
    public static class DelayTimer
    {
        private static Timer timer;
        private static int SPAN = 11;   //11毫秒误差
        private static long Begin = 0;
        private static int sumMs = 0;
        private static Stopwatch watch;
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
                    DispatcherHelper.Initialize();
                    if (currentTask == null)
                    {
                        Next();
                    }
                    else
                    {
                        DispatcherHelper.CheckBeginInvokeOnUI(currentTask.Item1);
                        //currentTask.Item1.Invoke();

                        if(Begin == 0)
                            Begin = watch.ElapsedMilliseconds;
                        
                        lock (delayLock)
                        {
                            delayList.Dequeue();
                        }
                        Next();
                    }
                }));
                
                watch = new Stopwatch();
            }
            if(delayList.Count == 1)
            {
                sumMs = 0;
                timer.Change(0, 0);
                //开始计时
                if (!watch.IsRunning)
                {
                    watch.Restart();
                    Console.WriteLine("Restart");
                }
            }
        }

        private static bool Next()
        {
            lock (delayLock)
            {
                if (delayList.Count > 0)
                {
                    currentTask = delayList.Peek();
                    //计算总时间，并调整累计时间
                    sumMs += currentTask.Item2;
                    timer.Change(sumMs - watch.ElapsedMilliseconds-SPAN, 0);

                    return true;
                }else
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    currentTask = null;
                    watch.Stop();
                    timer.Dispose();
                    timer = null;
                    Console.WriteLine("Dispose");
                }
                return false;
            }
        }
    }
}
