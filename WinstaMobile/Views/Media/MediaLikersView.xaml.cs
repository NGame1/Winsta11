using Resources;
using WinstaCore.Interfaces.Views.Medias;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaLikersView : BasePage, IMediaLikersView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.MediaLikers;

        public MediaLikersView()
        {
            this.InitializeComponent();
        }
    }
}
