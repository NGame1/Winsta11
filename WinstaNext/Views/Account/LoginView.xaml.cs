using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Globalization;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Account
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoginView : BasePage
    {
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
    }
}
