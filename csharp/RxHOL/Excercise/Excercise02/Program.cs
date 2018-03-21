using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Reactive.Linq;

using ExcerBaseLibrary;

namespace Excercise02
{
    class Program
    {
        static void Main(string[] args)
        {
            var sources = GetSourcesFromExcercise2();
            var clock = new Stopwatch();
            clock.Restart();
            OnSubscribeToNEC(sources, clock, true);

            
        }

        /// <summary>
        /// Observable.Function(立即对象或异步执行返回)
        /// </summary>
        /// <returns></returns>
        static IList<Tuple<string, IObservable<int>>> GetSourcesFromExcercise2()
        {
            var list = new List<Tuple<string, IObservable<int>>>();
            list.Add(new Tuple<string, IObservable<int>>(
                "Never<int>()",
                Observable.Never<int>()
                ));
            list.Add(new Tuple<string, IObservable<int>>(
                "Empty<int>()",
                Observable.Empty<int>()
                ));
            list.Add(new Tuple<string, IObservable<int>>(
                "Throw<int>(new Exception(\"Oops\"))",
                Observable.Throw<int>(new Exception("Oops"))
                ));
            list.Add(new Tuple<string, IObservable<int>>(
                "Return(77)",
                Observable.Return<int>(77)
                ));
            list.Add(new Tuple<string, IObservable<int>>(
                "Range(10,3)",
                Observable.Range(10, 3)
                ));
            list.Add(new Tuple<string, IObservable<int>>(
                "Generate(0,i=>i<3,i=>i+1,i=>i*i)",
                Observable.Generate(0, i => i < 3, i => i + 1, i => i * i)
                ));
            //list.Add(new Tuple<string, IObservable<int>>(
            //    "Return(100,耗时100毫秒)",
            //    Observable.Return<int>(100)
            //    ));

            return list;
        }

        /// <summary>
        /// 对数据源顺序观察其执行情况：
        /// [=====> No.{序号/总数:D2}\t{主题}\t{源的数量}
        /// 方法:  {内容}\t{总运行时间}
        /// ...
        /// <======]\t{总运行时间}\n
        /// 按q跳出数据源的观察
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="Watch"></param>
        static void OnSubscribeToNEC(IEnumerable<Tuple<string, IObservable<int>>> sources, Stopwatch Watch, bool resetWatch=false)
        {
            //No.序号 主题
            const string tagFmt = "[=====> No.{0:D2}/{1:D2}\t{2}\t{3}";
            int i = 1;
            int count = sources.Count();

            Console.WriteLine(string.Format("==>遍历{0}个发射端:\n", count));
            foreach (var group in sources)
            {
                if (resetWatch) Watch.Restart();
                Console.WriteLine(tagFmt, i++, count, group.Item1, Watch.ElapsedMilliseconds);

                IDisposable subscription = group.Item2.Subscribe(
                x => Console.WriteLine("OnNext:  {0}\t{1}", x, Watch.ElapsedMilliseconds),
                ex => Console.WriteLine("OnError: {0}\t{1}", ex.Message, Watch.ElapsedMilliseconds),
                () => Console.WriteLine("OnCompleted\t{0}", Watch.ElapsedMilliseconds)
            );
                Console.WriteLine("<======]\t{0}\n", Watch.ElapsedMilliseconds);

                #region q跳出
                //Console.WriteLine("Press ENTER to unsubscribe...");
                //Console.ReadKey();
                var line = Console.In.ReadLine();
                if (line.Length == 1 && line.ToLower() == "q")
                {
                    break;
                }
                #endregion

                subscription.Dispose();
            }
        }

        /// <summary>
        /// DelayTimer的使用：
        /// 使用绝对时序顺序执行添加的任务
        /// </summary>
        static void DelayTimer_Sample()
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine(watch.ElapsedMilliseconds);
            for (int i = 0; i < 10; i++)
            {
                DelayTimer.Add(() =>
                {
                    Console.WriteLine(watch.ElapsedMilliseconds);
                }, 1000);
                System.Threading.Thread.Sleep(850);
            }

            System.Threading.Thread.Sleep(2000);

            for (int i = 0; i < 10; i++)
            {
                DelayTimer.Add(() =>
                {
                    Console.WriteLine(watch.ElapsedMilliseconds);
                }, 1000);
            }
            //Console.ReadKey();
        }
    }
}
