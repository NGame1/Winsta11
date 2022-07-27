using Resources;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Comments
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentLikersView : BasePage
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.CommentLikers;

        public CommentLikersView()
        {
            this.InitializeComponent();
        }
    }
}
