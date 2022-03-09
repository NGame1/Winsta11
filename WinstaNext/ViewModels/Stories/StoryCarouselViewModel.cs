using InstagramApiSharp.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Controls;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Core.Collections.IncrementalSources.Stories;
using WinstaNext.Core.Navigation;
using WinstaNext.Views.Stories;

namespace WinstaNext.ViewModels.Stories
{
    internal class StoryCarouselViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public IncrementalLoadingCollection<IncrementalFeedStories, WinstaStoryItem> Stories { get; private set; }

        public RelayCommand<Carousel> CarouselSelectionChangedCommand { get; set; }

        [OnChangedMethod(nameof(OnSelectedItemChanged))]
        public WinstaStoryItem SelectedItem { get; set; }

        public double PageHeight { get; set; }
        public double PageWidth { get; set; }

        public StoryCarouselViewModel()
        {
            CarouselSelectionChangedCommand = new(CarouselSelectionChanged);
        }

        void StopPreviousItem(Carousel carousel)
        {
            if (previousindex == -1) return;

            var previousContainer = (CarouselItem)carousel.ContainerFromIndex(previousindex);
            if (previousContainer == null) return;
            if (previousContainer.ContentTemplateRoot is StoryItemView previousStoryItemView)
            {
                previousStoryItemView.Play = false;
            }
        }

        int previousindex = -1;
        void CarouselSelectionChanged(Carousel carousel)
        {
            if (carousel == null || carousel.SelectedItem == null) return;
            var container = (CarouselItem)carousel.ContainerFromItem(carousel.SelectedItem);
            if (container == null) return;

            StopPreviousItem(carousel);

            previousindex = carousel.SelectedIndex;
            if (container.ContentTemplateRoot is StoryItemView storyItemView)
            {
                storyItemView.Play = true;
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
            Stories = para.Stories;
            SelectedItem = para.TargetItem;
            for (int i = 0; i < Stories.Count; i++)
            {
                var StoryItem = Stories.ElementAt(i).ReelFeed;
                if (StoryItem != null)
                {
                    for (int j = 0; j < StoryItem.Items.Count; j++)
                    {
                        var sti = StoryItem.Items.ElementAt(j);
                        sti.User = StoryItem.User;
                    }
                }
            }

            base.OnNavigatedTo(e);
        }

        public override void OnNavigatedFrom(NavigationEventArgs e)
        {
            (NavigationService.Content as FrameworkElement).SizeChanged -= StoryCarouselViewModel_SizeChanged;
            
            base.OnNavigatedFrom(e);
        }

        private void StoryCarouselViewModel_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PageHeight = e.NewSize.Height;
            PageWidth = e.NewSize.Width;
        }

        void OnSelectedItemChanged()
        {
            var storyindex = Stories.IndexOf(SelectedItem);
            for (int i = storyindex; i < storyindex + 4; i++)
            {
                if (i < Stories.Count)
                {
                    var story = Stories.ElementAt(i);
                    LoadStory(story);
                }
            }
        }

        Dictionary<string, bool> loadingStories = new Dictionary<string, bool>();
        async void LoadStory(WinstaStoryItem story)
        {
            if (story.ReelFeed == null) return;
            var StoryItem = story.ReelFeed;
            string pk = StoryItem.User != null ? StoryItem.User.Pk.ToString() : StoryItem.Owner.Pk;
            try
            {
                if (loadingStories.TryGetValue(pk, out var isloading))
                {
                    if (isloading) return;
                }
                loadingStories[pk] = true;
                if (!StoryItem.Items.Any())
                {
                    using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                    {
                        //var result = await Api.StoryProcessor.GetUsersStoriesAsHighlightsAsync(pk);
                        var result = await Api.StoryProcessor.GetUserStoryFeedAsync(StoryItem.User.Pk);
                        if (result.Succeeded)
                        {
                            foreach (var item in result.Value.Items)
                            {
                                StoryItem.Items.Add(item);
                                //foreach (var storyitem in item.Items)
                                //{
                                //    StoryItem.Items.Add(storyitem);
                                //}
                            }
                        }
                    }
                }
            }
            finally { loadingStories.Remove(pk); }
        }

    }
}
