using System.Linq;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using PropertyChanged;
using Windows.ApplicationModel.Core;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using System.Diagnostics;
using InstagramApiSharp;
using InstagramApiSharp.API.Push;
using WinstaBackgroundHelpers.Push;
using Windows.System;
using WinstaCore.Theme;
using Core.Collections;
using WinstaCore;
using Resources;
using WinstaCore.Interfaces.Views;
using WinstaCore.Interfaces.Views.Activities;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Interfaces.Views.Directs;
using WinstaCore.Interfaces.Views.Settings;
using WinstaCore.Interfaces;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore.Interfaces.Views.Search;
using NotificationHandler;
using Microsoft.UI.Xaml.Controls;
#if !WINDOWS_UWP15063
using System.Threading;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Controls.AnimatedVisuals;
#else
using System.Threading;
using System.Threading.Tasks;
#endif
using InstagramApiSharp.Classes;
using WinstaCore.Interfaces.Views.Accounts;
using System;
using Windows.Storage.Pickers;
using System.IO;
using WinstaCore.Converters.FileConverters;
using WinstaCore.Services;
using WinstaCore.Models.ConfigureDelays;
using WinstaCore.Interfaces.Views.Medias.Upload;
#nullable enable

namespace ViewModels;

public class MainPageViewModel : BaseViewModelWithStopwatch
{
    readonly ThemeListener _themeListener = new();
    public string SearchQuery { get; set; } = string.Empty;
    public string WindowTitle { get; set; } = LanguageManager.Instance.General.ApplicationName;
    public bool IsNavigationViewPaneOpened { get; set; }

    public RelayCommand ToggleNavigationViewPane { get; }
    public AsyncRelayCommand<AutoSuggestBoxTextChangedEventArgs> SearchBoxTextChangedCommand { get; }
    public RelayCommand<AutoSuggestBoxQuerySubmittedEventArgs> SearchBoxQuerySubmittedCommand { get; }
    public RelayCommand<AutoSuggestBoxSuggestionChosenEventArgs> SearchBoxSuggestionChosenCommand { get; }
    public RelayCommand<NavigationEventArgs> FrameNavigatedCommand { get; }
    public RelayCommand<object> NavigateToUserProfileCommand { get; }
    public AsyncRelayCommand<object?> UploadStoryCommand { get; set; }
    public AsyncRelayCommand<object?> UploadPostCommand { get; set; }

    /// <summary>
    /// Items at the top of the NavigationView.
    /// </summary>
    public ExtendedObservableCollection<MenuItemModel> MenuItems { get; } = new();

    /// <summary>
    /// Gets or sets the list of items to displayed in the Search Box after a search.
    /// </summary>
    public ExtendedObservableCollection<InstaUser> SearchResults { get; } = new();

    /// <summary>
    /// Items at the bottom of the NavigationView.
    /// </summary>
    public ExtendedObservableCollection<MenuItemModel> FooterMenuItems { get; } = new();

    /// <summary>
    /// Gets or sets the selected menu item in the NavitationView.
    /// </summary>
    [OnChangedMethod(nameof(SelectedMenuItemChanged))]
    public MenuItemModel? SelectedMenuItem { get; set; }

    public Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode NavigationViewDisplayMode { get; set; }

    [OnChangedMethod(nameof(OnInstaUserChanged))]
    public InstaUserShort? InstaUser { get; private set; }

    IInstaApi? PushClientApi { get; set; }

    public static MainPageViewModel? mainPageViewModel = null;
    SystemNavigationManager? SystemNavigationManager = null;

    public event EventHandler? ForceLogout;

    public void RaiseForceLogoutEvent() => ForceLogout?.Invoke(this, EventArgs.Empty);

