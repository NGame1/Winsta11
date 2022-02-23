using InstagramApiSharp.Classes.Models;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    public sealed partial class InstaBroadcastPresenterUC : UserControl
    {
        public static readonly DependencyProperty BroadcastProperty = DependencyProperty.Register(
             "Broadcast",
             typeof(InstaBroadcast),
             typeof(InstaBroadcastPresenterUC),
             new PropertyMetadata(null));

        public InstaBroadcast Broadcast
        {
            get { return (InstaBroadcast)GetValue(BroadcastProperty); }
            set { SetValue(BroadcastProperty, value); }
        }

        public InstaBroadcastPresenterUC()
        {
            this.InitializeComponent();
        }
    }
}
