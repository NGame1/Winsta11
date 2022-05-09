using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Helpers;
using WinstaNext.Services;
using WinstaNext.UI.Stories;
#nullable enable

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class StoryItemView : UserControl
    {
        public static readonly DependencyProperty StoryRootProperty = DependencyProperty.Register(
          "StoryRoot",
          typeof(WinstaStoryItem),
          typeof(StoryItemView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(RegisterStoryRootPropertyChanged))]
        public WinstaStoryItem StoryRoot
        {
            get { return (WinstaStoryItem)GetValue(StoryRootProperty); }
            set { SetValue(StoryRootProperty, value); }
        }

        public WinstaReelFeed StoryItem
        {
            get => StoryRoot.ReelFeed;
        }

        public double PageHeight { get; set; }

        public double PageWidth { get; set; }

        public double StoryDuration { get; set; } = 0;
        public double ElapsedTime { get; set; }

        public StoryItemView()
        {
            this.InitializeComponent();
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
                var container = GetContainer(addeditem);
                if (container == null) return;
                container.Play();
            }
            if (e.RemovedItems.Any())
            {
                if (e.RemovedItems.FirstOrDefault() is not InstaStoryItem removeditem) return;
                var container = GetContainer(removeditem);
                if (container == null) return;
                container.Stop();
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
            var container = GetContainer(storyItem);
            if (container == null) return;
            if (val) container.Play();
            else container.Stop();
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
