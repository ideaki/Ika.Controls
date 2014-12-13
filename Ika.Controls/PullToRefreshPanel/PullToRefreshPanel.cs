using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Ika.Controls
{
    public class PullToRefreshPanel : ContentControl
    {
        bool isPullRefresh = false;

        public event EventHandler PullToRefresh;

        public PullToRefreshPanel()
        {
            DefaultStyleKey = typeof(PullToRefreshPanel);
            FontSize = 28;
        }

        public object RefreshContent
        {
            get { return (object)GetValue(RefreshContentProperty); }
            set { SetValue(RefreshContentProperty, value); }
        }

        public static readonly DependencyProperty RefreshContentProperty =
            DependencyProperty.Register("RefreshContent",
                                        typeof(object),
                                        typeof(PullToRefreshPanel),
                                        new PropertyMetadata("離して更新"));

        public object PullContent
        {
            get { return (object)GetValue(PullContentProperty); }
            set { SetValue(PullContentProperty, value); }
        }

        public static readonly DependencyProperty PullContentProperty =
            DependencyProperty.Register("PullContent",
                                        typeof(object),
                                        typeof(PullToRefreshPanel),
                                        new PropertyMetadata("引っ張って"));

        public double PullRange
        {
            get { return (double)GetValue(PullRangeProperty); }
            set { SetValue(PullRangeProperty, value); }
        }

        public static readonly DependencyProperty PullRangeProperty =
            DependencyProperty.Register("PullRange",
                                        typeof(double),
                                        typeof(PullToRefreshPanel),
                                        new PropertyMetadata(200.0));

        void UpdateStates(bool useTransitions)
        {
            if (ScrollViewer.VerticalOffset == 0.0)
                VisualStateManager.GoToState(this, "Refresh", useTransitions);
            else
                VisualStateManager.GoToState(this, "Pull", useTransitions);
        }

        void UpdateTransform()
        {
            var element = GetTemplateChild("StackPanel") as UIElement;
            var transform = element.RenderTransform as CompositeTransform ?? new CompositeTransform();
            transform.TranslateY = (ScrollViewer.VerticalOffset - PullRange) * 0.8;
            element.RenderTransform = transform;
        }

        protected virtual void OnPullToRefresh(EventArgs e)
        {
            if (PullToRefresh != null)
                PullToRefresh(this, e);
        }

        protected override void OnApplyTemplate()
        {
            ScrollViewer = GetTemplateChild("ScrollViewer") as ScrollViewer;
            ScrollViewer.SizeChanged += ScrollViewer_SizeChanged;
            ScrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            base.OnApplyTemplate();
        }

        void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var grid = GetTemplateChild("PullGrid") as Grid;
            ScrollViewer.ChangeView(null, grid.ActualHeight, null);
            var contentgrid = GetTemplateChild("ContentGrid") as Grid;
            contentgrid.SetValue(HeightProperty, ScrollViewer.ActualHeight);
            contentgrid.SetValue(WidthProperty, ScrollViewer.ActualWidth);
        }

        void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateStates(true);
            UpdateTransform();

            if (ScrollViewer.VerticalOffset != 0.0)
                isPullRefresh = true;

            if (!e.IsIntermediate)
            {
                if (ScrollViewer.VerticalOffset == 0.0 && isPullRefresh)
                {
                    OnPullToRefresh(new EventArgs());

                    //await Task.Delay(50);
                }
                isPullRefresh = false;
                var grid = GetTemplateChild("PullGrid") as Grid;
                ScrollViewer.ChangeView(null, grid.ActualHeight, null);
            }
        }

        ScrollViewer scrollviewer;
        ScrollViewer ScrollViewer
        {
            get { return scrollviewer; }
            set { scrollviewer = value; }
        }
    }
}
