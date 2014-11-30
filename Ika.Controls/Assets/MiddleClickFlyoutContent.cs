using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Ika.Controls.Assets
{
    public sealed partial class MiddleClickFlyoutContent : UserControl
    {
        static SolidColorBrush white = new SolidColorBrush(Colors.White);
        static SolidColorBrush gray = new SolidColorBrush(Colors.Gray);

        public MiddleClickFlyoutContent()
        {
            this.IsHitTestVisible = false;

            var grid = new Grid();
            this.Content = grid;

            grid.Children.Add(new Ellipse()
            {
                Width = 30,
                Height = 30,
                Fill = new SolidColorBrush(Colors.White),
                Stroke = gray,
                StrokeThickness = 2
            });
            grid.Children.Add(new Ellipse
            {
                Width = 4,
                Height = 4,
                Fill = gray
            });
            grid.Children.Add(new Polygon
            {
                Margin = new Windows.UI.Xaml.Thickness(6, 3, 6, 0),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Top,
                Fill = gray,
                Points = PointCollectionFromPath("0,6 10,6, 5,0")
            });
            grid.Children.Add(new Polygon
            {
                Margin = new Windows.UI.Xaml.Thickness(8, 0, 8, 3),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Center,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Bottom,
                Fill = gray,
                Points = PointCollectionFromPath("0,0 10,0, 5,6")
            });
            grid.Children.Add(new Polygon
            {
                Margin = new Windows.UI.Xaml.Thickness(3, 8, 0, 8),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Left,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                Fill = gray,
                Points = PointCollectionFromPath("6,0 6,10, 0,5")
            });
            grid.Children.Add(new Polygon
            {
                Margin = new Windows.UI.Xaml.Thickness(0, 8, 3, 8),
                HorizontalAlignment = Windows.UI.Xaml.HorizontalAlignment.Right,
                VerticalAlignment = Windows.UI.Xaml.VerticalAlignment.Center,
                Fill = gray,
                Points = PointCollectionFromPath("0,0 6,5, 0,10")
            });
        }


        PointCollection PointCollectionFromPath(string path)
        {
            var pc = new PointCollection();

            string[] points = path.Split(new[] { " " }, StringSplitOptions.None);
            foreach (var point in points)
            {
                var p = point.Split(new[] { "," }, StringSplitOptions.None);
                pc.Add(new Point(double.Parse(p[0]), double.Parse(p[1])));
            }

            return pc;
        }
    }
}
