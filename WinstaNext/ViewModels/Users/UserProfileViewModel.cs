using Abstractions.Stories;
using Core.Collections;
using Core.Collections.IncrementalSources.Highlights;
using Core.Collections.IncrementalSources.Users;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PropertyChanged;
using Resources;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Attributes;
using Abstractions.Navigation;
using WinstaNext.Helpers.ExtensionMethods;
using WinstaNext.Models.Core;
using WinstaNext.Views.Media;
using WinstaNext.Views.Profiles;
using ViewModels;

namespace WinstaNext.ViewModels.Users
{
    public class UserProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        IncrementalUserHighlights HighlightsInstance { get; set; }
        IncrementalUserMedias MediasInstance { get; set; }
        IncrementalUserReels ReelsInstance { get; set; }
        IncrementalUserTaggedMedia TaggedInstance { get; set; }
        IncrementalUserTVMedias IGTVInstance { get; set; }

        public IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem> HighlightFeeds { get; set; }
        RangePlayerAttribute UserReels { get; set; }
        RangePlayerAttribute UserMedias { get; set; }
        RangePlayerAttribute UserTaggedMedias { get; set; }
        RangePlayerAttribute TVMedias { get; set; }

        public InstaStoryAndLives StoriesAndLives { get; set; }

        public RangePlayerAttribute ItemsSource { get; set; }

        public InstaUserInfo User { get; private set; }

        public ScrollViewer ListViewScroll { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToMediaCommand { get; set; }
        public RelayCommand<LinkClickedEventArgs> CaptionLinkClickedCommand { get; set; }
        public AsyncRelayCommand<string> ExternalLinkClickCommand { get; set; }
        public RelayCommand NavigateToFollowingsCommand { get; set; }
        public RelayCommand NavigateToFollowersCommand { get; set; }

        public AsyncRelayCommand FollowButtonCommand { get; set; }

        public ExtendedObservableCollection<MenuItemModel> ProfileTabs { get; set; } = new();

        [OnChangedMethod(nameof(ONSelectedTabChanged))]
        public MenuItemModel SelectedTab { get; set; }
        
        public UserProfileViewModel() : base()
        {
            CaptionLinkClickedCommand = new(CaptionLinkClicked);
            ExternalLinkClickCommand = new(ExternalLinkClickAsync);
            NavigateToMediaCommand = new(NavigateToMedia);
            FollowButtonCommand = new(FollowButtonFuncAsync);
            NavigateToFollowingsCommand = new(NavigateToFollowings);
            NavigateToFollowersCommand = new(NavigateToFollowers);
        }

        void NavigateToFollowings()
        {
            NavigationService.Navigate(typeof(UserFollowingsView), User.Pk);
        }

        void NavigateToFollowers()
        {
            NavigationService.Navigate(typeof(UserFollowersView), User.Pk);
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
            if (User.Pk == App.Container.GetService<InstaUserShort>().Pk)
            {
                //Edit profile
                throw new NotImplementedException();
            }
            if (User.FriendshipStatus == null)
                throw new ArgumentNullException(nameof(User.FriendshipStatus));

            var follow = !User.FriendshipStatus.Following;

            IResult<InstaFriendshipFullStatus> result;
            using (IInstaApi Api = App.Container?.GetService<IInstaApi>())
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
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            await Task.Delay(10);
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
                }
            }
            else if(e.Parameter is InstaUserShort instaUsershort)
            {
                if (User != null && User.Pk == instaUsershort.Pk) return;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(instaUsershort.Pk,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value;
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
                    var StoriesAndLivesResult = await Api.StoryProcessor.GetUserStoryAndLivesAsync(User.Pk);
                    if (StoriesAndLivesResult.Succeeded)
                    {
                        StoriesAndLives = StoriesAndLivesResult.Value;
                    }
                }
            }
            HighlightsInstance = new(User.Pk, AppCore.IsDark);
            ReelsInstance = new(User.Pk);
            MediasInstance = new(User.Pk);
            TaggedInstance = new(User.Pk);
            IGTVInstance = new(User.Pk);

            HighlightFeeds = new(HighlightsInstance);
            UserReels = new(ReelsInstance);
            UserMedias = new(MediasInstance);
            UserTaggedMedias = new(TaggedInstance);
            TVMedias = new(IGTVInstance);
            CreateProfileTabs();
            ListViewScroll.ChangeView(null, 0, null);
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
            SelectedTab = null;
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
