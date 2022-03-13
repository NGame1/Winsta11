using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class HashtagProfileView : BasePage
    {
        ItemsWrapGrid WrapGrid { get; set; }
        public HashtagProfileView()
        {
            this.InitializeComponent();
        }

        private void lst_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ListViewScroll = lst.FindDescendantOrSelf<ScrollViewer>();
        }

        private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            WrapGrid = (ItemsWrapGrid)sender;
        }

        private void lst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WrapGrid == null) return;
            if(!ApplicationSettingsManager.Instance.GetForceThreeColumns())
            {
                WrapGrid.ItemHeight = WrapGrid.ItemWidth = 185;
                WrapGrid.SizeChanged -= lst_SizeChanged;
                lst.SizeChanged -= lst_SizeChanged;
                return;
            }
            var width = e.NewSize.Width / 3f;
            WrapGrid.ItemHeight = WrapGrid.ItemWidth = width;
        }

    }
}
