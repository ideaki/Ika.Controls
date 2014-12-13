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
    //[TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    //[TemplatePart(Name = "Grid", Type = typeof(Grid))]
    //[TemplateVisualState(Name = "Normal", GroupName = "VisualStates")]
    //[TemplateVisualState(Name = "Pull", GroupName = "VisualStates")]
    //[TemplateVisualState(Name = "Refresh", GroupName = "VisualStates")]
    public class PullToRefreshPanel : ContentControl
    {
        bool isPullRefresh = false;

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



        public event EventHandler PullToRefresh;

        void UpdateStates(bool useTransitions)
        {
            if (ScrollViewer.VerticalOffset == 0.0)
                VisualStateManager.GoToState(this, "Refresh", useTransitions);
            else
                VisualStateManager.GoToState(this, "Pull", useTransitions);
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
            var grid = GetTemplateChild("Grid") as Grid;
            ScrollViewer.ChangeView(null, grid.ActualHeight, null);
            var content = Content as FrameworkElement;
            if (content == null) return;
            content.SetValue(HeightProperty, ScrollViewer.ActualHeight);
            content.SetValue(WidthProperty, ScrollViewer.ActualWidth);
        }

        void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {

            UpdateStates(true);

            if (ScrollViewer.VerticalOffset != 0.0)
                isPullRefresh = true;


            var Element = GetTemplateChild("StackPanel") as UIElement;
            var Transform = Element.RenderTransform as CompositeTransform;
            if (Transform == null)
                Transform = new CompositeTransform();

            Transform.TranslateY = (ScrollViewer.VerticalOffset - PullRange) * 0.8;

            Element.RenderTransform = Transform;

            var grid = GetTemplateChild("Grid") as Grid;

            if (!e.IsIntermediate)
            {
                if (ScrollViewer.VerticalOffset == 0.0 && isPullRefresh)
                {
                    OnPullToRefresh(new EventArgs());

                    //await Task.Delay(50);
                }
                isPullRefresh = false;
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
