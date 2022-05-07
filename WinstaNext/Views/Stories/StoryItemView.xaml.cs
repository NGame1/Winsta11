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

        public InstaReelFeed StoryItem
        {
            get { return (InstaReelFeed)GetValue(StoryItemProperty); }
            set { SetValue(StoryItemProperty, value); }
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
        void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e) => SetFlipViewSize();

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
    }
}
