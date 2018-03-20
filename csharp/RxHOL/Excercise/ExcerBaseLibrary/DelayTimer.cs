using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace ExcerBaseLibrary
{
    public static class DelayTimerOld
    {
        private static DispatcherTimer timer;
        private static readonly object delayLock = new object();
        private static Queue<Tuple<Action, int>> delayList = new Queue<Tuple<Action, int>>();

        public static void Add(Action action, int ms)
        {
            lock (delayLock)
            {
                delayList.Enqueue(new Tuple<Action, int>(action, ms));
            }
            if (timer == null)
            {
                timer = new DispatcherTimer(DispatcherPriority.Normal);
            }
            Next();
            AutoStart();
        }
        private static void AutoStart()
        {
            if (timer != null && !timer.IsEnabled)
            {
                timer.Start();
            }
        }
        private static bool Next()
        {
            lock (delayLock)
            {
                if (delayList.Count > 0)
                {
                    var group = delayList.Dequeue();

                    timer.Interval = TimeSpan.FromMilliseconds(group.Item2);
                    timer.Tick += (sender, e) =>
                    {
                        Console.WriteLine("Tick");
                        group.Item1();

                        //Next();
                    };
                    return true;
                }
                return false;
            }
        }
        private static void TryStop()
        {
            if (timer != null && timer.IsEnabled)
            {
                lock (delayLock)
                {
                    if (delayList.Count == 0)
                    {
                        timer.Stop();
                    }
                }
            }
        }
    }
}
