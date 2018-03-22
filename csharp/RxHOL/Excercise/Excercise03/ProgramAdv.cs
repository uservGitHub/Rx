using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using ExcerBaseLibrary;

namespace Excercise03
{
    class ProgramAdv
    {
        [STAThread]
        static void Main(string[] argv)
        {
            Application app = new Application();

            //app.Run(Kotlin.window("标准窗口",
            //    Kotlin.stackpanel(
            //        Kotlin.label(),
            //        Kotlin.textbox()
            //        )));
            Console.WriteLine("3种方式");
            //FromEventPattern();
            //FromEventLinq();
            //FromEventCondition();
            FromEventCondition(4);
        }

        static void FromEventPattern()
        {
            var lb1 = Kotlin.label();
            var tb1 = Kotlin.textbox();
            var sp1 = Kotlin.stackpanel(lb1, tb1);

            var moves = Observable.FromEventPattern<MouseEventArgs>(sp1, "MouseMove");
            var changes = Observable.FromEventPattern(tb1, "TextChanged");

            using (new CompositeDisposable(
                moves.Subscribe(evt =>
                {
                    lb1.Content = evt.EventArgs.GetPosition(sp1).ToString();
                }),
                changes.Subscribe(evt =>
                {
                    Console.WriteLine(((TextBox)evt.Sender).Text);
                })
                ))
            {
                Application.Current.Run(Kotlin.window(null, sp1));
            }
        }

        static void FromEventLinq()
        {
            var lb1 = Kotlin.label();
            var tb1 = Kotlin.textbox();
            var sp1 = Kotlin.stackpanel(lb1, tb1);

            var moves = from evt in Observable.FromEventPattern<MouseEventArgs>(sp1, "MouseMove")
                        select evt.EventArgs.GetPosition(sp1);
            var changes = from evt in Observable.FromEventPattern(tb1, "TextChanged")
                          select ((TextBox)evt.Sender).Text;

            using (new CompositeDisposable(
                moves.Subscribe(pos =>
                {
                    lb1.Content = pos.ToString();
                }),
                changes.Subscribe(inp =>
                {
                    Console.WriteLine(inp);
                })
                ))
            {
                Application.Current.Run(Kotlin.window(null, sp1));
            }
        }

        static void FromEventCondition()
        {
            var lb1 = Kotlin.label();
            var tb1 = Kotlin.textbox();
            var sp1 = Kotlin.stackpanel(lb1, tb1);

            var moves = from evt in Observable.FromEventPattern<MouseEventArgs>(sp1, "MouseMove")
                        select evt.EventArgs.GetPosition(sp1);
            var changes = from evt in Observable.FromEventPattern(tb1, "TextChanged")
                          select ((TextBox)evt.Sender).Text;

            var equalXY = from pos in moves
                          where pos.X == pos.Y
                          select pos;

            using (new CompositeDisposable(
                equalXY.Subscribe(pos =>
                {
                    lb1.Content = pos.ToString();
                }),
                changes.Subscribe(inp =>
                {
                    Console.WriteLine(inp);
                })
                ))
            {
                Application.Current.Run(Kotlin.window(null, sp1));
            }
        }

        static void FromEventCondition(int index)
        {
            var lb1 = Kotlin.label();
            var tb1 = Kotlin.textbox();
            var sp1 = Kotlin.stackpanel(lb1, tb1);

            var moves = from evt in Observable.FromEventPattern<MouseEventArgs>(sp1, "MouseMove")
                        select evt.EventArgs.GetPosition(sp1);

            IObservable<string> changes = null;
            
            switch (index)
            {
                case 1:
                changes = from evt in Observable.FromEventPattern(tb1, "TextChanged")
                          select ((TextBox)evt.Sender).Text;
                    break;

                case 2:
                    changes = (from evt in Observable.FromEventPattern(tb1, "TextChanged")
                               select ((TextBox)evt.Sender).Text)
                              .Do(inp => Console.WriteLine("Before DistinctUntilChanged: " + inp))
                              .DistinctUntilChanged();
                    break;
                case 3:
                    changes = (from evt in Observable.FromEventPattern(tb1, "TextChanged")
                             select ((TextBox)evt.Sender).Text)
                         .Timestamp()
                         .Do(inp => Console.WriteLine("I: " + inp.Timestamp.Millisecond + " - " + inp.Value))
                         .Select(x => x.Value)
                         .Throttle(TimeSpan.FromSeconds(1))
                         .Timestamp()
                         .Do(inp => Console.WriteLine("T: " + inp.Timestamp.Millisecond + " - " + inp.Value))
                         .Select(x => x.Value)
                         .DistinctUntilChanged();
                    break;
                case 4:
                    changes = (from evt in Observable.FromEventPattern(tb1, "TextChanged")
                               select ((TextBox)evt.Sender).Text)
                              .Throttle(TimeSpan.FromSeconds(1))
                              .DistinctUntilChanged();
                    break;
                default:
                    //changes = (from evt in Observable.FromEventPattern(tb1, "TextChanged")
                    //         select ((TextBox)evt.Sender).Text)
                    //     .LogTimestampedValues(x => Console.WriteLine("I: " + x.Timestamp.Millisecond + " - " + x.Value))
                    //     .Throttle(TimeSpan.FromSeconds(1))
                    //     .LogTimestampedValues(x => Console.WriteLine("T: " + x.Timestamp.Millisecond + " - " + x.Value))
                    //     .DistinctUntilChanged();
                    break;
            }
            //Console.WriteLine(inp)


            //DispatcherHelper.Initialize();
            //using (changes.Subscribe(inp => DispatcherHelper.CheckBeginInvokeOnUI(()=> { lb1.Content = inp; })))
            //{
            //    Application.Current.Run(Kotlin.window(null, sp1));
            //}

            //ObserveOn(ui) 可以获取到ui的线程 Excerices6,6以后的DictServiceSoap
            using (changes.ObserveOn(lb1).Subscribe(inp=>lb1.Content = inp))
            {
                Application.Current.Run(Kotlin.window(null, sp1));
            }



            //using (new CompositeDisposable(
            //    equalXY.Subscribe(pos =>
            //    {
            //        lb1.Content = pos.ToString();
            //    }),
            //    changes.Subscribe(inp =>
            //    {
            //        Console.WriteLine(inp);
            //    })
            //    ))
            //{
            //    Application.Current.Run(Kotlin.window(null, sp1));
            //}
        }
    }
}
