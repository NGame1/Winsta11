﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore;
using WinstaMobile.Helpers.ExtensionMethods;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HashtagProfileView : BasePage, IHashtagProfileView
    {
        public override string PageHeader { get; protected set; } = string.Empty;

        ItemsWrapGrid? WrapGrid { get; set; }

        public HashtagProfileView()
        {
            this.InitializeComponent();
        }

        private void lst_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.ListViewScroll = lst.FindDescendant<ScrollViewer>();
        }

        private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            WrapGrid = (ItemsWrapGrid)sender;
        }

        private void lst_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (WrapGrid == null) return;
            if (!ApplicationSettingsManager.Instance.GetForceThreeColumns())
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
