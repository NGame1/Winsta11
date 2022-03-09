using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Helpers;
using WinstaNext.Services;
using WinstaNext.UI.Stories;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class StoryItemView : UserControl
    {
        public static readonly DependencyProperty StoryItemProperty = DependencyProperty.Register(
          "StoryItem",
          typeof(InstaReelFeed),
          typeof(StoryItemView),
          new PropertyMetadata(null));

        //[OnChangedMethod(nameof(OnStoryItemChanged))]
        public InstaReelFeed StoryItem
        {
            get { return (InstaReelFeed)GetValue(StoryItemProperty); }
            set { SetValue(StoryItemProperty, value); }
        }

        public double PageHeight { get; set; }

        public double PageWidth { get; set; }

        [OnChangedMethod(nameof(OnPlayChanged))]
        public bool Play { get; set; } = false;

        public double StoryDuration { get; set; } = 0;
        public double ElapsedTime { get; set; }

        public event EventHandler ItemsEnded;

        public StoryItemView()
        {
            this.InitializeComponent();
        }

        InstaStoryItemPresenterUC GetStoryPresenter(int index)
        {
            var fic = FlipView.ContainerFromIndex(index);
            if (fic is FlipViewItem fi)
            {
                if (fi.ContentTemplateRoot is InstaStoryItemPresenterUC presenter)
                    return presenter;
            }
            return null;
        }

        int previousIndex = -1;
        void StopAll()
        {
            if (previousIndex == -1) return;
            var presenter = GetStoryPresenter(previousIndex);
            if (presenter == null) return;
            if (presenter.LoadMediaElement)
                presenter.videoplayer.Stop();
            else presenter.StopTimer();
        }

        void PlayCarouselItem()
        {
            var itemPresenter = GetStoryPresenter(previousIndex);
            if (itemPresenter == null) return;
            if (itemPresenter.LoadMediaElement)
            {
                //itemPresenter.videoplayer.SetPlaybackSource(MediaSource.CreateFromUri(new Uri(itemPresenter.Story.Videos[0].Uri)));
                if (itemPresenter.videoplayer.Source == null)
                    itemPresenter.videoplayer.Source = new Uri(itemPresenter.Story.Videos[0].Uri);
                itemPresenter.videoplayer.Play();
            }
            else
            {
                itemPresenter.StartTimer();
            }
        }

        void OnPlayChanged()
        {
            if (!Play) StopAll();
            else
            {
                PlayCarouselItem();

                if (FlipView.SelectedItem is InstaStoryItem currentStoryItem)
                {
                    if (currentStoryItem.TakenAtUnix > StoryItem.Seen)
                    {
                        StoryItem.Seen = currentStoryItem.TakenAtUnix;
                        MarkStoryAsSeen(currentStoryItem.Id, currentStoryItem.TakenAtUnix);
                    }
                }
            }
        }

        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFlipViewSize();
            StopAll();

            previousIndex = FlipView.SelectedIndex;

            if (!Play) return;

            if (FlipView.SelectedItem is InstaStoryItem currentStoryItem)
            {
                if (currentStoryItem.TakenAtUnix > StoryItem.Seen)
                {
                    StoryItem.Seen = currentStoryItem.TakenAtUnix;
                    MarkStoryAsSeen(currentStoryItem.Id, currentStoryItem.TakenAtUnix);
                }
            }

            PlayCarouselItem();
        }

        private void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            SetFlipViewSize();
            var lastseen = StoryItem.Seen;
            var seenitem = StoryItem.Items.FirstOrDefault(x => x.TakenAtUnix == lastseen);
            var index = StoryItem.Items.IndexOf(seenitem);
            if (index > 0 && index != StoryItem.Items.Count - 1)
            {
                FlipView.SelectedIndex = index + 1;
            }
        }

        private void FlipView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetFlipViewSize();
        }

        void SetFlipViewSize()
        {
            var nav = App.Container.GetService<NavigationService>();
            {
                var parentFrame = (FrameworkElement)nav.Content;
                if (FlipView.SelectedItem is not InstaStoryItem story) return;
                var size = ControlSizeHelper.CalculateSizeInBox(story.Images[0].Width, story.Images[0].Height, parentFrame.ActualHeight, parentFrame.ActualWidth);
                PageHeight = size.Height;
                PageWidth = size.Width;
                FlipView.Height = PageHeight;
                FlipView.Width = PageWidth;
                var container = FlipView.ContainerFromItem(FlipView.SelectedItem);
                if (container is FlipViewItem fvi)
                {
                    fvi.Height = PageHeight;
                    fvi.Width = PageWidth;
                }
            }
        }

        async void MarkStoryAsSeen(string storyItemId, long takenAtUnix)
        {
            using (var Api = App.Container.GetService<IInstaApi>())
            {
                await Api.StoryProcessor.MarkStoryAsSeenAsync(storyItemId, takenAtUnix);
            }
        }

        private void InstaStoryItemPresenterUC_TimerEnded(object sender, bool e)
        {
            if (sender is not InstaStoryItemPresenterUC presenter) return;
            presenter.StopTimer();
            if (FlipView.SelectedIndex < FlipView.Items.Count - 1)
            {
                FlipView.SelectedIndex++;
            }
            else ItemsEnded?.Invoke(this, EventArgs.Empty);
        }
    }
}
