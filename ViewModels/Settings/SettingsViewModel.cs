using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Microsoft.Toolkit.Mvvm.Input;
using Windows.Storage.Pickers;
using WinstaCore.Theme;
using WinstaCore.Models;
using WinstaCore;
using Resources;
using WinstaCore.Enums;
using WinstaCore.Interfaces.Views.Settings;
using Windows.UI.Xaml.Media.Animation;

namespace ViewModels.Settings
{
    public class SettingsViewModel : BaseViewModel
    {
        public RelayCommand AccountSettingsNavigateCommand { get; set; }
        public RelayCommand ApplicationSettingsNavigateCommand { get; set; }

        public SettingsViewModel() : base()
        {
            AccountSettingsNavigateCommand = new(AccountSettingsNavigate);
            ApplicationSettingsNavigateCommand = new(ApplicationSettingsNavigate);
        }

        void AccountSettingsNavigate()
        {
            var AccountSettings = AppCore.Container.GetService<IAccountSettings>();
#if !WINDOWS_UWP15063
            var transitionInfo = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            NavigationService.Navigate(AccountSettings, transitionInfo);
#else
            NavigationService.Navigate(AccountSettings);
#endif
        }

        void ApplicationSettingsNavigate()
        {
            var applicationSettings = AppCore.Container.GetService<IApplicationSettingsView>();
#if !WINDOWS_UWP15063
            var transitionInfo = new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight };
            NavigationService.Navigate(applicationSettings, transitionInfo);
#else
            NavigationService.Navigate(AccountSettings);
#endif
        }

    }
}
