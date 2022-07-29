using Microsoft.Extensions.DependencyInjection;
using Resources;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Services;
using ViewModels;
using WinstaMobile.Views.Account;

namespace WinstaMobile.Services
{
    public static class AccountManagementService
    {
        static void StopPush()
        {
            try
            {
                if (MainPageViewModel.mainPageViewModel != null)
                    MainPageViewModel.mainPageViewModel.StopPushClient();
            }
            catch { }
        }

        public static void LoginToAnotherUser()
        {
            StopPush();
            ((App)App.Current).SetCurrentUserSession(null);
            var frame = new Frame();
            Window.Current.Content = frame;
            frame.Navigate(typeof(LoginView));
            var nav = App.Container.GetService<NavigationService>();
            nav.SetNavigationFrame(frame);
        }

        public static async void SwitchToUser(string userpk)
        {
            StopPush();
            var session = await ApplicationSettingsManager.Instance.GetUserSession(userpk);
            ((App)App.Current).SetCurrentUserSession(session);
            var frame = new Frame();
            if (LanguageManager.Instance.General.IsRightToLeft)
                frame.FlowDirection = FlowDirection.RightToLeft;
            Window.Current.Content = frame;
            frame.Navigate(typeof(MainPage));
            var nav = App.Container.GetService<NavigationService>();
            nav.SetNavigationFrame(frame);
        }
    }
}
