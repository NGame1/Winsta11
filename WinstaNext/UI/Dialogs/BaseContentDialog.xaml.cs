using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ViewModels.Dialogs;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Dialogs
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public partial class BaseContentDialog : ContentDialog
    {
        public BaseContentDialog()
        {
            this.InitializeComponent();
        }

        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if(DataContext is BaseDialogViewModel vm)
            {
                vm.HideDialogAction = Hide;
            }
        }
    }
}
