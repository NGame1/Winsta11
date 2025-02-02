﻿using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PropertyChanged;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Abstractions.Navigation;
using WinstaNext.UI.Dialogs;
using WinstaNext.UI.Media;
using WinstaNext.Views;
using WinstaNext.Views.Comments;
using WinstaNext.Views.Media;
using WinstaNext.Views.Profiles;
using ViewModels;
using WinstaCore.Helpers;
using WinstaCore.Helpers.ExtensionMethods;

namespace WinstaNext.ViewModels.Media
{
    public class InstaMediaPresenterUCViewModel : BaseViewModel
    {
        public bool ImagePresenterLoaded { get; set; } = false;
        public bool VideoPresenterLoaded { get; set; } = false;
        public bool CarouselPresenterLoaded { get; set; } = false;

        [AlsoNotifyFor(nameof(IsSendCommentButtonEnabled))]
        public string CommentText { get; set; } = "";

        public bool IsSendCommentButtonEnabled { get => CommentText.Length != 0; }

        public bool LoadLikeAnimation { get; set; } = false;
        public bool LoadUnLikeAnimation { get; set; } = false;

        public InstaMedia Media { get; set; }

        public AsyncRelayCommand AddCommentCommand { get; set; }
        public RelayCommand<KeyRoutedEventArgs> CommentboxKeyDownCommand { get; set; }
        public AsyncRelayCommand LikeMediaCommand { get; set; }
        public RelayCommand<RichTextBlock> MoreButtonCommand { get; set; }
        public RelayCommand NavigateToCommentsCommand { get; set; }
        public RelayCommand NavigateToMediaLikersCommand { get; set; }
        public RelayCommand<InstaUser> NavigateToUserCommand { get; set; }
        public RelayCommand<string> NavigateToHashtagCommand { get; set; }
        public RelayCommand NavigateToLocationCommand { get; set; }
        public AsyncRelayCommand SaveMediaCommand { get; set; }
        public AsyncRelayCommand ShareMediaCommand { get; set; }
        public RelayCommand<LinkClickedEventArgs> CaptionLinkClickedCommand { get; set; }

        public InstaMediaPresenterUCViewModel()
        {
            AddCommentCommand = new(AddCommentAsync);
            CommentboxKeyDownCommand = new(CommentboxKeyDown);
            CaptionLinkClickedCommand = new(CaptionLinkClicked);
            LikeMediaCommand = new(LikeMediaAsync);
            MoreButtonCommand = new(MoreButton);
            NavigateToCommentsCommand = new(NavigateToComments);
            NavigateToMediaLikersCommand = new(NavigateToMediaLikers);
            NavigateToUserCommand = new(NavigateToUser);
            NavigateToHashtagCommand = new(NavigateToHashtag);
            NavigateToLocationCommand = new(NavigateToLocation);
            SaveMediaCommand = new(SaveMediaAsync);
            ShareMediaCommand = new(ShareMediaAsync);
        }

        void CaptionLinkClicked(LinkClickedEventArgs obj)
            => obj.HandleClickEvent(NavigationService);

        async Task AddCommentAsync()
        {
            if (AddCommentCommand.IsRunning) return;
            try
            {
                InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.FeedTimeline;
                var Frame = NavigationService.Content;
                int? carouselIndex = null;
                uint feedPosition = 0;
                switch (Frame)
                {
                    case HomeView home:
                        containerModule = InstaCommentContainerModuleType.FeedTimeline;
                        feedPosition = FindMediaIndexInFeedTimeline(home);
                        carouselIndex = FindCarouselIndexInFeedTimeline(home);
                        break;
                }

                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.CommentProcessor.CommentMediaAsync(Media.InstaIdentifier, CommentText,
                      containerModule: containerModule,
                      feedPosition: feedPosition,
                      isCarouselBumpedPost: Media.MediaType == InstaMediaType.Carousel,
                      carouselIndex: carouselIndex);

                    if (!result.Succeeded)
                    {
                        MessageDialogHelper.Show(result.Info.Message);
                        return;
                    }
                    CommentText = string.Empty;
                }
            }
            finally { }
        }

        void CommentboxKeyDown(KeyRoutedEventArgs args)
        {
            if (AddCommentCommand.IsRunning) return;
            if (!IsSendCommentButtonEnabled) return;
            if (args.Key != VirtualKey.Enter) return;
            AddCommentCommand.Execute(null);
        }

