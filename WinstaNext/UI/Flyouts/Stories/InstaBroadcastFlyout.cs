using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

namespace WinstaNext.UI.Flyouts.Stories
{
    internal class InstaBroadcastFlyout : MenuFlyout
    {
        public static readonly DependencyProperty BroadcastProperty = DependencyProperty.Register(
          "Broadcast",
          typeof(InstaBroadcast),
          typeof(InstaBroadcastFlyout),
          new PropertyMetadata(null));

        public InstaBroadcast Broadcast
        {
            get { return (InstaBroadcast)GetValue(BroadcastProperty); }
            set { SetValue(BroadcastProperty, value); }
        }

        RelayCommand NavigateToUserProfileCommand { get; set; }

        public InstaBroadcastFlyout()
        {
            Opening += InstaBroadcastFlyout_Opening;
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
        }

        void NavigateToUserProfile()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), Broadcast.BroadcastOwner);
        }

        private void InstaBroadcastFlyout_Opening(object sender, object e)
        {
            if (Broadcast == null) return;
            Items.Clear();

            //var font = (FontFamily)App.Current.Resources["FluentIcons"];
            var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

            Items.Add(new MenuFlyoutItem()
            {
                Icon = new FontIcon() { Glyph = "\uF5BE", FontFamily = FluentSystemIconsRegular },
                Text = LanguageManager.Instance.Instagram.ViewProfile,
                Command = NavigateToUserProfileCommand
            });
        }
    }
}
