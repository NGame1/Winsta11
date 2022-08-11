using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

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
    }
}
