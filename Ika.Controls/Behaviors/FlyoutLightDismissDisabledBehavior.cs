using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace Ika.Controls.Behaviors
{
    /// <summary>
    /// 動かない
    /// </summary>
    public class FlyoutPopupBehavior : FrameworkElement, IBehavior
    {
        public DependencyObject AssociatedObject { get; private set; }

        public Popup Popup
        {
            get { return (Popup)this.GetValue(PopupProperty); }
            set { this.SetValue(PopupProperty, value); }
        }

        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register("Popup",
                                        typeof(Popup),
                                        typeof(FlyoutPopupBehavior),
                                        new PropertyMetadata(false, PopupChanged));

        private static void PopupChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            //var popup = d as Popup;
            //if (popup is Popup)
            //    popup.IsLightDismissEnabled = false;
        }

        public void Attach(DependencyObject associatedObject)
        {
            this.AssociatedObject = associatedObject;

            Flyout flyout = (Flyout)this.AssociatedObject;
            var fe = flyout.Content as FrameworkElement;
            flyout.Opening += flyout_Opening;
            flyout.Opened += flyout_Opened;
            fe.Loaded += fe_Loaded;

        }

        void flyout_Opened(object sender, object e)
        {
            Flyout flyout = (Flyout)sender;
            var fe = flyout.Content as FrameworkElement;

            while (!(fe is Popup))
            {
                fe = fe.Parent as FrameworkElement;
            }
            Popup = fe as Popup;
            Popup.IsLightDismissEnabled = false;
        }

        void flyout_Opening(object sender, object e)
        {
        }

        public void Detach()
        {
            FrameworkElement fe = (FrameworkElement)this.AssociatedObject;
            fe.Loaded -= fe_Loaded;
            this.AssociatedObject = null;
        }

        void fe_Loaded(object sender, RoutedEventArgs e)
        {
            SetBinding(
                DataContextProperty,
                new Binding
                {
                    Path = new PropertyPath("DataContext"),
                    Source = AssociatedObject,
                    Mode = BindingMode.TwoWay
                });

            var fe = sender as FrameworkElement;
            while (!(fe is Popup))
            {
                fe = fe.Parent as FrameworkElement;
            }
            Popup = fe as Popup;
            Popup.IsLightDismissEnabled = false;
        }

    }
}
