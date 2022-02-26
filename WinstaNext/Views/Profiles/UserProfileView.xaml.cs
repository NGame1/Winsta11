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
    public sealed partial class UserProfileView : BasePage
    {
        public UserProfileView()
        {
            this.InitializeComponent();
        }

        private void lst_Loaded(object sender, RoutedEventArgs e)
        {
            //ViewModel.ListViewScroll = lst.FindAscendantOrSelf<ScrollViewer>();
            ViewModel.ListViewScroll = lst.FindDescendantOrSelf<ScrollViewer>();
            //var s = lst.FindChildOrSelf<ScrollViewer>();
        }

        private void NavigationView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            var item = args.SelectedItem as NavigationViewItem;
            if (item.Content.ToString() == "Medias")
            {
                lst.ItemsSource = ViewModel.UserMedias;
            }
            else if (item.Content.ToString() == "Reels")
            {
                lst.ItemsSource = ViewModel.UserReels;
            }
        }
    }
}
