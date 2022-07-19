using Abstractions.Media;
using Core.Collections.IncrementalSources.Media;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Media
{
    public class MediaLikersViewModel : BaseViewModel
    {
        public IncrementalMediaLikers Instance { get; private set; }
        public IncrementalLoadingCollection<IncrementalMediaLikers, WinstaMediaLikerUser> MediaLikers { get; private set; }
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.MediaLikers;

        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public MediaLikersViewModel()
        {
            NavigateToUserCommand = new(NavigateToUser);
        }

        void NavigateToUser(ItemClickEventArgs obj)
        {
            var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
            NavigationService.Navigate(UserProfileView, (obj.ClickedItem as WinstaMediaLikerUser).User);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            if (e.Parameter is string mediaId)
            {
                if (string.IsNullOrEmpty(mediaId))
                {
                    NavigationService.GoBack();
                    return;
                }
                if (Instance != null && Instance.MediaId == mediaId) return;
                Instance = new(mediaId);
                MediaLikers = new(Instance);
                return;
            }
            NavigationService.GoBack();
            return;
        }
    }
}
