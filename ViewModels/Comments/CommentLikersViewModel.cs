using Core.Collections.IncrementalSources.Comments;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Resources;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views.Profiles;

namespace ViewModels.Comments
{
    public class CommentLikersViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.CommentLikers;

        IncrementalCommentLikers Instance { get; set; }
        public IncrementalLoadingCollection<IncrementalCommentLikers, InstaUserShort> CommentLikers { get; private set; }

        public RelayCommand<ItemClickEventArgs> NavigateToUserCommand { get; set; }

        public CommentLikersViewModel()
        {
            NavigateToUserCommand = new(NavigateToUser);
        }

        void NavigateToUser(ItemClickEventArgs obj)
        {
            var UserProfileView = AppCore.Container.GetService<IUserProfileView>();
            NavigationService.Navigate(UserProfileView, obj.ClickedItem);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (Instance != null && Instance.CommentPk == e.Parameter.ToString()) return;
            Instance = new(e.Parameter.ToString());
            CommentLikers = new(Instance);
            base.OnNavigatedTo(e);
        }
    }
}
