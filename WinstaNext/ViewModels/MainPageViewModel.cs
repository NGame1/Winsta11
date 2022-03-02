
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WinstaNext.Core.Collections;
using WinstaNext.Core.Messages;
using WinstaNext.Core.Theme;
using WinstaNext.Models.Core;
using WinstaNext.Views;
using WinstaNext.Views.Settings;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
using PropertyChanged;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InstagramApiSharp.API;
using Microsoft.Extensions.DependencyInjection;
using InstagramApiSharp.Classes.Models;
using WinstaNext.Views.Directs;
using System.Diagnostics;
using InstagramApiSharp;
using WinstaNext.Views.Profiles;
using WinstaNext.Views.Search;
using InstagramApiSharp.API.Push;
using WinstaBackgroundHelpers.Push;
using NotificationHandler;

namespace WinstaNext.ViewModels
{
    public class MainPageViewModel : BaseViewModel
    {
        ThemeListener _themeListener;
        public string SearchQuery { get; set; }
        public string WindowTitle { get; set; } = "My App";
        public bool IsNavigationViewPaneOpened { get; set; }

        public RelayCommand ToggleNavigationViewPane { get; }
        public AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs> SearchBoxTextChangedCommand { get; }
        public RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs> SearchBoxQuerySubmittedCommand { get; }
        public RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs> SearchBoxSuggestionChosenCommand { get; }
        public RelayCommand<NavigationEventArgs> FrameNavigatedCommand { get; }
        public RelayCommand<object> NavigateToUserProfileCommand { get; }

        /// <summary>
        /// Items at the top of the NavigationView.
        /// </summary>
        internal ExtendedObservableCollection<MenuItemModel> MenuItems { get; } = new();

        /// <summary>
        /// Gets or sets the list of items to displayed in the Search Box after a search.
        /// </summary>
        internal ExtendedObservableCollection<InstaUser> SearchResults { get; } = new();

        /// <summary>
        /// Items at the bottom of the NavigationView.
        /// </summary>
        internal ExtendedObservableCollection<MenuItemModel> FooterMenuItems { get; } = new();

        /// <summary>
        /// Gets or sets the selected menu item in the NavitationView.
        /// </summary>
        [OnChangedMethod(nameof(SelectedMenuItemChanged))]
        internal MenuItemModel SelectedMenuItem { get; set; }

        public Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode NavigationViewDisplayMode { get; set; }

        public InstaUserShort InstaUser { get; private set; }

        public override string PageHeader { get; protected set; }

