using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Ika.Controls.Test
{
    public class MyListView : ListView
    {
        protected override void OnApplyTemplate()
        {
            ItemsSource = Enumerable.Range(1, 30);
            base.OnApplyTemplate();
        }

        /// <summary>
        /// アイテムに色と高さを設定
        /// </summary>
        /// <param name="element"></param>
        /// <param name="item"></param>
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            int index = IndexFromContainer(element) * 10;
            //if (index % 2 == 0)
            //{
            //    element.SetValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb((byte)0, (byte)(index/256/256),(byte)(index/256),(byte)index)));
            //}
            //else
            //{
            //    element.SetValue(BackgroundProperty, new SolidColorBrush(Colors.Black));
            //}
            var r = Math.Min(255, Math.Max(0, 255 - index));
            var g = Math.Min(255, Math.Max(0, 510 - index));
            var b = Math.Min(255, index);

#if WINDOWS_PHONE_APP
            element.SetValue(MarginProperty, new Thickness(5));
#endif

            element.SetValue(BackgroundProperty, new SolidColorBrush(Color.FromArgb((byte)255, (byte)r, (byte)g, (byte)b)));
            //element.SetValue(HeightProperty, index + 10);
            base.PrepareContainerForItemOverride(element, item);
        }

    }
}
