using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Ika.Controls.Behaviors
{
    public class ScrollViewerDraggedScrollBehavior : DependencyObject, IBehavior
    {
        Point _pressedPoint;
        double _h;
        double _v;

        public DependencyObject AssociatedObject { get; private set; }

        public void Attach(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;
            var scrollviewer = (ScrollViewer)this.AssociatedObject;
            scrollviewer.Loaded += scrollviewer_Loaded;
        }

        public void Detach()
        {
            this.AssociatedObject = null;
        }

        void scrollviewer_Loaded(object sender, RoutedEventArgs e)
        {
            var scrollviewer = (ScrollViewer)this.AssociatedObject;
            scrollviewer.PointerPressed += scrollviewer_PointerPressed;
        }

        void scrollviewer_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var devicetype = e.Pointer.PointerDeviceType;
            if (devicetype == PointerDeviceType.Touch) return;
            if (!e.GetCurrentPoint(null).Properties.IsLeftButtonPressed) return;

            var scrollviewer = (ScrollViewer)this.AssociatedObject;
            _pressedPoint = e.GetCurrentPoint(null).Position;
            _h = scrollviewer.HorizontalOffset;
            _v = scrollviewer.VerticalOffset;
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased += CoreWindow_PointerReleased;
            e.Handled = true;
        }

        void CoreWindow_PointerMoved(CoreWindow sender, PointerEventArgs args)
        {
            var deltaX = args.CurrentPoint.Position.X - _pressedPoint.X;
            var deltaY = args.CurrentPoint.Position.Y - _pressedPoint.Y;

            var moveX = _h - deltaX;
            var moveY = _v - deltaY;

            var scrollviewer = (ScrollViewer)this.AssociatedObject;

            scrollviewer.ChangeView(moveX, moveY, null, true);
        }

        void CoreWindow_PointerReleased(CoreWindow sender, PointerEventArgs args)
        {
            Window.Current.CoreWindow.PointerMoved -= CoreWindow_PointerMoved;
            Window.Current.CoreWindow.PointerReleased -= CoreWindow_PointerReleased;
        }
    }
}
