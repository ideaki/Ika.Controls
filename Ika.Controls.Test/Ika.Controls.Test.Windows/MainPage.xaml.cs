using Ika.Controls.Test.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234238 を参照してください

namespace Ika.Controls.Test
{
    /// <summary>
    /// Frame 内へナビゲートするために利用する空欄ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }

    public class AutoScroll : BindableBase
    {
        bool _isScroll = false;
        double _vspeed = 1;
        double _hspeed = 1;

        public double VerticalSpeed
        {
            get { return _vspeed; }
            set { SetProperty(ref _vspeed, value); }
        }
        public double HorizontalSpeed
        {
            get { return _hspeed; }
            set { SetProperty(ref _hspeed, value); }
        }
        public bool IsScroll
        {
            get { return _isScroll; }
            set { SetProperty(ref _isScroll, value); }
        }
        public string Text { get; set; }
    }
}
