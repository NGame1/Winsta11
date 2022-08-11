using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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
    }
}
