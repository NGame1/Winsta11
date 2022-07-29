using Resources;
using WinstaCore.Interfaces.Views.Directs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Directs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DirectsListView : BasePage, IDirectsListView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Directs;

        public DirectsListView()
        {
            this.InitializeComponent();
        }

    }
}
