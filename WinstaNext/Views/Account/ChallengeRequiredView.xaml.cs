using Microsoft.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls;
using WinstaCore.Interfaces.Views.Accounts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChallengeRequiredView : BasePage, IChallengeRequiredView
    {
        public ChallengeRequiredView()
        {
            this.InitializeComponent();
        }

        //private void WebView2_CoreWebView2Initialized(WebView2 sender, CoreWebView2InitializedEventArgs args)
        //{
        //    var currentDevice = ViewModel.Api.GetCurrentDevice();
        //    var currentUserAgent = ViewModel.Api.GetUserAgent();
        //    var browswerUserAgent = $"Mozilla/5.0 (Linux; Android {currentDevice.AndroidVer.APILevel}; {currentDevice.AndroidBoardName} {currentDevice.DeviceModel} Build/{currentDevice.HardwareModel}; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/91.0.4472.120 Mobile Safari/537.36";

        //    var userAgent = $"{browswerUserAgent} {currentUserAgent}";

        //    sender.CoreWebView2.Settings.IsSwipeNavigationEnabled = false;
        //    sender.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
        //    sender.CoreWebView2.Settings.IsScriptEnabled = true;
        //    sender.CoreWebView2.Settings.UserAgent = userAgent;
        //}
    }
}
