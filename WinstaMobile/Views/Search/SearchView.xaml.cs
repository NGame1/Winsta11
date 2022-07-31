using Resources;
using WinstaCore.Interfaces.Views.Search;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Search
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchView : BasePage, ISearchView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.General.Search;

        public SearchView()
        {
            this.InitializeComponent();
        }

    }
}
