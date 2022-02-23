using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

namespace WinstaNext.Services
{
    public class NavigationService : INotifyPropertyChanged
    {
        Frame Frame { get; set; }

        public NavigationService(Frame frame)
        {
            SetNavigationFrame(frame);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Frame_Navigated(object sender, NavigationEventArgs e)
        {
            CanGoBack = Frame.CanGoBack;
            CanGoForward = Frame.CanGoForward;
        }

        public void SetNavigationFrame(Frame frame)
        {
            if (Frame != null)
            {
                Frame.BackStack.Clear();
                Frame.Navigated -= Frame_Navigated;
            }
            Frame = frame;
            Frame.Navigated += Frame_Navigated;
        }

        public bool CanGoBack { get; set; }
        public bool CanGoForward { get; set; }
        public object Content { get => Frame.Content; }

        public IList<PageStackEntry> BackStack { get => Frame.BackStack; }

        public void GoBack() => Frame.GoBack();
        public void GoForward() => Frame.GoForward();

        public bool Navigate(Type sourcePageType) => Frame.Navigate(sourcePageType);
        public bool Navigate(Type sourcePageType, object parameter) => Frame.Navigate(sourcePageType, parameter);
        public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride) => Frame.Navigate(sourcePageType, parameter, infoOverride);

    }
}
