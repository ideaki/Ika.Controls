using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Ika.Controls.Utils;

namespace Ika.Controls.Behaviors
{
    public class ScrollViewerDoubleTappedZoomBehavior : DependencyObject, IBehavior
    {
        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;
            var scrollviewer = (ScrollViewer)this.AssociatedObject;
            scrollviewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollviewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            scrollviewer.IsHorizontalRailEnabled = false;
            scrollviewer.IsVerticalRailEnabled = false;
            scrollviewer.IsHorizontalScrollChainingEnabled = true;
            scrollviewer.IsVerticalScrollChainingEnabled = true;
            scrollviewer.Loaded += scrollviewer_Loaded;
        }

        public void Detach()
        {
            this.AssociatedObject = null;
        }

        void scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollviewer = (ScrollViewer)this.AssociatedObject;
            scrollviewer.DoubleTapped += scrollviewer_DoubleTapped;
        }

        void scrollviewer_DoubleTapped(object sender, Windows.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
        {
            var img = VisualTreeUtils.FindFirstElementInVisualTree<Image>(sender as DependencyObject);
            if (img == null) return;
            var point = e.GetPosition(img);
            ImageZoom(point, (ScrollViewer)sender, img);
        }

        async void ImageZoom(Point point, ScrollViewer sv, Image img, ZoomType zoomtype = ZoomType.DotByDot)
        {
            var bmp = img.Source as BitmapSource;
            if (bmp == null) return;
            if (bmp.PixelHeight == 0 || bmp.PixelWidth == 0) return;

            float? zoom = 1;

            //枠内
            if (sv.ActualHeight > bmp.PixelHeight && sv.ActualWidth > bmp.PixelWidth)
            {
                sv.ChangeView(null, null, zoom);
                sv.MinZoomFactor = (float)zoom;
                return;
            }
            sv.IsEnabled = false;

            //枠外
            zoom = (float?)(1 / (bmp.PixelHeight / sv.ActualHeight) * 0.99);
            var temp_zoom = (float?)(1 / (bmp.PixelWidth / sv.ActualWidth) * 0.99);
            if (temp_zoom < zoom) zoom = temp_zoom;
            if (zoom > 0.1) sv.MinZoomFactor = (float)zoom;
            float? horizontaloffset = (float?)(point.X - sv.ActualWidth / 2.0);
            float? verticaloffset = (float?)(point.Y - sv.ActualHeight / 2.0);
            switch (zoomtype)//1:デフォルト 2:ドットバイドット<=>デフォルト
            {
                case ZoomType.UniformToFill:
                    sv.ChangeView(null, null, zoom);
                    break;

                case ZoomType.DotByDot:
                    var z = (int)(sv.ZoomFactor * 1000) - (int)(zoom * 1000);
                    if (z > -3 && 3 > z) zoom = 1;
                    sv.ChangeView(horizontaloffset, verticaloffset, zoom);
                    break;
            }
            await Task.Delay(300);
            sv.IsEnabled = true;
        }

        enum ZoomType
        {
            UniformToFill,
            DotByDot, // ドットバイドット<=>デフォルト
        }

    }
}