        public MainPageViewModel()
        {
            SearchBoxTextChangedCommand = new AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs>(SearchBoxTextChanged);
            SearchBoxQuerySubmittedCommand = new RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs>(SearchBoxQuerySubmitted);
            SearchBoxSuggestionChosenCommand = new RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs>(SearchBoxSuggestionChosen);

            FrameNavigatedCommand = new RelayCommand<NavigationEventArgs>(FrameNavigated);
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            _themeListener = new ThemeListener();
            SetupTitlebar(CoreApplication.GetCurrentView().TitleBar);
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.General.Home, "\uE10F", typeof(HomeView)));
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Activities, "\uE006", null));
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Explore, "\uF6FA", null));
            MenuItems.Add(new MenuItemModel(LanguageManager.Instance.Instagram.Directs, "\uE15F", typeof(DirectsListView)));
            FooterMenuItems.Add(new MenuItemModel(LanguageManager.Instance.General.Settings, new AnimatedSettingsVisualSource(), typeof(SettingsView)));
            ToggleNavigationViewPane = new RelayCommand(ToggleNavigationPane);
            _themeListener.ThemeChanged += MainPageViewModel_ThemeChanged;
            new Thread(SyncLauncher).Start();
            new Thread(GetMyUser).Start();
        }

        async void StartPushClient()
        {
            var apis = await ApplicationSettingsManager.Instance.GetUsersApiListAsync();
            IInstaApi Api = App.Container.GetService<IInstaApi>();

            Api.PushClient = new PushClient(apis, Api);
            Api.PushClient.MessageReceived += PushClient_MessageReceived;
            await Api.PushProcessor.RegisterPushAsync();
            Api.PushClient.Start();
        }

        async void PushClient_MessageReceived(object sender, PushReceivedEventArgs e)
        {
            if (e == null || e.NotificationContent == null) return;
            var apis = await ApplicationSettingsManager.Instance.GetUsersApiListAsync();
            PushHelper.HandleNotify(e.NotificationContent, apis);
        }

        async void SyncLauncher()
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                await Api.LauncherSyncAsync();
                await Api.PushProcessor.RegisterPushAsync();
            }
            StartPushClient();
        }

        async void GetMyUser()
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.UserProcessor.GetCurrentUserAsync();
                UIContext.Post((a) =>
                {
                    if (!result.Succeeded)
                    {
                        InstaUser = App.Container.GetService<InstaUserShort>();
                        return;
                    }
                    InstaUser = result.Value;
                    ((App)App.Current).SetMyUserInstance(result.Value);
                }, null);
            }
        }

        void NavigateToUserProfile(object obj)
        {
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }

        bool SuggestionChosen = false;
        private void SearchBoxSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs arg)
        {
            SuggestionChosen = true;
            var user = (InstaUser)arg.SelectedItem;
            NavigationService.Navigate(typeof(UserProfileView), user);
            SearchQuery = String.Empty;
        }

        private void SearchBoxQuerySubmitted(AutoSuggestBoxQuerySubmittedEventArgs arg)
        {
            if (SuggestionChosen) { SuggestionChosen = false; return; }
            if (string.IsNullOrEmpty(SearchQuery)) return;
            NavigationService.Navigate(typeof(SearchView), SearchQuery);
            SearchQuery = String.Empty;
        }

        Stopwatch stopwatch = null;
        private async Task SearchBoxTextChanged(AutoSuggestBoxTextChangedEventArgs arg)
        {
            if (arg.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
            if (stopwatch == null)
                stopwatch = Stopwatch.StartNew();
            else stopwatch.Restart();
            await Task.Delay(400);
            if (stopwatch.ElapsedMilliseconds < 400) return;
            var offset = DateTime.Now.Subtract(DateTime.UtcNow).TotalSeconds;
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.DiscoverProcessor.SearchPeopleAsync(SearchQuery,
                       PaginationParameters.MaxPagesToLoad(1));
                    if (result.Succeeded)
                    {
                        SearchResults.Clear();
                        SearchResults.AddRange(result.Value.Users);
                    }
                }
            }
            finally { }
        }

        void ToggleNavigationPane()
        {
            IsNavigationViewPaneOpened = !IsNavigationViewPaneOpened;
        }

        bool ignoreSetMenuItem = false;
        private void FrameNavigated(NavigationEventArgs obj)
        {
            switch (obj.Content.GetType().Name)
            {
                case "HomeView":
                    SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(HomeView));
                    break;

                case "DirectsListView":
                    SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(DirectsListView));
                    break;

                case "SettingsView":
                    SelectedMenuItem = FooterMenuItems.FirstOrDefault(x => x.View == typeof(SettingsView));
                    break;

                default:
                    break;
            }
        }

        private void MainPageViewModel_ThemeChanged(ThemeListener sender)
        {
            UIContext.Post(new SendOrPostCallback(ApplyThemeForTitleBarButtons), null);
        }

        void SelectedMenuItemChanged()
        {
            if (SelectedMenuItem == null) return;
            if (ignoreSetMenuItem) { ignoreSetMenuItem = false; return; }
            if (NavigationService.Content != null && NavigationService.Content.GetType() == SelectedMenuItem.View) return;
            if (SelectedMenuItem.View != null)
            {
                WeakReferenceMessenger.Default.Send(new NavigateToPageMessage(SelectedMenuItem.View));
            }
        }

        void SetupTitlebar(CoreApplicationViewTitleBar coreTitleBar)
        {
            coreTitleBar.ExtendViewIntoTitleBar = true;
            coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            ApplyThemeForTitleBarButtons();
        }

        private void ApplyThemeForTitleBarButtons(object noUse = null)
        {
            ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
            var theme = ApplicationSettingsManager.Instance.GetTheme();

            if (theme == AppTheme.Default)
            {
                theme = _themeListener.CurrentTheme == ApplicationTheme.Light ? AppTheme.Light : AppTheme.Dark;
            }

            if (theme == AppTheme.Dark)
            {
                // Set active window colors
                titleBar.ButtonForegroundColor = Colors.White;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = Colors.White;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 90, 90, 90);
                titleBar.ButtonPressedForegroundColor = Colors.White;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 120, 120, 120);

                // Set inactive window colors
                titleBar.InactiveForegroundColor = Colors.Gray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.Gray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                titleBar.BackgroundColor = Color.FromArgb(255, 45, 45, 45);
            }
            else if (theme == AppTheme.Light)
            {
                // Set active window colors
                titleBar.ButtonForegroundColor = Colors.Black;
                titleBar.ButtonBackgroundColor = Colors.Transparent;
                titleBar.ButtonHoverForegroundColor = Colors.Black;
                titleBar.ButtonHoverBackgroundColor = Color.FromArgb(255, 180, 180, 180);
                titleBar.ButtonPressedForegroundColor = Colors.Black;
                titleBar.ButtonPressedBackgroundColor = Color.FromArgb(255, 150, 150, 150);

                // Set inactive window colors
                titleBar.InactiveForegroundColor = Colors.DimGray;
                titleBar.InactiveBackgroundColor = Colors.Transparent;
                titleBar.ButtonInactiveForegroundColor = Colors.DimGray;
                titleBar.ButtonInactiveBackgroundColor = Colors.Transparent;

                titleBar.BackgroundColor = Color.FromArgb(255, 210, 210, 210);
            }
        }


        private void CoreTitleBar_LayoutMetricsChanged(CoreApplicationViewTitleBar sender, object args)
        {
            SetupTitlebar(sender);
        }

    }
}
