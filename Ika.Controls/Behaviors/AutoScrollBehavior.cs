using Ika.Controls.Views;
using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;

namespace Ika.Controls.Behaviors
{
    public class AutoScrollBehavior : ItemsControl, IBehavior
    {

        public DependencyObject AssociatedObject { get; private set; }

        ScrollViewer _scrollViewer;
        DispatcherTimer _dt = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(16) };
        Point _pressedPoint;
        Popup _middleClickFlyout = new Popup()
        {
            IsLightDismissEnabled = true,
            Width = 30,
            Height = 30,
            Child = new MiddleClickFlyoutContent()
        };

        public bool IsAutoScrollEnabled
        {
            get { return (bool)this.GetValue(IsAutoScrollEnabledProperty); }
            set { this.SetValue(IsAutoScrollEnabledProperty, value); }
        }

        public double VerticalSpeed
        {
            get { return (double)this.GetValue(VerticalSpeedProperty); }
            set { this.SetValue(VerticalSpeedProperty, value); }
        }

        public double HorizontalSpeed
        {
            get { return (double)this.GetValue(HorizontalSpeedProperty); }
            set { this.SetValue(HorizontalSpeedProperty, value); }
        }


        public static readonly DependencyProperty IsAutoScrollEnabledProperty =
            DependencyProperty.Register("IsAutoScrollEnabled",
                                        typeof(bool),
                                        typeof(AutoScrollBehavior),
                                        new PropertyMetadata(false, IsAutoScrollEnabledChanged));

        public static readonly DependencyProperty VerticalSpeedProperty =
            DependencyProperty.Register("VerticalSpeed",
                                        typeof(double),
                                        typeof(AutoScrollBehavior),
                                        null);

        public static readonly DependencyProperty HorizontalSpeedProperty =
            DependencyProperty.Register("HorizontalSpeed",
                                        typeof(double),
                                        typeof(AutoScrollBehavior),
                                        null);

        public void Attach(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;

            ItemsControl itemscontrol = (ItemsControl)this.AssociatedObject;
            itemscontrol.Loaded += itemscontrol_Loaded;
            itemscontrol.PointerPressed += itemscontrol_PointerPressed;
        }

        public void Detach()
        {
            ItemsControl itemscontrol = (ItemsControl)this.AssociatedObject;
            itemscontrol.Loaded -= itemscontrol_Loaded;
            itemscontrol.PointerPressed -= itemscontrol_PointerPressed;
            this.AssociatedObject = null;
        }

        static void IsAutoScrollEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var b = d as AutoScrollBehavior;
            if ((bool)e.NewValue) b._dt.Start();
            else b._dt.Stop();
        }

        void itemscontrol_Loaded(object sender, RoutedEventArgs e)
        {
            AutomationPeer ap = FrameworkElementAutomationPeer.CreatePeerForElement(this.AssociatedObject as UIElement);
            ScrollViewerAutomationPeer avap = (ScrollViewerAutomationPeer)ap.GetPattern(PatternInterface.Scroll);
            _scrollViewer = (ScrollViewer)avap.Owner;

            SetBinding(
                DataContextProperty,
                new Binding
                {
                    Path = new PropertyPath("DataContext"),
                    Source = AssociatedObject,
                    Mode = BindingMode.TwoWay
                });

            _dt.Tick += (_, __) =>
            {
                _scrollViewer.ChangeView(_scrollViewer.HorizontalOffset + HorizontalSpeed,
                                        _scrollViewer.VerticalOffset + VerticalSpeed,
                                        null,
                                       true);
                _scrollViewer.UpdateLayout();
            };
        }

        void _autoScrollNavigationFlyout_Closed(object sender, object e)
        {
            IsAutoScrollEnabled = false;
            VerticalSpeed = 0;
            HorizontalSpeed = 0;
            Window.Current.CoreWindow.PointerMoved -= CoreWindow_PointerMoved;
            _middleClickFlyout.Closed -= _autoScrollNavigationFlyout_Closed;
        }

        void itemscontrol_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!e.GetCurrentPoint(null).Properties.IsMiddleButtonPressed) return;

            _pressedPoint = e.GetCurrentPoint(null).Position;
            Canvas.SetLeft(_middleClickFlyout, _pressedPoint.X - 15);
            Canvas.SetTop(_middleClickFlyout, _pressedPoint.Y - 15);
            VerticalSpeed = 0;
            HorizontalSpeed = 0;
            IsAutoScrollEnabled = true;
            _middleClickFlyout.IsOpen = true;
            _dt.Start();
            _middleClickFlyout.Closed += _autoScrollNavigationFlyout_Closed;
            Window.Current.CoreWindow.PointerMoved += CoreWindow_PointerMoved;
            e.Handled = true;
        }

        void CoreWindow_PointerMoved(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.PointerEventArgs args)
        {
            var cpt = args.CurrentPoint.Position;
            VerticalSpeed = SpeedFromLength((int)(cpt.Y - _pressedPoint.Y));
            HorizontalSpeed = SpeedFromLength((int)(cpt.X - _pressedPoint.X));
        }

        double SpeedFromLength(int len)
        {
            var alen = Math.Abs(len);

            if (alen < 16)
                return 0;
            else if (alen < 100)
                return Math.Sign(len) * (alen - 15) / 8.0;
            else
                return alen * len / Math.Max(900 - alen, 1);
        }
    }
}
