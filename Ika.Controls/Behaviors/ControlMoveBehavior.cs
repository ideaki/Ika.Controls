using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Ika.Controls.Behaviors
{
    public class ControlMoveBehavior : FrameworkElement, IBehavior
    {
        private static int zindex = 100;

        public DependencyObject AssociatedObject
        {
            get;
            private set;
        }

        public void Attach(DependencyObject associatedObject)
        {
            AssociatedObject = associatedObject;
            //SetBinding(
            //    DataContextProperty,
            //    new Binding
            //    {
            //        Path = new PropertyPath("DataContext"),
            //        Source = AssociatedObject,
            //        Mode = BindingMode.TwoWay
            //    });
            var fe = AssociatedObject as FrameworkElement;
            fe.Loaded += fe_Loaded;
        }

        void fe_Loaded(object sender, RoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            fe.RenderTransformOrigin = new Windows.Foundation.Point(0.5, 0.5);
            fe.ManipulationMode =
                ManipulationModes.TranslateX |
                ManipulationModes.TranslateY |
                ManipulationModes.Rotate |
                ManipulationModes.TranslateInertia |
                ManipulationModes.RotateInertia;
            fe.ManipulationDelta += Control_ManipulationDelta;
            fe.ManipulationCompleted += Control_ManipulationCompleted;
            fe.PointerWheelChanged += Control_PointerWheelChanged;
        }

        public void Detach()
        {
            var fe = AssociatedObject as FrameworkElement;
            fe.ManipulationDelta -= Control_ManipulationDelta;
            fe.ManipulationCompleted -= Control_ManipulationCompleted;
            fe.PointerWheelChanged -= Control_PointerWheelChanged;
            AssociatedObject = null;
        }


        private void Control_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var fe = e.OriginalSource as FrameworkElement;
            var canvas = fe.Parent as FrameworkElement;
            var x = Canvas.GetLeft(fe) + e.Delta.Translation.X;
            var y = Canvas.GetTop(fe) + e.Delta.Translation.Y;

            //Debug.WriteLine(string.Join(" : ", canvas.ActualHeight, canvas.ActualWidth, x, y, _zindex));
            //x = x <= 0 ? 0 : ActualWidth + x >= canvas.ActualWidth ? canvas.ActualWidth - ActualWidth : x;
            //y = y <= 0 ? 0 : ActualHeight + y >= canvas.ActualHeight ? canvas.ActualHeight - ActualHeight : y;
            //Canvas.SetLeft(fe, x);
            //Canvas.SetTop(fe, y);
            var transform = (fe.RenderTransform = (fe.RenderTransform as CompositeTransform) ?? new CompositeTransform()) as CompositeTransform;
            //transform.TranslateX += x;
            //transform.TranslateY += y;
            transform.TranslateX += e.Delta.Translation.X;
            transform.TranslateY += e.Delta.Translation.Y;
            //VisualStateManager.GoToState(fe, "dragging", true);
            Canvas.SetZIndex(fe, zindex++);
            //var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            //_RotateTransform.Angle += e.Delta.Rotation;
            //Debug.WriteLine(_RotateTransform.Angle);
            return;

            var element = sender as FrameworkElement;

            if (transform == null)
                element.RenderTransform = (transform = new CompositeTransform());
            transform.TranslateX += e.Delta.Translation.X;
            transform.TranslateY += e.Delta.Translation.Y;
        }

        private void Control_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            _RotateTransform.Angle = ((int)_RotateTransform.Angle) / 30 * 30;
            //VisualStateManager.GoToState(fe, "normal", true);
            return;
            //var _Rectangle = sender as Windows.UI.Xaml.Shapes.Rectangle;
            var _Rectangle = fe;
            _Rectangle.RenderTransform = null;

            var _Column = System.Convert.ToInt16(_Rectangle.GetValue(Grid.ColumnProperty));
            if (_Column <= 0 && e.Cumulative.Translation.X > _Rectangle.RenderSize.Width * .5)
                _Rectangle.SetValue(Grid.ColumnProperty, 1);
            else if (_Column == 1 && e.Cumulative.Translation.X < _Rectangle.RenderSize.Width * -.5)
                _Rectangle.SetValue(Grid.ColumnProperty, 0);

            var _Row = System.Convert.ToInt16(_Rectangle.GetValue(Grid.RowProperty));
            if (_Row <= 0 && e.Cumulative.Translation.Y > _Rectangle.RenderSize.Height * .5)
                _Rectangle.SetValue(Grid.RowProperty, 1);
            else if (_Row == 1 && e.Cumulative.Translation.Y < _Rectangle.RenderSize.Height * -.5)
                _Rectangle.SetValue(Grid.RowProperty, 0);
            //Debug.WriteLine(string.Join(" : ", _Column, _Row));
            //Debug.WriteLine(string.Join(" : ", e.Cumulative.Translation.X, e.Cumulative.Translation.Y));
            //Canvas.SetZIndex(fe, 0);
        }

        private void Control_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            _RotateTransform.Angle += e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0 ? -30 : 30;
        }

    }
}
