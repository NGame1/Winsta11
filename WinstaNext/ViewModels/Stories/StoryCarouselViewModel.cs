using Abstractions.Stories;
using Core.Collections.IncrementalSources.Stories;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Navigation;

namespace WinstaNext.ViewModels.Stories
{
    internal class StoryCarouselViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public IncrementalLoadingCollection<IIncrementalSource<WinstaStoryItem>, WinstaStoryItem> Stories { get; private set; }

        [OnChangedMethod(nameof(OnSelectedItemChanged))]
        public WinstaStoryItem SelectedItem { get; set; }

        public double PageHeight { get; set; }
        public double PageWidth { get; set; }

        public StoryCarouselViewModel()
        {

        }

        public void NextStory(WinstaReelFeed feed)
        {
            //Event invoked from wrong story!
            if (SelectedItem == null || SelectedItem.ReelFeed != feed) return;
            NextStory();
        }

        void NextStory()
        {
            if (Stories.IndexOf(SelectedItem) is int Index && Index >= 0
                && Index != Stories.Count - 1)
            {
                SelectedItem = Stories.ElementAt(++Index);
            }
            else
            {
                //Last story item, Exit stories.
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                return;
            }
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not StoryCarouselViewParameter para)
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                return;
            }
            (NavigationService.Content as FrameworkElement).SizeChanged += StoryCarouselViewModel_SizeChanged;
            if (para.Stories == null)
            {
                var Instance = new IncrementalDummyStories();
                Stories = new(Instance);
                Stories.Add(para.TargetItem);
            }
            else Stories = para.Stories;
            SelectedItem = para.TargetItem;
            base.OnNavigatedTo(e);
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (NavigationService.Content as FrameworkElement).SizeChanged -= StoryCarouselViewModel_SizeChanged;
            SelectedItem = null;
            base.OnNavigatedFrom(e);
        }

        private void StoryCarouselViewModel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PageHeight = e.NewSize.Height;
            PageWidth = e.NewSize.Width;
        }

        void OnSelectedItemChanged()
        {
            if (SelectedItem == null) return;
            var selectedItems = Stories.Where(x => x.IsSelected);

            if (selectedItems.Any())
            {
                for (int i = 0; i < selectedItems.Count(); i++)
                {
                    var item = selectedItems.ElementAt(i);
                    SetIsSelected(item, false);
                }
            }

            SetIsSelected(SelectedItem, true);

            var Index = Stories.IndexOf(SelectedItem);
            LoadStoryIndexes(Index - 2, Index + 2);
        }

        void SetIsSelected(WinstaStoryItem storyItem, bool value)
        {
            if (storyItem is null) return;
            storyItem.IsSelected = value;
        }

        void LoadStoryIndexes(int first, int last)
        {
            for (int i = first; i <= last; i++)
            {
                if (i < 0 || i >= Stories.Count) continue;
                LoadStory(i);
            }
        }

        void LoadStory(int Index)
        {
            var story = Stories.ElementAt(Index);
            if (story.ReelFeed != null) LoadReelFeed(story.ReelFeed);
            if (story.HighlightStory != null) LoadHighlightStory(story.HighlightStory);
        }

        async void LoadReelFeed(WinstaReelFeed feed)
        {
            if (feed == null) throw new ArgumentNullException(nameof(feed));
            var me = App.Container?.GetService<InstaUserShort>();
            if (feed.User.Pk == me.Pk)
            {
                for (int i = 0; i < feed.Items.Count; i++)
                {
                    var u = feed.Items.ElementAt(i);
                    u.User = me;
                }
                feed.User.Pk = me.Pk;
                feed.User.UserName = me.UserName;
                feed.User.ProfilePicture = me.ProfilePicture;
            }
            if (feed.Items.Any()) return;
            if (feed.IsLoading) return;
            feed.IsLoading = true;
            try
            {
                string pk = feed.User != null ? feed.User.Pk.ToString() : feed.Owner.Pk;
                using (var Api = App.Container?.GetService<IInstaApi>())
                {
                    if (feed.User == null)
                    {
                        var res = await Api.StoryProcessor.GetUsersStoriesAsHighlightsAsync(pk);
                    }

                    var result = await Api.StoryProcessor.GetUserStoryFeedAsync(feed.User.Pk);
                    if (!result.Succeeded) return;
                    for (int i = 0; i < result.Value.Items.Count; i++)
                    {
                        feed.Items.Add(result.Value.Items.ElementAt(i));
                    }
                }
            }
            finally
            {
                feed.IsLoading = false;
            }
        }

        async void LoadHighlightStory(WinstaHighlightFeed feed)
        {
            if (feed == null) throw new ArgumentNullException(nameof(feed));
            if (feed.Items.Any() || feed.IsLoading) return;
            feed.IsLoading = true;
            try
            {
                using (var Api = App.Container?.GetService<IInstaApi>())
                {
                    var result = await Api.StoryProcessor.GetHighlightMediasAsync(feed.HighlightId);
                    if (!result.Succeeded) return;
                    for (int i = 0; i < result.Value.Items.Count; i++)
                    {
                        var highlightItem = result.Value.Items.ElementAt(i);
                        highlightItem.User.ProfilePicture = feed.CoverMedia.CroppedImage.Uri;
                        highlightItem.User.UserName = feed.Title;
                        highlightItem.User.CloseButton = true;
                        highlightItem.User.Pk = feed.User.Pk;
                        feed.Items.Add(highlightItem);
                    }
                }
            }
            finally
            {
                feed.IsLoading = false;
            }
        }
    }
}
