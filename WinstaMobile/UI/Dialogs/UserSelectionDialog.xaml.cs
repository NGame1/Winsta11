using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaMobile.UI.Dialogs;

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

    public static async Task<IEnumerable<InstaDirectInboxThread>> SelectDirectThreads()
    {
        var dialog = new UserSelectionDialog();
        var result = await dialog.ShowAsync(ContentDialogPlacement.InPlace);
        if (result == ContentDialogResult.Primary)
            return dialog.lst.SelectedItems.Cast<InstaDirectInboxThread>();
        else return Enumerable.Empty<InstaDirectInboxThread>();
    }

    private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
    {
        ViewModel.HideDialogAction = Hide;
    }
}