    public MainPageViewModel()
    {
        mainPageViewModel = this;

        SearchBoxTextChangedCommand = new(SearchBoxTextChanged);
        SearchBoxQuerySubmittedCommand = new(SearchBoxQuerySubmitted);
        SearchBoxSuggestionChosenCommand = new(SearchBoxSuggestionChosen);
        UploadPostCommand = new(UploadPostAsync);
        UploadStoryCommand = new(UploadStoryAsync);

        FrameNavigatedCommand = new(FrameNavigated);
        NavigateToUserProfileCommand = new(NavigateToUserProfile);
        SetupTitlebar(CoreApplication.GetCurrentView().TitleBar);
        MenuItems.Add(new(LanguageManager.Instance.General.Home, "\uE10F", typeof(IHomeView)));
        MenuItems.Add(new(LanguageManager.Instance.Instagram.Activities, "\uE006", typeof(IActivitiesView)));
        MenuItems.Add(new(LanguageManager.Instance.Instagram.Explore, "\uF6FA", typeof(IExploreView)));
        MenuItems.Add(new(LanguageManager.Instance.Instagram.Directs, "\uE15F", typeof(IDirectsListView)));
        MenuItems.Add(new(LanguageManager.Instance.Instagram.UploadFeed, "\uE11C", command: UploadPostCommand));
        //MenuItems.Add(new(LanguageManager.Instance.Instagram.UploadStory, "\uE11C", command: UploadStoryCommand));

#if !WINDOWS_UWP15063
        FooterMenuItems.Add(new(LanguageManager.Instance.General.Settings, typeof(ISettingsView)) { Icon = new AnimatedIcon { Source = new AnimatedSettingsVisualSource() } });
#else   
        //MenuItems.Add(new(LanguageManager.Instance.General.Settings, "\uE713", typeof(ISettingsView)));
        FooterMenuItems.Add(new(LanguageManager.Instance.General.Settings, "\uE713", typeof(ISettingsView)));
#endif

        ToggleNavigationViewPane = new(ToggleNavigationPane);
        _themeListener.ThemeChanged += MainPageViewModel_ThemeChanged;

#if !WINDOWS_UWP15063
        new Thread(SyncLauncher).Start();
        new Thread(GetDirectsCountAsync).Start();
        new Thread(GetMyUser).Start();
#else
        Task.Factory.StartNew(SyncLauncher);
        Task.Factory.StartNew(GetDirectsCountAsync);
        Task.Factory.StartNew(GetMyUser);
#endif
        SystemNavigationManager = SystemNavigationManager.GetForCurrentView();
        SystemNavigationManager.BackRequested += MainPageViewModel_BackRequested;
        Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
    }

    async Task UploadPostAsync(object? obj)
    {
        try
        {
            FileOpenPicker fop = new()
            {
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
                ViewMode = PickerViewMode.Thumbnail,
            };
            //fop.FileTypeFilter.Add(".jpg");
            //fop.FileTypeFilter.Add(".png");
            //fop.FileTypeFilter.Add(".bmp");
            fop.FileTypeFilter.Add(".mp4");
            var file = await fop.PickSingleFileAsync();
            if (file == null) return;
            var MediaCropperView = AppCore.Container.GetService<IMediaCropperView>();
            NavigationService.Navigate(MediaCropperView, file);
            return;
            //using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            //Api.SetConfigureMediaDelay(new ImageConfigureMediaDelay());
            //var props = await file.Properties.GetImagePropertiesAsync();
            //var upload = new InstaImageUpload
            //{
            //    ImageBytes = await ImageFileConverter.ConvertImageToJpegAsync(file),
            //    Width = (int)props.Width,
            //    Height = (int)props.Height
            //};
            //var res = await Api.MediaProcessor.UploadPhotoAsync(upload, string.Empty);
            //if (res.Succeeded)
            //{
            //    var SingleInstaMediaView = AppCore.Container.GetService<ISingleInstaMediaView>();
            //    NavigationService.Navigate(SingleInstaMediaView, res.Value);
            //}
            //else
            //{
            //    throw res.Info.Exception;
            //}
        }
        finally
        {
            SelectMenuItem(null);
        }
    }

    async Task UploadStoryAsync(object? obj)
    {
        await Task.Delay(1);
    }

