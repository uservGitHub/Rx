using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace ExcerBaseLibrary
{
    public class Kotlin
    {
        private static int AutoWindowNo = 0;

        public static Label label(object context = null)
        {
            var obj = new Label()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
            };
            if (context != null) obj.Content = context;
            else obj.Content = string.Empty;
            return obj;
        }

        public static TextBox textbox(object context = null)
        {
            var obj = new TextBox()
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Top,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                TextWrapping = TextWrapping.Wrap,
                AcceptsReturn = true,
            };
            if(context != null) obj.Text = (context == null) ? "" : context.ToString();
            return obj;
        }

        //public static StackPanel stackpanel(params UIElement[] children)
        //{
        //    var obj = new StackPanel();
        //    obj.Orientation = Orientation.Vertical;
        //    foreach (var sub in children)
        //    {
        //        if (sub != null) obj.Children.Add(sub);
        //    }
        //    return obj;
        //}
        public static StackPanel stackpanel(params UIElement[] children)
        {
            var obj = new StackPanel();
            obj.Orientation = Orientation.Vertical;
            foreach(var sub in children)
            {
                if (sub != null) obj.Children.Add(sub);
            }
            return obj;
        }

        public static Window window(string title=null, object context=null)
        {
            var obj = new Window();
            obj.Title = (title == null) ? string.Format("Auto_{0:D2}", ++AutoWindowNo) : title;
            if (context != null) obj.Content = context;
            return obj;
        }
    }
}
