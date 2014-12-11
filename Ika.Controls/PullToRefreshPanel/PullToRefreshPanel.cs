﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Ika.Controls
{
    [TemplatePart(Name = "ScrollViewer", Type = typeof(ScrollViewer))]
    public class PullToRefreshPanel : ContentControl
    {
        bool isPullRefresh = false;

        public DependencyObject RefreshMessage
        {
            get { return (DependencyObject)GetValue(RefreshMessageProperty); }
            set { SetValue(RefreshMessageProperty, value); }
        }

        public static readonly DependencyProperty RefreshMessageProperty =
            DependencyProperty.Register("RefreshMessage", typeof(DependencyObject), typeof(PullToRefreshPanel), null);



        //public FrameworkElement Content
        //{
        //    get { return (FrameworkElement)GetValue(ContentProperty); }
        //    set { SetValue(ContentProperty, value); }
        //}


        //public static readonly DependencyProperty ContentProperty =
        //    DependencyProperty.Register("Content", typeof(FrameworkElement), typeof(PullToRefreshPanel), null);

        //public ScrollViewer ScrollViewer
        //{
        //    get { return (ScrollViewer)GetValue(ScrollViewerProperty); }
        //    private set { SetValue(ScrollViewerProperty, value); }
        //}

        //private static readonly DependencyProperty ScrollViewerProperty =
        //    DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(PullToRefreshPanel), null);

        public event EventHandler PullToRefresh;

        protected virtual void OnPullToRefresh(EventArgs e)
        {
            if (PullToRefresh != null)
                PullToRefresh(this, e);
        }

        public PullToRefreshPanel()
        {
            DefaultStyleKey = typeof(PullToRefreshPanel);
        }

        protected override void OnApplyTemplate()
        {
            var sc = GetTemplateChild("ScrollViewer") as ScrollViewer;
            sc.SizeChanged += ScrollViewer_SizeChanged;
            sc.ViewChanged += ScrollViewer_ViewChanged;
            base.OnApplyTemplate();
        }

        void ScrollViewer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            sv.ChangeView(null, (double)RefreshMessage.GetValue(HeightProperty), null);
            var content = Content as FrameworkElement;
            if (content == null) return;
            content.SetValue(HeightProperty, sv.ActualHeight);
            content.SetValue(WidthProperty, sv.ActualWidth);
        }

        async void ScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;


            if (sv.VerticalOffset == 0.0)
                VisualStateManager.GoToState(this, "Refresh", true);
            else
                VisualStateManager.GoToState(this, "Pull", true);

            if (sv.VerticalOffset != 0.0)
                isPullRefresh = true;

            if (!e.IsIntermediate)
            {
                if (sv.VerticalOffset == 0.0 && isPullRefresh)
                {
                    OnPullToRefresh(new EventArgs());

                    await Task.Delay(300);
                }
                isPullRefresh = false;
                sv.ChangeView(null, (double)RefreshMessage.GetValue(HeightProperty), null);
            }
        }
    }
}
