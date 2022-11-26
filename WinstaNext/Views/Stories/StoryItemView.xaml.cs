using Abstractions.Stories;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Helpers;
using WinstaCore.Services;
using WinstaNext.Helpers;
using WinstaNext.UI.Stories;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class StoryItemView : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty StoryRootProperty = DependencyProperty.Register(
          nameof(StoryRoot),
          typeof(WinstaStoryItem),
          typeof(StoryItemView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(RegisterStoryRootPropertyChanged))]
        public WinstaStoryItem StoryRoot
        {
            get { return (WinstaStoryItem)GetValue(StoryRootProperty); }
            set { SetValue(StoryRootProperty, value); }
        }

        WinstaReelFeed? ReelFeed
        {
            get => StoryRoot?.ReelFeed;
        }

        InstaHighlightFeed? Highlight
        {
            get => StoryRoot?.HighlightStory;
        }

        public ObservableCollection<InstaStoryItem>? StoryItems
        {
            get => ReelFeed != null ? ReelFeed.Items : Highlight?.Items;
        }

        public double PageHeight { get; set; }

        public double PageWidth { get; set; }

        public double StoryDuration { get; set; } = 0;
        public double ElapsedTime { get; set; }

        internal Progress<double> StoryItemProgress { get; private set; } = new();

        public event EventHandler<WinstaReelFeed?>? ItemsEnded;
        public event PropertyChangedEventHandler? PropertyChanged;

        public StoryItemView()
        {
            this.InitializeComponent();
            StoryItemProgress.ProgressChanged += OnStoryItemProgressChanged;
        }

        void FlipView_Loaded(object sender, RoutedEventArgs e) => SetFlipViewSize();
        void FlipView_SizeChanged(object sender, SizeChangedEventArgs e) => SetFlipViewSize();

        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFlipViewSize();
            HandlePlayback(e);
        }

        /// <summary>
        /// FlipView SelectionChanged
        /// </summary>
        /// <param name="e"></param>
        void HandlePlayback(SelectionChangedEventArgs e)
        {
            if (StoryRoot == null || !StoryRoot.IsSelected) return;
            //stop the removed item
            //Play the added item
            if (e.AddedItems.Any())
            {
                if (e.AddedItems.FirstOrDefault() is not InstaStoryItem addeditem) return;
                PlaybackController(addeditem, true);
            }
            if (e.RemovedItems.Any())
            {
                if (e.RemovedItems.FirstOrDefault() is not InstaStoryItem removeditem) return;
                PlaybackController(removeditem, false);
            }
        }

        /// <summary>
        /// Carousel IsSelected changed
        /// </summary>
        void HandlePlayback(bool val)
        {
            if (StoryRoot == null) return;
            //Play or stop the current slide
            if (FlipView.SelectedItem is not InstaStoryItem storyItem) return;
            PlaybackController(storyItem, val);
        }
        

        void PlaybackController(InstaStoryItem storyItem, bool val)
        {
            var container = GetContainer(storyItem);
            if (container == null) return;

            if (val) container.Play(StoryItemProgress);
            else container.Stop();
        }

        void OnStoryItemProgressChanged(object? sender, double progress)
        {
            if (progress == 100)
            {
                if (FlipView.Items.Count - 1 > FlipView.SelectedIndex)
                {
                    //Can go to the next slide
                    FlipView.SelectedIndex++;
                }
                else
                {
                    //Need to notify to carousel view move to the next carousel.
                    ItemsEnded?.Invoke(this, ReelFeed);
                }
            }
        }

        void SetFlipViewSize()
        {
            var nav = App.Container.GetRequiredService<NavigationService>();
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

        bool _isregistered = false;
        void RegisterStoryRootPropertyChanged()
        {
            if (StoryRoot == null) return;
            if (_isregistered) return;
            _isregistered = true;
            StoryRoot.PropertyChanged += StoryRoot_PropertyChanged;
        }

        private void StoryRoot_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != nameof(StoryRoot.IsSelected)) return;
            HandlePlayback(StoryRoot.IsSelected);
        }

        InstaStoryItemPresenterUC? GetContainer(InstaStoryItem storyItem)
        {
            if (storyItem == null) throw new ArgumentNullException(nameof(storyItem));
            if (FlipView.ContainerFromItem(storyItem) is FlipViewItem fvi &&
                fvi.ContentTemplateRoot is InstaStoryItemPresenterUC uc)
                return uc;
            return null;
        }
    }
}
