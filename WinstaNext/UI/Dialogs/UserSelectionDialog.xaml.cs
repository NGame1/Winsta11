using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Dialogs
{
    public sealed partial class UserSelectionDialog : ContentDialog
    {
        private UserSelectionDialog()
        {
            this.InitializeComponent();
        }

        private void Select_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        private void Cancel_Click(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {

        }

        public static async Task SelectUsers()
        {
            var dialog = new UserSelectionDialog();
            var result = await dialog.ShowAsync(ContentDialogPlacement.InPlace);
            
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.HideDialogAction = Hide;
        }
    }
}
