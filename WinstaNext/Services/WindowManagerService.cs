using Microsoft.Extensions.DependencyInjection;
using SecondaryViewsHelpers;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.Services
{
    public class WindowManagerService
    {
        public static async Task<ViewLifetimeControl> CreateViewLifetimeControlAsync(string windowTitle, Type pageType, object parameter = null)
        {
            ViewLifetimeControl viewControl = null;

            await CoreApplication.CreateNewView().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                viewControl = ViewLifetimeControl.CreateForCurrentView();
                viewControl.Title = windowTitle;
                viewControl.StartViewInUse();
                var frame = new Frame();
                var Nav = App.Container.GetService<NavigationService>();
                Nav.SetNavigationFrame(frame);
                var theme = ApplicationSettingsManager.Instance.GetTheme();
                if (theme != Core.Theme.AppTheme.Default)
                {
                    frame.RequestedTheme =
                        theme == Core.Theme.AppTheme.Dark ?
                            ElementTheme.Dark : ElementTheme.Light;
                }
                //Nav.Navigate(typeof(MainPage));
                Nav.Navigate(pageType, parameter);
                Window.Current.Content = frame;
                Window.Current.Activate();
                ApplicationView.GetForCurrentView().Title = viewControl.Title;
            });

            return viewControl;
        }
    }
}
