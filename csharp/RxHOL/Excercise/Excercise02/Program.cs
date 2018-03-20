using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using ExcerBaseLibrary;

namespace Excercise02
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine(watch.ElapsedMilliseconds);
            DelayTimer.Add(() =>
            {
                Console.WriteLine(watch.ElapsedMilliseconds);
            }, 1500);
            DelayTimer.Add(() =>
            {
                Console.WriteLine(watch.ElapsedMilliseconds);
            }, 1500);
            DelayTimer.Add(() =>
            {
                Console.WriteLine(watch.ElapsedMilliseconds);
            }, 1500);
            Console.ReadKey();
        }
    }
}