    private void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
    {
        if (args.VirtualKey == VirtualKey.Escape)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }
        args.Handled = true;
    }

    private void MainPageViewModel_BackRequested(object sender, BackRequestedEventArgs e)
    {
        if (NavigationService.CanGoBack)
        {
            NavigationService.GoBack();
            e.Handled = true;
        }
    }

    ~MainPageViewModel()
    {
        mainPageViewModel = null;
    }

    async void GetDirectsCountAsync()
    {
        using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
        {
            var result = await Api.MessagingProcessor
                                  .GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
            if (!result.Succeeded) return;
            var value = result.Value;
            var count = value.PendingRequestsCount + value.Inbox.UnseenCount;

            UIContext.Post((e) =>
            {
                var DirectsText = LanguageManager.Instance.Instagram.Directs;
                var menu = MenuItems.FirstOrDefault(x => x.Text == DirectsText);
                if (menu != null)
                    menu.Badge = count.ToString();
            }, null);
        }
    }

    async void StartPushClient()
    {
        var apis = await ApplicationSettingsManager.Instance.GetUsersApiListAsync();
        PushClientApi = AppCore.Container.GetService<IInstaApi>();

        PushClientApi.PushClient = new PushClient(apis, PushClientApi);
        PushClientApi.PushClient.MessageReceived += PushClient_MessageReceived;
        await PushClientApi.PushProcessor.RegisterPushAsync();
        PushClientApi.PushClient.Start();
    }

    public void StopPushClient()
    {
        if (PushClientApi == null) return;
        PushClientApi.PushClient.MessageReceived -= PushClient_MessageReceived;
        PushClientApi.PushClient.Shutdown();
        PushClientApi.Dispose();
    }

    public void RemoveNavigationEvents()
    {
        SystemNavigationManager ??= SystemNavigationManager.GetForCurrentView();
        SystemNavigationManager.BackRequested -= MainPageViewModel_BackRequested;
        SystemNavigationManager = null;
    }

    async void PushClient_MessageReceived(object sender, PushReceivedEventArgs e)
    {
        if (e == null || e.NotificationContent == null) return;

        UIContext.Post((s) =>
        {
            var directsmenu = MenuItems.FirstOrDefault(x => x.Text == LanguageManager.Instance.Instagram.Directs);
            directsmenu.Badge = $"{e.NotificationContent.BadgeCount.Direct}";

            var activitiesmenu = MenuItems.FirstOrDefault(x => x.Text == LanguageManager.Instance.Instagram.Activities);
            activitiesmenu.Badge = $"{e.NotificationContent.BadgeCount.Activities}";
        }, null);

        var apis = await ApplicationSettingsManager.Instance.GetUsersApiListAsync();
        PushHelper.HandleNotify(e.NotificationContent, apis);
    }

    async void SyncLauncher()
    {
        using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
        {
            await Api.LauncherMobileConfigAsync();
            var syncres = await Api.PushProcessor.RegisterPushAsync();
            if (!syncres.Succeeded)
            {
                if (syncres.Info.ResponseType == ResponseType.LoginRequired)
                {
                    //Logout user
                    ApplicationSettingsManager.Instance.RemoveUser(Api.GetLoggedUser().LoggedInUser.Pk);
                    if (ApplicationSettingsManager.Instance.GetUsersList().Any())
                        ForceLogout?.Invoke(this, EventArgs.Empty);
                    else NavigationService.Navigate(AppCore.Container.GetService<ILoginView>());
                    return;
                }
                if (syncres.Info.ResponseType == ResponseType.ChallengeRequired)
                {
                    // handle birthday challenge
                    if (syncres.Info.Challenge != null)
                    {
                        var challenge = await Api.GetChallengeRequireVerifyMethodAsync();
                        if (challenge.Succeeded)
                        {
                            if (challenge.Value.IsBirthdayChallenge)
                            {
                                // this function will automatically set an birthday to your account,
                                // so you don't need to do anything
                                await Api.GetDeltaChallengeAsync();
                            }

                            return;
                        }
                    }

                    var challengeData = await Api.GetLoggedInChallengeDataInfoAsync();
                    // Do something to challenge data, if you want!

                    var acceptChallenge = await Api.AcceptChallengeAsync();
                    // If Succeeded was TRUE, you can continue to your work!
                    if (!acceptChallenge.Succeeded)
                    {
                        var ChallengeRequiredView = AppCore.Container.GetService<IChallengeRequiredView>();
                        NavigationService.Navigate(ChallengeRequiredView, Api);
                    }
                }
            }
        }
        StartPushClient();
    }

    async void GetMyUser()
    {
        using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
        {
            var result = await Api.UserProcessor.GetCurrentUserAsync();
            UIContext.Post((a) =>
            {
                if (!result.Succeeded)
                {
                    InstaUser = AppCore.Container.GetService<InstaUserShort>();
                    ApplicationSettingsManager.Instance.SetLastLoggedUser(InstaUser.Pk.ToString());
                    return;
                }
                InstaUser = result.Value;
                var App = AppCore.Container.GetService<IWinstaApp>();
                App.SetMyUserInstance(result.Value);
                ApplicationSettingsManager.Instance.SetLastLoggedUser(result.Value.Pk.ToString());
            }, null);
        }
    }
    async void OnInstaUserChanged()
    {
        if (InstaUser == null) return;
        try
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                Api.UpdateUser(InstaUser);
                var state = Api.GetStateDataAsString();
                await ApplicationSettingsManager.Instance.
                            AddOrUpdateUser(InstaUser.Pk, state, InstaUser.UserName);
            }
        }
        finally
        {

        }
    }

    void NavigateToUserProfile(object? obj)
    {
        if (obj == null) return;
        var IUserProfileView = AppCore.Container.GetService<IUserProfileView>();
        NavigationService.Navigate(IUserProfileView, obj);
    }

    bool SuggestionChosen = false;
    private void SearchBoxSuggestionChosen(AutoSuggestBoxSuggestionChosenEventArgs? arg)
    {
        if (arg == null) return;
        SuggestionChosen = true;
        var user = (InstaUser)arg.SelectedItem;
        var IUserProfileView = AppCore.Container.GetService<IUserProfileView>();
        NavigationService.Navigate(IUserProfileView, user);
        SearchQuery = string.Empty;
    }

    private void SearchBoxQuerySubmitted(AutoSuggestBoxQuerySubmittedEventArgs? arg)
    {
        if (SuggestionChosen) { SuggestionChosen = false; return; }
        if (string.IsNullOrEmpty(SearchQuery)) return;
        var ISearchView = AppCore.Container.GetService<ISearchView>();
        NavigationService.Navigate(ISearchView, SearchQuery);
        SearchQuery = string.Empty;
    }

    private async Task SearchBoxTextChanged(AutoSuggestBoxTextChangedEventArgs? arg)
    {
        if (arg == null || arg.Reason != AutoSuggestionBoxTextChangeReason.UserInput) return;
        if (!await base.EnsureTimeElapsed()) return;
        try
        {
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
            var result = await Api.DiscoverProcessor.TypeaheadSearchAsync(SearchQuery);
            if (result.Succeeded)
            {
                SearchResults.Clear();
                SearchResults.AddRange(result.Value.Users);
            }
        }
        finally { }
    }

    void ToggleNavigationPane()
    {
        IsNavigationViewPaneOpened = !IsNavigationViewPaneOpened;
    }

    bool ignoreSetMenuItem = false;
    void SelectMenuItem(object? obj)
    {
        obj ??= NavigationService.Content;
        switch (obj)
        {
            case IHomeView:
                SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(IHomeView));
                break;
            case IActivitiesView:
                SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(IActivitiesView));
                break;
            case IDirectsListView:
                SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(IDirectsListView));
                break;
            case ISettingsView:
                SelectedMenuItem = MenuItems.FirstOrDefault(x => x.View == typeof(ISettingsView));
                break;
            default:
                break;
        }
    }

    private void FrameNavigated(NavigationEventArgs? obj)
    {
        if (obj == null) return;
        SelectMenuItem(obj.Content);
    }

    private void MainPageViewModel_ThemeChanged(ThemeListener sender)
    {
        UIContext.Post(new SendOrPostCallback(ApplyThemeForTitleBarButtons), null);
    }

    public void SelectedMenuItemChanged()
    {
        if (SelectedMenuItem == null) return;
        if (ignoreSetMenuItem) { ignoreSetMenuItem = false; return; }
        if (NavigationService.Content != null && NavigationService.Content.GetType() == SelectedMenuItem.View) return;
        if (SelectedMenuItem.View != null)
        {
            var page = (IView)AppCore.Container.GetService(SelectedMenuItem.View);
            NavigationService.Navigate(page);
        }
        else SelectedMenuItem.Command?.Execute(null);
    }

    void SetupTitlebar(CoreApplicationViewTitleBar coreTitleBar)
    {
        coreTitleBar.ExtendViewIntoTitleBar = true;
        coreTitleBar.LayoutMetricsChanged += CoreTitleBar_LayoutMetricsChanged;
        SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        ApplyThemeForTitleBarButtons();
    }

    private void ApplyThemeForTitleBarButtons(object? noUse = null)
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
