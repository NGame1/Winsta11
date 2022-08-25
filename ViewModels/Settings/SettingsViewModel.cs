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
        public bool AutoPlayEnabled
        {
            get => ApplicationSettingsManager.Instance.GetAutoPlay();
            set => ApplicationSettingsManager.Instance.SetAutoPlay(value);
        }

        public string DownloadsPath
        {
            get; set;
        }

        public bool ForceThreeColumns
        {
            get => ApplicationSettingsManager.Instance.GetForceThreeColumns();
            set => ApplicationSettingsManager.Instance.SetForceThreeColumns(value);
        }

        public bool RemoveFeedAds
        {
            get => ApplicationSettingsManager.Instance.GetRemoveFeedAds();
            set => ApplicationSettingsManager.Instance.SetRemoveFeedAds(value);
        }

        [OnChangedMethod(nameof(OnThemeChanged))]
        public AppTheme Theme { get; set; }

        [OnChangedMethod(nameof(OnQualityChanged))]
        public string PlaybackQuality { get; set; }

        public List<LanguageDefinition> AvailableLanguages { get; } = new();
        public List<string> AvailableQualities { get; } = new();

        [OnChangedMethod(nameof(OnLanguageChanged))]
        public LanguageDefinition Language { get; set; }

        public AsyncRelayCommand SetDownloadsFolderCommand { get; set; }

        public RelayCommand AccountSettingsNavigateCommand { get; set; }

        public SettingsViewModel() : base()
        {
            AccountSettingsNavigateCommand = new(AccountSettingsNavigate);
            SetDownloadsFolderCommand = new(SetDownloadsFolderAsync);
            Theme = ApplicationSettingsManager.Instance.GetTheme();
            var q = ApplicationSettingsManager.Instance.GetPlaybackQuality();
            var langs = ApplicationSettingsManager.Instance.GetSupportedLanguages();
            var currentlang = ApplicationSettingsManager.Instance.GetLanguage();
            AvailableLanguages.AddRange(langs);
            Language = langs.FirstOrDefault(x => x.LangCode == currentlang);
            AvailableQualities.AddRange(Enum.GetNames(typeof(PlaybackQualityEnum)));
            PlaybackQuality = AvailableQualities.FirstOrDefault(x => x.ToLower() == q.ToString().ToLower());
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            var downloadsFolder = await ApplicationSettingsManager.Instance.GetDownloadsFolderAsync();
            DownloadsPath = downloadsFolder.Path;
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

        async Task SetDownloadsFolderAsync()
        {
            FolderPicker fop = new()
            {
                SuggestedStartLocation = PickerLocationId.Downloads,
                ViewMode = PickerViewMode.List
            };
            var fol = await fop.PickSingleFolderAsync();
            if (fol == null) return;
            ApplicationSettingsManager.Instance.SetDownloadsFolderAsync(fol);
            DownloadsPath = fol.Path;
        }

        void OnLanguageChanged()
        {
            ApplicationSettingsManager.Instance.SetLanguage(Language.LangCode);
        }

        void OnQualityChanged()
        {
            var q = (PlaybackQualityEnum)Enum.Parse(typeof(PlaybackQualityEnum), PlaybackQuality);
            ApplicationSettingsManager.Instance.SetPlaybackQuality(q);
        }

        void OnThemeChanged()
        {
            if (UIContext == null) return;
            ApplicationSettingsManager.Instance.SetTheme(Theme);
            switch (Theme)
            {
                case AppTheme.Default:
                    UIContext.Post(new SendOrPostCallback((s) =>
                    {
                        var el = Window.Current.Content as FrameworkElement;
                        el.RequestedTheme = ElementTheme.Default;
                    }), null);
                    break;

                case AppTheme.Light:
                    UIContext.Post(new SendOrPostCallback((s) =>
                    {
                        var el = Window.Current.Content as FrameworkElement;
                        el.RequestedTheme = ElementTheme.Light;
                    }), null);
                    break;

                case AppTheme.Dark:
                    UIContext.Post(new SendOrPostCallback((s) =>
                    {
                        var el = Window.Current.Content as FrameworkElement;
                        el.RequestedTheme = ElementTheme.Dark;
                    }), null);
                    break;

                default:
                    UIContext.Post(new SendOrPostCallback((s) =>
                    {
                        var el = Window.Current.Content as FrameworkElement;
                        el.RequestedTheme = ElementTheme.Default;
                    }), null);
                    break;
            }
        }

    }
}
