using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using PropertyChanged;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaCore;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Stories.StickersView
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class PollStickerUC : UserControl
    {
        public static readonly DependencyProperty PollProperty = DependencyProperty.Register(
                nameof(Poll),
                typeof(InstaStoryPollItem),
                typeof(PollStickerUC),
                new PropertyMetadata(null));

        public InstaStoryPollItem Poll
        {
            get { return (InstaStoryPollItem)GetValue(PollProperty); }
            set { SetValue(PollProperty, value); }
        }

        public float ScaleValue { get => Poll.Height / 0.163081378f; }
        public float QuestionFontSize { get => 16; }

        public PollStickerUC()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Poll == null) return;
            if (string.IsNullOrEmpty(Poll.PollSticker.Question))
                titleRow.Height = new(0);

            var parent = this.FindParent<StickersViewGrid>();
            var pollHeightPixels = parent.ActualHeight * Poll.Height;
            var scale = pollHeightPixels / parent.ActualHeight;
            TalliesSection.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = 1 + scale
            };
            QuestionSection.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = Poll.Height / scale
            };
            txtQuestion.FontSize = Math.Round(24 / (Poll.Height / scale));
            Thickness marg = new(0, Math.Round(14 * (Poll.Height / scale)), 0, Math.Round(14 * (Poll.Height / scale)));
            txtQuestion.Margin = marg;

        }

        private void Button_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Button, "VisualStateNormal", false);
        }

        private void Button_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            VisualStateManager.GoToState(sender as Button, "VisualStateNormal", false);
        }

        private async void VoteChoosen_Click(object sender, RoutedEventArgs e)
        {
            var btn = sender as Button;
            var btnText = btn.Content is TextBlock txt ? txt.Text : throw new Exception();
            if (btn.DataContext is not InstaStoryTalliesItem talliesItem) return;
            var parent = this.FindParent<InstaStoryItemPresenterUC>();
            using (var Api = AppCore.Container.GetService<IInstaApi>())
            {
                var vote = (InstaStoryPollVoteType)Enum.Parse(typeof(InstaStoryPollVoteType), btnText);
                await Api.StoryProcessor.VoteStoryPollAsync(parent.Story.Id, $"{Poll.PollSticker.PollId}", vote);
            }
        }
    }
}
