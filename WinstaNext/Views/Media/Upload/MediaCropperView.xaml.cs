using Windows.Storage;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views.Medias.Upload;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media.Upload
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MediaCropperView : Page, IMediaCropperView
    {
        public MediaCropperView()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not StorageFile file)
            {
                Frame.GoBack();
                return;
            }
            ViewModel.BeginLoading(file, slider);
            base.OnNavigatedTo(e);
        }

        void CommandBar_Closing(object sender, object e)
        {
            if (sender is CommandBar commandBar)
                commandBar.IsOpen = true;
        }
    }
}
