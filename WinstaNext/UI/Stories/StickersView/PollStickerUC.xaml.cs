using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System;
using System.Numerics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Stories.StickersView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
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

            TalliesSection.RenderTransform = new ScaleTransform()
            {
                //ScaleX = Poll.Height / 0.163081378f,
                ScaleY = Poll.Height / 0.163081378f
            };
            txtQuestion.FontSize = Math.Round(17 * Poll.Height / 0.163081378f);
            Thickness marg = new(0, Math.Round(10 * Poll.Height / 0.163081378f), 0, Math.Round(10 * Poll.Height / 0.163081378f));
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
    }
}
