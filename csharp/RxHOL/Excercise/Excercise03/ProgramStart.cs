using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ExcerBaseLibrary;

namespace Excercise03
{
    class ProgramStart
    {
        [STAThread]//
        static void Main_NoOpen(string[] args)
        {
            System.Threading.Thread thread = new System.Threading.Thread(() =>
             {
                 Console.WriteLine("开启Form");
                 Main_Form();    //阻塞方式
             });
            thread.Start();

            Console.WriteLine("开启WPF");
            Main_WPF(); //阻塞方式

            thread.Join();
            Console.WriteLine("按任意键退出!");
            Console.ReadKey();
        }

        static void Main_Form()
        {
            var frm = new Form();
            Application.Run(frm);
        }

        //[STAThread]
        static void Main_WPF()
        {
            System.Windows.Application app = new System.Windows.Application();
            //app.MainWindow = new System.Windows.Window();
            var win = new System.Windows.Window();
            app.Run(win);
        }
    }
}
