using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections;
using WinstaNext.Core.Collections.IncrementalSources.Users;
using WinstaNext.Core.Navigation;
using WinstaNext.Helpers.ExtensionMethods;
using WinstaNext.Models.Core;
using WinstaNext.Services;
using WinstaNext.Views;
using WinstaNext.Views.Media;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Users
{
    public class UserProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        public string FollowBtnContent { get; set; }

        IncrementalUserMedias MediasInstance { get; set; }
        IncrementalUserReels ReelsInstance { get; set; }
        IncrementalUserTaggedMedia TaggedInstance { get; set; }
        IncrementalUserTVMedias IGTVInstance { get; set; }

        IncrementalLoadingCollection<IncrementalUserReels, InstaMedia> UserReels { get; set; }
        IncrementalLoadingCollection<IncrementalUserMedias, InstaMedia> UserMedias { get; set; }
        IncrementalLoadingCollection<IncrementalUserTaggedMedia, InstaMedia> UserTaggedMedias { get; set; }
        IncrementalLoadingCollection<IncrementalUserTVMedias, InstaMedia> TVMedias { get; set; }

        public ISupportIncrementalLoading ItemsSource { get; set; }

        public InstaUserInfo User { get; private set; }

        public ScrollViewer ListViewScroll { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToMediaCommand { get; set; }
        public RelayCommand<LinkClickedEventArgs> CaptionLinkClickedCommand { get; set; }
        public AsyncRelayCommand<string> ExternalLinkClickCommand { get; set; }

        public AsyncRelayCommand FollowButtonCommand { get; set; }

        public ExtendedObservableCollection<MenuItemModel> ProfileTabs { get; set; } = new();

        [OnChangedMethod(nameof(ONSelectedTabChanged))]
        public MenuItemModel SelectedTab { get; set; }
        ~UserProfileViewModel()
        {

        }
        public UserProfileViewModel() : base()
        {
            CaptionLinkClickedCommand = new(CaptionLinkClicked);
            ExternalLinkClickCommand = new(ExternalLinkClickAsync);
            NavigateToMediaCommand = new(NavigateToMedia);
            FollowButtonCommand = new(FollowButtonFuncAsync);
        }

        async Task ExternalLinkClickAsync(string link)
            => await Launcher.LaunchUriAsync(new Uri(link, UriKind.RelativeOrAbsolute));

        void CaptionLinkClicked(LinkClickedEventArgs obj)
            => obj.HandleClickEvent(NavigationService);

        void NavigateToMedia(ItemClickEventArgs args)
        {
            if (args.ClickedItem is not InstaMedia media) throw new ArgumentOutOfRangeException(nameof(args.ClickedItem));
            //var index = UserMedias.IndexOf(media);
            var para = new IncrementalMediaViewParameter(ItemsSource, media);
            NavigationService.Navigate(typeof(IncrementalInstaMediaView), para);
        }

        async Task FollowButtonFuncAsync()
        {
            if (FollowButtonCommand.IsRunning) return;
            if (FollowBtnContent == LanguageManager.Instance.Instagram.EditProfile)
            {
                throw new NotImplementedException();
            }
            var follow = !string.IsNullOrEmpty(FollowBtnContent) &&
                         ((FollowBtnContent == LanguageManager.Instance.Instagram.Follow) ||
                         (FollowBtnContent == LanguageManager.Instance.Instagram.FollowBack));

            IResult<InstaFriendshipFullStatus> result;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                if (follow)
                    result = await Api.UserProcessor.FollowUserAsync(User.Pk,
                             surfaceType: InstaMediaSurfaceType.Profile,
                             mediaIdAttribution: null);
                else result = await Api.UserProcessor.UnFollowUserAsync(User.Pk,
                             surfaceType: InstaMediaSurfaceType.Profile,
                             mediaIdAttribution: null);
            }
            if (!result.Succeeded) throw result.Info.Exception;
            User.FriendshipStatus = result.Value.Adapt<InstaStoryFriendshipStatus>();
            SetFollowButtonContent();
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            if (e.Parameter is long userId)
            {
                if (User != null && User.Pk == userId) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(userId,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
                    //User = result.Value.Adapt<InstaUser>();
                }
            }
            else if (e.Parameter is string username && !string.IsNullOrEmpty(username))
            {
                if (User != null && User.UserName.ToLower() == username.ToLower()) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByUsernameAsync(username,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
                    //User = result.Value.Adapt<InstaUser>();
                }
            }
            else if (e.Parameter is InstaUserShortFriendshipFull friendshipFull)
            {
                if (User != null && User.Pk == friendshipFull.Pk) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(friendshipFull.Pk,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
                    //User = result.Value.Adapt<InstaUser>();
                }
            }
            else if (e.Parameter is InstaCurrentUser currentUser)
            {
                if (User != null && User.Pk == currentUser.Pk) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(currentUser.Pk,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
                    //User = result.Value.Adapt<InstaUser>();
                }
            }
            else if (e.Parameter is InstaUser instaUser)
            {
                if (User != null && User.Pk == instaUser.Pk) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(instaUser.Pk,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
                    //User = result.Value.Adapt<InstaUser>();
                }
            }
            else
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }
            if (User.FriendshipStatus == null)
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetFriendshipStatusAsync(User.Pk);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User.FriendshipStatus = result.Value;
                }
            }
            SetFollowButtonContent();
            ReelsInstance = new(User.Pk);
            MediasInstance = new(User.Pk);
            TaggedInstance = new(User.Pk);
            IGTVInstance = new(User.Pk);
            UserReels = new(ReelsInstance);
            UserMedias = new(MediasInstance);
            UserTaggedMedias = new(TaggedInstance);
            TVMedias = new(IGTVInstance);
            CreateProfileTabs();
            await base.OnNavigatedToAsync(e);
        }

        void ONSelectedTabChanged()
        {
            if (SelectedTab == null) return;

            if (SelectedTab.Text == LanguageManager.Instance.Instagram.Posts)
            {
                ItemsSource = UserMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Reels)
            {
                ItemsSource = UserReels;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.IGTV)
            {
                ItemsSource = TVMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Tagged)
            {
                ItemsSource = UserTaggedMedias;
            }
        }

        void CreateProfileTabs()
        {
            if (ProfileTabs.Any()) ProfileTabs.Clear();
            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Posts, "\uF0E2"));

            if (User.TotalClipsCount != 0)
                ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Reels, "\uE102"));

            if (User.TotalIGTVVideos != 0)
                ProfileTabs.Add(new(LanguageManager.Instance.Instagram.IGTV, "\uE7F4"));

            if (User.UsertagsCount != 0)
                ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Tagged, "\uE168"));

            if (SelectedTab != null)
                SelectedTab = null;

            SelectedTab = ProfileTabs.FirstOrDefault();
        }

        void SetFollowButtonContent()
        {
            if (User.Pk == App.Container.GetService<InstaUserShort>().Pk)
                FollowBtnContent = LanguageManager.Instance.Instagram.EditProfile;
            else if (!User.FriendshipStatus.Following && !User.FriendshipStatus.FollowedBy)
                FollowBtnContent = LanguageManager.Instance.Instagram.Follow;
            else if (!User.FriendshipStatus.Following && User.FriendshipStatus.FollowedBy)
                FollowBtnContent = LanguageManager.Instance.Instagram.FollowBack;
            else if (User.FriendshipStatus.Following)
                FollowBtnContent = LanguageManager.Instance.Instagram.Unfollow;
            else if (User.FriendshipStatus.OutgoingRequest)
                FollowBtnContent = LanguageManager.Instance.Instagram.Requested;
        }

        private void UserProfileViewModel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewHeight = e.NewSize.Height;
            ViewWidth = e.NewSize.Width;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            (NavigationService.Content as UserProfileView).SizeChanged += UserProfileViewModel_SizeChanged;
            base.OnNavigatedTo(e);
        }

        public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            (NavigationService.Content as UserProfileView).SizeChanged -= UserProfileViewModel_SizeChanged;
            base.OnNavigatingFrom(e);
        }
    }
}
