using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExcerBaseLibrary;

/// <summary>
/// https://www.cnblogs.com/hayasi/p/6852474.html
/// 自定义控件
/// </summary>
namespace GxdApp1
{
    
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            DispatcherHelper.Initialize();
            InitializeComponent();
            timer = new DispatcherTimer(DispatcherPriority.ApplicationIdle);

            InitEvents();

            Closed += (a, b) =>
            {
                timer.Stop();
            };
        }
        private Stopwatch watch = new Stopwatch();
        private DispatcherTimer timer = null;

        private void InitEvents()
        {
            timer.Tick += (sender, e) =>
            {
                tbStartClock.Text = DateTime.Now.ToString(); ;
            };
            timer.Interval = TimeSpan.FromMilliseconds(250);
            timer.Start();

            btnDelayThreeSecond.Click += (sender, e) =>
            {
                watch.Restart();
                Delay(3000);
                Console.WriteLine("上面的Delay是异步的,{0}", watch.ElapsedMilliseconds);
            };

            btnTaskOrderExcute.Click += (sender, e) =>
            {
                watch.Restart();
                Task<int> t = new Task<int>(() =>
                {
                    Console.WriteLine("任务开始,{0}", watch.ElapsedMilliseconds);
                    //模拟任务执行
                    System.Threading.Thread.Sleep(3000);
                    return 100;
                });
                t.Start();
                t.ContinueWith(task =>
                {
                    Console.WriteLine("任务完成,{0}", watch.ElapsedMilliseconds);
                    Console.WriteLine("任务状态:IsCanceled={0}\tIsCompleted={1}\tIsFaulted={2}\tResult={3}", 
                        task.IsCanceled, task.IsCompleted, task.IsFaulted, task.Result);
                });
            };

            btnTaskCanCancel.Click += (sender, e) =>
            {
                //要防止未执行完毕的情况下，再次执行
                ((Button)sender).IsEnabled = false;
                //https://blog.csdn.net/djc11282/article/details/17524013
                CancellationTokenSource cts = new CancellationTokenSource();

                watch.Restart();
                Task<int> t = new Task<int>(() =>
                {
                    #region 任务
                    Console.WriteLine("任务开始,{0}", watch.ElapsedMilliseconds);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        btnCancelToken.IsEnabled = true;
                        btnCancelToken.Click += (subSender, subE) =>
                        {
                            btnCancelToken.IsEnabled = false;
                            cts.Cancel();
                        };
                    });

                    for (int i = 0; i < 8; i++)
                    {
                        if (cts.IsCancellationRequested)
                        {
                            return -1;
                        }
                        Thread.Sleep(1000);
                        Console.WriteLine("任务执行中...,{0}", watch.ElapsedMilliseconds);
                        DispatcherHelper.CheckBeginInvokeOnUI(() =>
                        {
                            btnCancelToken.Content = string.Format("{0}ms", i + 1);
                        });
                    }
                    Console.WriteLine("任务执行完毕,{0}", watch.ElapsedMilliseconds);
                    return 100;
                    #endregion
                }, cts.Token);
                t.Start();
                t.ContinueWith(task =>
                {
                    Console.WriteLine("任务完成,{0}", watch.ElapsedMilliseconds);
                    Console.WriteLine("任务状态:IsCanceled={0}\tIsCompleted={1}\tIsFaulted={2}\tResult={3}",
                        task.IsCanceled, task.IsCompleted, task.IsFaulted, task.Result);
                    DispatcherHelper.CheckBeginInvokeOnUI(() =>
                    {
                        ((Button)sender).IsEnabled = true;
                    });
                });
            };
        }

        private async void Delay(int ms)
        {
            await Task.Delay(ms);
            Console.WriteLine("延时{0}毫秒,{1}", ms, watch.ElapsedMilliseconds);            
        }
    }
}
