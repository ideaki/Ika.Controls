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
        bool _isPullRefresh;

        public event EventHandler PullToRefresh;

        public PullToRefreshPanel()
        {
            DefaultStyleKey = typeof(PullToRefreshPanel);
        }

        public object RefreshContent
        {
            get { return GetValue(RefreshContentProperty); }
            set { SetValue(RefreshContentProperty, value); }
        }

        public static readonly DependencyProperty RefreshContentProperty =
            DependencyProperty.Register("RefreshContent",
                                        typeof(object),
                                        typeof(PullToRefreshPanel),
                                        new PropertyMetadata("離して更新"));

        public object PullContent
        {
            get { return GetValue(PullContentProperty); }
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

        void UpdateLView()
        {
            var grid = (Grid)GetTemplateChild("PullGrid");
            ScrollViewer.ChangeView(null, grid.ActualHeight, null);
            var contentgrid = (Grid)GetTemplateChild("ContentGrid");
            contentgrid.SetValue(HeightProperty, ScrollViewer.ActualHeight);
            contentgrid.SetValue(WidthProperty, ScrollViewer.ActualWidth);
            UpdateTransform();
        }

        void UpdateStates(bool useTransitions)
        {
            VisualStateManager.GoToState(this, Math.Abs(ScrollViewer.VerticalOffset) < 0.0 ? "Refresh" : "Pull", useTransitions);
        }

        void UpdateTransform()
        {
            var element = (UIElement)GetTemplateChild("StackPanel");
            var transform = element.RenderTransform as CompositeTransform ?? new CompositeTransform();
            transform.TranslateY = (ScrollViewer.VerticalOffset - PullRange) * 0.8;
            element.RenderTransform = transform;
        }

        protected virtual void OnPullToRefresh(EventArgs e)
        {
            PullToRefresh?.Invoke(this, e);
        }

        protected override void OnApplyTemplate()
        {
            ScrollViewer = (ScrollViewer)GetTemplateChild("ScrollViewer");
            ScrollViewer.SizeChanged += (s, e) => UpdateLView();
            ScrollViewer.ViewChanged += ScrollViewer_ViewChanged;
            Loaded += (s, e) => UpdateLView();
            base.OnApplyTemplate();
        }

        void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            UpdateStates(true);
            UpdateTransform();

            if (Math.Abs(ScrollViewer.VerticalOffset) > 0.0)
                _isPullRefresh = true;

            if (e.IsIntermediate)
                return;
            if (Math.Abs(ScrollViewer.VerticalOffset) < 0.0 && _isPullRefresh)
            {
                OnPullToRefresh(new EventArgs());
            }
            _isPullRefresh = false;
            var grid = (Grid)GetTemplateChild("PullGrid");
            ScrollViewer.ChangeView(null, grid.ActualHeight, null);
        }

        private ScrollViewer ScrollViewer { get; set; }
    }
}
