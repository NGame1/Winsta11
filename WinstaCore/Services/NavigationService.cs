using ColorCode.Compilation.Languages;
using Microsoft.UI.Xaml.Controls;
using SecondaryViewsHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views;

namespace WinstaCore.Services
{
    public class NavigationService : INotifyPropertyChanged
    {
        CoreWindow _cireWindow;
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
            if (frame == null) return;
            if (Frame != null)
            {
                Frame.BackStack.Clear();
                Frame.Navigated -= Frame_Navigated;
            }
            Frame = frame;
            Frame.Navigated += Frame_Navigated;
            _cireWindow = CoreWindow.GetForCurrentThread();
        }

        public bool CanGoBack { get; set; }
        public bool CanGoForward { get; set; }
        public object Content { get => Frame.Content; }

        public IList<PageStackEntry> BackStack { get => Frame.BackStack; }

        public void GoBack() => Frame.GoBack();
        public void GoForward() => Frame.GoForward();

        public bool Navigate(Type sourcePageType)
        {
            //if (_cireWindow.GetKeyState(VirtualKey.Shift).
            //    HasFlag(CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked))
            //{
            //    OpenNewWindow(sourcePageType);
            //    return true;
            //}
            return Frame.Navigate(sourcePageType);
        }

        public bool Navigate(Type sourcePageType, object parameter)
        {
            //if (_cireWindow.GetKeyState(VirtualKey.Shift).
            //    HasFlag(CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked))
            //{
            //    OpenNewWindow(sourcePageType, parameter);
            //    return true;
            //}
            return Frame.Navigate(sourcePageType, parameter);
        }

        public bool Navigate(Type sourcePageType, object parameter, NavigationTransitionInfo infoOverride)
        {
            //if (_cireWindow.GetKeyState(VirtualKey.Shift).
            //    HasFlag(CoreVirtualKeyStates.Down | CoreVirtualKeyStates.Locked))
            //{
            //    OpenNewWindow(sourcePageType, parameter);
            //    return true;
            //}
            return Frame.Navigate(sourcePageType, parameter, infoOverride);
        }

        public bool Navigate<TView>(TView view, NavigationTransitionInfo transitionInfo = default) where TView : IView
        {
            return Frame.Navigate(view.GetType(), null, transitionInfo);
        }

        public bool Navigate<TView>(TView view, object parameter, NavigationTransitionInfo transitionInfo = default) where TView : IView
        {
            return Frame.Navigate(view.GetType(), parameter, transitionInfo);
        }


        /// <summary>
        /// Navigates to a page and returns the instance of the page if it succeeded,
        /// otherwise returns null.
        /// </summary>
        /// <typeparam name="TPage"></typeparam>
        /// <param name="frame"></param>
        /// <param name="transitionInfo">The navigation transition.
        /// Example: <see cref="DrillInNavigationTransitionInfo"/> or
        /// <see cref="SlideNavigationTransitionInfo"/></param>
        /// <returns></returns>
        public TPage Navigate<TPage>(
            NavigationTransitionInfo transitionInfo = default)
            where TPage : Page
        {
            TPage view = null;
            void OnNavigated(object s, NavigationEventArgs args)
            {
                Frame.Navigated -= OnNavigated;
                view = args.Content as TPage;
            }

            Frame.Navigated += OnNavigated;
            Frame.Navigate(typeof(TPage), null, transitionInfo);
            return view;
        }

        async void OpenNewWindow(Type sourcePageType, object parameter = null)
        {
            await OpenPageAsWindowAsync(sourcePageType, parameter);
        }

        public async Task OpenNewWindowAsync(Type sourcePageType, object parameter = null)
        {
            await OpenPageAsWindowAsync(sourcePageType, parameter);
        }

        /// <summary>
        /// Opens a page given the page type as a new window.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        async Task<bool> OpenPageAsWindowAsync(Type t, object parameter = null)
        {
            var res = await WindowManagerService.CreateViewLifetimeControlAsync(string.Empty, t, parameter);
            res.Released += Lifetimecontrol_Released;
            return await ApplicationViewSwitcher.TryShowAsStandaloneAsync(res.Id);
        }

        async void Lifetimecontrol_Released(object sender, EventArgs e)
        {
            var control = sender as ViewLifetimeControl;
            control.Released -= Lifetimecontrol_Released;
            await control.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                Window.Current.Close();
            });
        }
    }
}
