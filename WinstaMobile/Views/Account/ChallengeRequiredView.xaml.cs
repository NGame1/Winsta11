using WinstaCore.Helpers;
using WinstaCore.Interfaces.Views.Accounts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChallengeRequiredView : BasePage, IChallengeRequiredView
    {
        public override string PageHeader { get; protected set; }

        public ChallengeRequiredView()
        {
            this.InitializeComponent();
        }

        private void ChallengeWebView_Loaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            var currentDevice = ViewModel.Api.GetCurrentDevice();
            var currentUserAgent = ViewModel.Api.GetUserAgent();
            var browswerUserAgent = $"Mozilla/5.0 (Linux; Android {currentDevice.AndroidVer.APILevel}; {currentDevice.AndroidBoardName} {currentDevice.DeviceModel} Build/{currentDevice.HardwareModel}; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.120 Mobile Safari/537.36";

            var userAgent = $"{browswerUserAgent} {currentUserAgent}";
            WebViewUserAgentHelper.SetUserAgent(userAgent);
        }
    }
}
