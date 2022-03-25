using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Media;
using WinstaNext.Core.Collections.IncrementalSources.Media;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Media
{
    internal class MediaLikersViewModel : BaseViewModel
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
            NavigationService.Navigate(typeof(UserProfileView), (obj.ClickedItem as WinstaMediaLikerUser).User);
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
