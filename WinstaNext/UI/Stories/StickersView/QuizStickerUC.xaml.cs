using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaCore;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories.StickersView
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class QuizStickerUC : UserControl
    {
        public static readonly DependencyProperty QuizProperty = DependencyProperty.Register(
                nameof(Quiz),
                typeof(InstaStoryQuizItem),
                typeof(QuizStickerUC),
                new PropertyMetadata(null));

        public InstaStoryQuizItem Quiz
        {
            get { return (InstaStoryQuizItem)GetValue(QuizProperty); }
            set { SetValue(QuizProperty, value); }
        }

        public float ScaleValue { get => Quiz.Height / 0.163081378f; }
        public float QuestionFontSize { get => 16; }

        public QuizStickerUC()
        {
            this.InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (Quiz == null) return;
            if (string.IsNullOrEmpty(Quiz.QuizSticker.Question))
                titleRow.Height = new(0);

            var parent = this.FindParent<StickersViewGrid>();
            var pollHeightPixels = parent.ActualHeight * Quiz.Height;
            var scale = pollHeightPixels / parent.ActualHeight;
            TalliesSection.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = 1 + scale
            };
            QuestionSection.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = Quiz.Height / scale
            };
            txtQuestion.FontSize = Math.Round(24 / (Quiz.Height / scale));
            Thickness marg = new(0, 
                Math.Round(14 * (Quiz.Height / scale)),
                0, 
                Math.Round(14 * (Quiz.Height / scale)));
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
                var Index = Quiz.QuizSticker.Tallies.FindIndex(x => x.Text == btnText);
                await Api.StoryProcessor.AnswerToStoryQuizAsync(parent.Story.Pk, Quiz.QuizSticker.QuizId, Index);
            }
        }
    }
}