        async Task LikeMediaAsync()
        {
            var liked = Media.HasLiked;
            var likesCount = Media.LikesCount;
            IResult<bool> result = null;
            try
            {
                if (!liked)
                {
                    LoadLikeAnimation = true;
                    Media.LikesCount++;
                    new Thread(UnloadLikeAnimation).Start();
                }
                else
                {
                    LoadUnLikeAnimation = true;
                    Media.LikesCount--;
                    new Thread(UnloadLikeAnimation).Start();
                }
                Media.HasLiked = !Media.HasLiked;
                var Frame = NavigationService.Content;
                InstaMediaContainerModuleType containerModule = InstaMediaContainerModuleType.None;
                InstaMediaInventorySource inventorySource = InstaMediaInventorySource.None;
                uint feedPosition = 0;
                int? carouselIndex = null;
                switch (Frame)
                {
                    case HomeView home:
                        containerModule = InstaMediaContainerModuleType.FeedTimeline;
                        feedPosition = FindMediaIndexInFeedTimeline(home);
                        carouselIndex = FindCarouselIndexInFeedTimeline(home);
                        break;
                    default:
                        break;
                }
                switch (Media.InventorySource)
                {
                    case null:
                        inventorySource = InstaMediaInventorySource.None;
                        break;

                    case "media_or_ad":
                        inventorySource = InstaMediaInventorySource.MediaOrAdd;
                        break;

                    case "follow_hashtag_story":
                        inventorySource = InstaMediaInventorySource.FollowHashtagStory;
                        break;

                    case "coauthored_post_unconnected":
                        inventorySource = InstaMediaInventorySource.CoauthoredPostUnconnected;
                        break;

                    default:
                        break;
                }
                var exploreSourceToken = Media.ExploreSourceToken;

                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    if (!liked)
                    {
                        result = await Api.MediaProcessor.LikeMediaAsync(Media.InstaIdentifier,
                                 Media,
                                 isCarouselBumpedPost: Media.MediaType == InstaMediaType.Carousel,
                                 exploreSourceToken: exploreSourceToken,
                                 containerModule: containerModule,
                                 carouselIndex: carouselIndex,
                                 feedPosition: feedPosition);
                    }
                    else
                    {
                        result = await Api.MediaProcessor.UnLikeMediaAsync(Media.InstaIdentifier,
                                 Media,
                                 isCarouselBumpedPost: Media.MediaType == InstaMediaType.Carousel,
                                 exploreSourceToken: exploreSourceToken,
                                 containerModule: containerModule,
                                 carouselIndex: carouselIndex,
                                 feedPosition: feedPosition);
                    }
                }
            }
            finally
            {
                if (result != null && !result.Succeeded)
                {
                    Media.HasLiked = liked;
                    Media.LikesCount = likesCount;
                }
            }
        }

        public void MoreButton(RichTextBlock txtCaption)
        {
            txtCaption.MaxLines = 0;
        }

        void NavigateToComments()
        {
            NavigationService.Navigate(typeof(MediaCommentsView),
                              new MediaCommentsViewParameter(Media));
        }

        void NavigateToMediaLikers()
        {
            NavigationService.Navigate(typeof(MediaLikersView), Media.InstaIdentifier);
        }

        void NavigateToHashtag(string hashtagName)
        {
            NavigationService.Navigate(typeof(HashtagProfileView), hashtagName);
        }

        void NavigateToUser(InstaUser user)
        {
            NavigationService.Navigate(typeof(UserProfileView), user);
        }

        void NavigateToLocation()
        {
            NavigationService.Navigate(typeof(PlaceProfileView), Media.Location);
        }

        async Task SaveMediaAsync()
        {
            IResult<bool> result = null;
            var isSaved = Media.HasViewerSaved;
            Media.HasViewerSaved = !Media.HasViewerSaved;
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    if (isSaved)
                    {
                        result = await Api.MediaProcessor.UnSaveMediaAsync(Media.InstaIdentifier);
                    }
                    else
                    {
                        result = await Api.MediaProcessor.SaveMediaAsync(Media.InstaIdentifier);
                    }
                }
            }
            finally
            {
                if (result == null || !result.Succeeded)
                {
                    Media.HasViewerSaved = isSaved;
                }
            }
        }

        async Task ShareMediaAsync()
        {
            var threads = await UserSelectionDialog.SelectDirectThreads();
            if (!threads.Any()) return;
            using var Api = App.Container.GetService<IInstaApi>();
            var result = await Api.MessagingProcessor.ShareMediaToThreadAsync(Media.Pk, Media.MediaType, string.Empty, threadIds: threads.Select(x => x.ThreadId).ToArray());
            if (!result.Succeeded)
                await MessageDialogHelper.ShowAsync(result.Info.Message);
        }

        async void UnloadLikeAnimation()
        {
            if (LoadLikeAnimation)
                await Task.Delay(2500);
            else await Task.Delay(1700);
            UIContext.Post(new SendOrPostCallback((a) =>
            {
                LoadLikeAnimation = false;
                LoadUnLikeAnimation = false;
            }), null);
        }

        DependencyObject FindContainerInFeedTimeline(HomeView home)
        {
            return home.FeedPostsList.ContainerFromItem(Media);
        }

        uint FindMediaIndexInFeedTimeline(HomeView home)
        {
            var container = FindContainerInFeedTimeline(home);
            var index = home.FeedPostsList.IndexFromContainer(container);
            if (index > 0) return (uint)index;
            else return 0;
        }

        int? FindCarouselIndexInFeedTimeline(HomeView home)
        {
            var container = FindContainerInFeedTimeline(home);
            if (Media.MediaType != InstaMediaType.Carousel) return null;

            if (container is ListViewItem lvi)
            {
                if (lvi.ContentTemplateRoot is InstaMediaPresenterUC presenter)
                {
                    var index = presenter.carouselPresenter.Gallery.SelectedIndex;
                    if (index >= 0) return index;
                    else return 0;
                }
            }
            return null;
        }
    }
}
