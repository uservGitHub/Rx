using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows.Forms;
using ExcerBaseLibrary;

namespace Excercise03
{
    class Program
    {
        [STAThread]
        static void MainHold(string[] args)
        {
            //AppDomain domain1 = AppDomain.CreateDomain("1st");
            //string dlFile = @"C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.6.1\PresentationFramework.dll";
            //var handle = domain1.CreateInstance(dlFile, "System.Windows.Window");
            //System.Windows.Window win =
            //    (System.Windows.Window)handle.Unwrap();
            //Console.WriteLine("win is " + ((win == null) ? "null" : "object"));

            //一个应用程序域只能有一个
            System.Windows.Application app = new System.Windows.Application();
            app.ShutdownMode = System.Windows.ShutdownMode.OnMainWindowClose;

            Console.WriteLine("2种方式:");
            #region Event
            {
                Console.WriteLine("Event方式:");
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    Console.WriteLine("开启Form");
                    Main_Form_Event();    //阻塞方式
                });
                thread.Start();

                //Console.WriteLine("开启WPF");
                //Main_WPF_Event(app); //阻塞方式

                thread.Join();
            }
            #endregion

            //System.Threading.Thread.Sleep(3000);

            #region Observable.FromEventPattern
            {
                Console.WriteLine("Observable.FromEventPattern方式:");
                System.Threading.Thread thread = new System.Threading.Thread(() =>
                {
                    Console.WriteLine("开启Form");
                    Main_Form_Pattern();    //阻塞方式
                });
                thread.Start();

                Console.WriteLine("开启WPF");
                Main_WPF_Pattern(app); //阻塞方式

                thread.Join();
            }
            #endregion
            Console.WriteLine("按任意键退出!");
            Console.ReadKey();
        }

        static void Main_Form_Event()
        {
            var lb1 = new Label();
            var txt = new TextBox();
            var frm = new Form()
            {
                Controls =
                {
                    lb1,
                    txt //frm.Controls.Add(txt);
                }
            };

            frm.MouseMove += (sender, e) =>
            {
                lb1.Text = e.Location.ToString();
            };
            Application.Run(frm);
        }

        //[STAThread]
        static void Main_WPF_Event(System.Windows.Application app)
        {
            //System.Windows.Application app = new System.Windows.Application();
            var lb1 = new System.Windows.Controls.Label();
            var txt = new System.Windows.Controls.TextBox();
            var panel = new System.Windows.Controls.StackPanel()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical
            };
            panel.Children.Add(lb1);
            panel.Children.Add(txt);
            var win = new System.Windows.Window()
            {
                Content = panel
            };
            win.MouseMove += (sender, e) =>
            {
                lb1.Content = e.GetPosition(win).ToString();
                txt.Text = e.GetPosition(txt).ToString();
            };
            win.Closed += (sender, e) =>
            {
                app.Shutdown();
            };
            app.MainWindow = win;
            win.Show();
            app.Run();
            
        }

        static void Main_Form_Pattern()
        {
            var lb1 = new Label();
            var txt = new TextBox();
            var frm = new Form()
            {
                Controls =
                {
                    lb1,
                    txt //frm.Controls.Add(txt);
                }
            };

            //frm.MouseMove += (sender, e) =>
            //{
            //    lb1.Text = e.Location.ToString();
            //};
            var moves = Observable.FromEventPattern<MouseEventArgs>(frm, "MouseMove");
            using (moves.Subscribe(evt =>
            {
                lb1.Text = evt.EventArgs.Location.ToString();
            }))

            Application.Run(frm);
        }

        static void Main_WPF_PatternOld(System.Windows.Application app)
        {
            //System.Windows.Application app = new System.Windows.Application();
            var lb1 = new System.Windows.Controls.Label();
            var txt = new System.Windows.Controls.TextBox();
            var panel = new System.Windows.Controls.StackPanel()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical
            };
            panel.Children.Add(lb1);
            panel.Children.Add(txt);
            var win = new System.Windows.Window()
            {
                Content = panel
            };
            win.MouseMove += (sender, e) =>
            {
                lb1.Content = e.GetPosition(win).ToString();
                txt.Text = e.GetPosition(txt).ToString();
            };
            //app.Run(win);
            app.MainWindow = win;
            win.Show();
            app.Run();
            
        }

        static void Main_WPF_Pattern(System.Windows.Application app)
        {
            //System.Windows.Application app = new System.Windows.Application();
            var lb1 = new System.Windows.Controls.Label();
            var txt = new System.Windows.Controls.TextBox();
            var panel = new System.Windows.Controls.StackPanel()
            {
                Orientation = System.Windows.Controls.Orientation.Vertical
            };
            panel.Children.Add(lb1);
            panel.Children.Add(txt);
            var win = new System.Windows.Window()
            {
                Content = panel
            };
            //win.MouseMove += (sender, e) =>
            //{
            //    lb1.Content = e.GetPosition(win).ToString();
            //    txt.Text = e.GetPosition(txt).ToString();
            //};
            var moves = Observable.FromEventPattern<System.Windows.Input.MouseEventArgs>(win, "MouseMove");
            var input = Observable.FromEventPattern(txt, "TextChanged");

            var moveSubscription = moves.Subscribe(evt =>
            {
                Console.WriteLine("Mouse at: " + evt.EventArgs.GetPosition(win));
            });

            var inputSubscription = input.Subscribe(evt =>
            {
                Console.WriteLine("User wrote: " + ((System.Windows.Controls.TextBox)evt.Sender).Text);
            });
            

            //app.Run(win);
            app.MainWindow = win;
            win.Show();
            using (new CompositeDisposable(moveSubscription, inputSubscription))
            {
                app.Run();
            }
        }
    }
}
