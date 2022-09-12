using InstagramApiSharp.Helpers;
using System;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using WinstaCore.Interfaces.Views.Accounts;
using WinstaCore.Helpers;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using System.Xml.Linq;
using System.Text;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : BasePage, ILoginView
    {
        public override string PageHeader { get; protected set; }

        InputPane InputPane { get; }
        public LoginView()
        {
            this.InitializeComponent();
            InputPane = InputPane.GetForCurrentView();
        }

        private void BasePage_Loaded(object sender, RoutedEventArgs e)
        {
            txtUserIdentifier.Focus(FocusState.Keyboard);
            InputPane.TryShow();
        }

        private void txtUserIdentifier_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            txtPassword.Focus(FocusState.Keyboard);
            InputPane.TryShow();
        }

        private void txtPassword_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
            InputPane.TryHide();
            ViewModel.LoginCommand.Execute(null);
        }

        private void webV_NavigationCompleted(Windows.UI.Xaml.Controls.WebView sender, Windows.UI.Xaml.Controls.WebViewNavigationCompletedEventArgs args)
        {
            if (InstaFbHelper.IsLoggedInFromUrl(args.Uri.ToString()))
                ViewModel.CompleteLoginWithFacebook(args.Uri.ToString(), InstaFbHelper.FacebookAddress);
        }

    }
}
