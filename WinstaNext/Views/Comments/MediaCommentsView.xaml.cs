using Resources;
using Windows.UI.Xaml.Input;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Comments
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaCommentsView : BasePage
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.MediaComments;

        public string MediaId { get => ViewModel.Media.InstaIdentifier; }
        public MediaCommentsView()
        {
            this.InitializeComponent();
        }

        private async void SendButtonKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            await ViewModel.AddCommentCommand.ExecuteAsync(commentsListView);
            args.Handled = true;
        }
    }
}
