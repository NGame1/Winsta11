﻿using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections;
using WinstaNext.Core.Collections.IncrementalSources.Hashtags;
using WinstaNext.Core.Collections.IncrementalSources.Users;
using WinstaNext.Core.Navigation;
using WinstaNext.Models.Core;
using WinstaNext.Services;
using WinstaNext.Views;
using WinstaNext.Views.Media;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Users
{
    public class HashtagProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        public string FollowBtnContent { get; set; }

        IncrementalHashtagTopMedias TopMediasInstance { get; set; }
        IncrementalHashtagRecentMedias RecentInstance { get; set; }

        IncrementalLoadingCollection<IncrementalHashtagTopMedias, InstaMedia> TopMedias { get; set; }
        IncrementalLoadingCollection<IncrementalHashtagRecentMedias, InstaMedia> RecentMedias { get; set; }

        public ISupportIncrementalLoading ItemsSource { get; set; }

        public InstaHashtag Hashtag { get; set; }

        public ScrollViewer ListViewScroll { get; set; }

        public RelayCommand<ItemClickEventArgs> NavigateToMediaCommand { get; set; }

        public AsyncRelayCommand FollowButtonCommand { get; set; }

        public ExtendedObservableCollection<MenuItemModel> ProfileTabs { get; set; } = new();

        [OnChangedMethod(nameof(ONSelectedTabChanged))]
        public MenuItemModel SelectedTab { get; set; }

        public HashtagProfileViewModel() : base()
        {
            NavigateToMediaCommand = new(NavigateToMedia);
            FollowButtonCommand = new(FollowButtonFuncAsync);
        }

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

            IResult<bool> result;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                if (follow)
                    result = await Api.HashtagProcessor.FollowHashtagAsync(Hashtag.Name);
                else result = await Api.HashtagProcessor.UnFollowHashtagAsync(Hashtag.Name);
            }
            if (!result.Succeeded) throw result.Info.Exception;
            SetFollowButtonContent();
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            if (e.Parameter is string tagName && !string.IsNullOrEmpty(tagName))
            {
                if (Hashtag != null && Hashtag.Name.ToLower() == tagName.ToLower()) return;
                IInstaApi Api = App.Container.GetService<IInstaApi>();
                {
                    var result = await Api.HashtagProcessor.GetHashtagInfoAsync(tagName);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    var hasht = result.Value.Adapt<InstaHashtag>();
                    Hashtag = hasht;
                }
            }
            else if (e.Parameter is InstaHashtag hashtag)
            {
                Hashtag = hashtag;
            }
            else
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }
            SetFollowButtonContent();
            RecentInstance = new(Hashtag.Name);
            TopMediasInstance = new(Hashtag.Name);
            RecentMedias = new(RecentInstance);
            TopMedias = new(TopMediasInstance);
            CreateProfileTabs();
            await base.OnNavigatedToAsync(e);
        }

        void ONSelectedTabChanged()
        {
            if (SelectedTab == null) return;
            if (SelectedTab.Text == LanguageManager.Instance.Instagram.Top)
            {
                ItemsSource = TopMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Recent)
            {
                ItemsSource = RecentMedias;
            }
            else if (SelectedTab.Text == LanguageManager.Instance.Instagram.Reels)
            {
                //ItemsSource = TVMedias;
            }
        }

        void CreateProfileTabs()
        {
            if (ProfileTabs.Any()) ProfileTabs.Clear();

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Recent, "\uE823"));

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Top, "\uE752"));

            ProfileTabs.Add(new(LanguageManager.Instance.Instagram.Reels, "\uE102"));

            if (SelectedTab != null)
                SelectedTab = null;

            SelectedTab = ProfileTabs.FirstOrDefault();
        }

        void SetFollowButtonContent()
        {
            if (Hashtag.Following)
                FollowBtnContent = LanguageManager.Instance.Instagram.Unfollow;
            else FollowBtnContent = LanguageManager.Instance.Instagram.Follow;
        }

        private void UserProfileViewModel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewHeight = e.NewSize.Height;
            ViewWidth = e.NewSize.Width;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            (NavigationService.Content as HashtagProfileView).SizeChanged += UserProfileViewModel_SizeChanged;
            base.OnNavigatedTo(e);
        }

        public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            (NavigationService.Content as HashtagProfileView).SizeChanged -= UserProfileViewModel_SizeChanged;
            base.OnNavigatingFrom(e);
        }
    }
}