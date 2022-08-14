using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using WinstaCore;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories.StickersView
{
    public sealed partial class SliderStickerUC : UserControl
    {
        public static readonly DependencyProperty SliderProperty = DependencyProperty.Register(
                nameof(Slider),
                typeof(InstaStorySliderItem),
                typeof(SliderStickerUC),
                new PropertyMetadata(null));

        public InstaStorySliderItem Slider
        {
            get { return (InstaStorySliderItem)GetValue(SliderProperty); }
            set { SetValue(SliderProperty, value); }
        }

        public SliderStickerUC()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Slider == null) return;
            if (string.IsNullOrEmpty(Slider.SliderSticker.Question))
                titleRow.Height = new(0);

            var parent = this.FindParent<StickersViewGrid>();
            var pollHeightPixels = parent.ActualHeight * Slider.Height;
            var pollWidthPixels = parent.ActualWidth * Slider.Width;
            var scale = pollHeightPixels / parent.ActualHeight;
            var scaleY = pollWidthPixels / parent.ActualWidth;
            //TalliesSection.RenderTransform = new ScaleTransform()
            //{
            //    //ScaleX = Poll.Height / 0.163081378f,
            //    ScaleY = 1 + scale
            //};
            this.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = Slider.Height / scale
            };
            slider.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = Slider.Height / scale
            };
            this.FontSize = Math.Round(24 / (Slider.Height / scale));

            Thickness marg = new(
                0,
                Math.Round(14 * (Slider.Height / scale)),
                0,
                Math.Round(14 * (Slider.Height / scale)));

            Thickness sliderMarg = new(
                Math.Round(14 * (Slider.Width / scaleY)),
                Math.Round(0 * (Slider.Height / scale)),
                Math.Round(14 * (Slider.Width / scaleY)),
                Math.Round(0 * (Slider.Height / scale)));
            slider.Margin = sliderMarg;
            //this.Margin = marg;

        }

        private async void slider_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(10);
            slider.Value = Slider.SliderSticker.SliderVoteAverage;
            var T4 = slider.FindDescendantOrSelf<TextBlock>();
            var thumb = slider.FindDescendantOrSelf<Thumb>();
            if (T4 != null)
            {
                T4.Text = Slider.SliderSticker.Emoji;
                thumb.PointerPressed += Thumb_PointerPressed;
                thumb.PointerReleased += Thumb_PointerReleased;
            }
        }

        private void Thumb_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var parent = this.FindParent<StickersViewGrid>();
            parent?.PauseTimerCommand.Execute(null);
        }

        private async void Thumb_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var parent = this.FindParent<InstaStoryItemPresenterUC>();
            this.FindParent<StickersViewGrid>()?.ResumeTimerCommand.Execute(null);
            try
            {
                using (var Api = AppCore.Container.GetService<IInstaApi>())
                {
                    var result = await Api.StoryProcessor.VoteStorySliderAsync(parent.Story.Id, Slider.SliderSticker.SliderId.ToString(), slider.Value);
                    Slider.SliderSticker.ViewerCanVote = false;
                    if (result.Succeeded)
                    {
                        //Change values of parent Story
                        result.Value.StorySliders.ForEach(x =>
                        {
                            if (x.SliderSticker.SliderId == Slider.SliderSticker.SliderId)
                            {
                                var NewValues = x.SliderSticker;
                                Slider.SliderSticker.ViewerVote = NewValues.ViewerVote;
                                Slider.SliderSticker.SliderVoteCount = NewValues.SliderVoteCount;
                                Slider.SliderSticker.ViewerCanVote = NewValues.ViewerCanVote;
                                Slider.SliderSticker.SliderVoteAverage = NewValues.SliderVoteAverage;
                            }
                        });
                    }
                }
            }
            catch
            {

            }
        }

    }
}
