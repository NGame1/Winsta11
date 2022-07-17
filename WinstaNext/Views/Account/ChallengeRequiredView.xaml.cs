using Windows.UI.Xaml.Controls;
using WinstaCore.Interfaces.Views.Accounts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChallengeRequiredView : Page, IChallengeRequiredView
    {
        public ChallengeRequiredView()
        {
            this.InitializeComponent();
        }
    }
}
