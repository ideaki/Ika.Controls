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
            var fe = sender as FrameworkElement;
            var canvas = ((FrameworkElement)fe).Parent as FrameworkElement;
            var x = Canvas.GetLeft(fe) + e.Delta.Translation.X;
            var y = Canvas.GetTop(fe) + e.Delta.Translation.Y;

            x = x <= 0 ? 0 : fe.ActualWidth + x >= canvas.ActualWidth ? canvas.ActualWidth - fe.ActualWidth : x;
            y = y <= 0 ? 0 : fe.ActualHeight + y >= canvas.ActualHeight ? canvas.ActualHeight - fe.ActualHeight : y;
            Canvas.SetLeft(fe, x);
            Canvas.SetTop(fe, y);
            //VisualStateManager.GoToState(this, "dragging", true);
            Canvas.SetZIndex(fe, zindex++);
            var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            _RotateTransform.Angle += e.Delta.Rotation;
            //Debug.WriteLine(_RotateTransform.Angle);
        }

        private void Control_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            _RotateTransform.Angle = ((int)_RotateTransform.Angle) / 30 * 30;
            //VisualStateManager.GoToState(fe, "normal", true);
        }

        private void Control_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var fe = sender as FrameworkElement;
            var _RotateTransform = (fe.RenderTransform = (fe.RenderTransform as RotateTransform) ?? new RotateTransform()) as RotateTransform;
            _RotateTransform.Angle += e.GetCurrentPoint(null).Properties.MouseWheelDelta > 0 ? -30 : 30;
        }

    }
}
