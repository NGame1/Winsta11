using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;
using WinstaCore.Interfaces.Views.Accounts;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TwoFactorAuthView : BasePage, ITwoFactorAuthView
    {
        public override string PageHeader { get; protected set; }

        public TwoFactorAuthView()
        {
            this.InitializeComponent();
            this.Loaded += TwoFactorAuthView_Loaded;
        }

        private void TwoFactorAuthView_Loaded(object sender, RoutedEventArgs e)
        {
            txtVerificationCode.Focus(FocusState.Keyboard);
            InputPane.GetForCurrentView().TryShow();
        }

        private void txtVerificationCode_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key != VirtualKey.Enter) return;
        }
    }
}
