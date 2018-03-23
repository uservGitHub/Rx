using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Drawing;

namespace GxdApp1
{
    public static class Extentions
    {
        public static SolidBrush ToBrush(this Color color)
        {
            return new SolidBrush(color);
        } 
    }
}
