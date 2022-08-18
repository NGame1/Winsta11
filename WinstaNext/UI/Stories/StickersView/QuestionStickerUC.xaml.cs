using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.UI;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using static System.Net.Mime.MediaTypeNames;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Stories.StickersView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionStickerUC : UserControl
    {
        public static readonly DependencyProperty QuestionProperty = DependencyProperty.Register(
                nameof(Question),
                typeof(InstaStoryQuestionItem),
                typeof(QuestionStickerUC),
                new PropertyMetadata(null));

        public InstaStoryQuestionItem Question
        {
            get { return (InstaStoryQuestionItem)GetValue(QuestionProperty); }
            set { SetValue(QuestionProperty, value); }
        }

        public QuestionStickerUC()
        {
            this.InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var parent = this.FindParent<StickersViewGrid>();
            parent?.PauseTimerCommand.Execute(null);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            var parent = this.FindParent<StickersViewGrid>();
            parent?.ResumeTimerCommand.Execute(null);
        }
        async void AnswerQuestionAsync(string StoryId, long QuestionId, string reply)
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.StoryProcessor.AnswerToStoryQuestionAsync(StoryId, QuestionId, reply);
                if (!result.Succeeded)
                {
                    throw result.Info.Exception;
                }
            }
        }

        private void TextBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            var state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Shift);
            if (!state.HasFlag(CoreVirtualKeyStates.Down))
            {
                if (sender is not TextBox txt) return;
                e.Handled = true;
                var parent = this.FindParent<InstaStoryItemPresenterUC>();
                AnswerQuestionAsync(parent.Story.Id, Question.QuestionSticker.QuestionId, txt.Text);
            }
        }
    }
}
