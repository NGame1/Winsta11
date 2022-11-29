using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System;
using System.Linq;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinstaCore.Converters.FileConverters;
using WinstaCore.Interfaces.Views.Directs;
using WinstaNext.ViewModels.Directs;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.Views.Directs
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class DirectThreadView : Page, IDirectThreadView
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
          nameof(DirectThread),
          typeof(InstaDirectInboxThread),
          typeof(DirectThreadView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnDirectItemChanged))]
        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set
            {
                SetValue(DirectThreadProperty, value);
            }
        }

        public static double MessageBubbleMaxWidth { get; private set; }

        public DirectThreadViewModel ViewModel { get; set; }

        public DirectThreadView()
        {
            this.InitializeComponent();
        }

        ~DirectThreadView()
        {
            ViewModel = null;
        }

        void OnDirectItemChanged()
        {
            if (DirectThread == null) return;
            ViewModel = new(DirectThread);
            ViewModel.ThreadId = DirectThread.ThreadId;
            lst.ItemsSource = ViewModel.ThreadItems;
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            MessageBubbleMaxWidth = e.NewSize.Width - 150;
        }

        private void SendMessageKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ViewModel.SendMessageCommand.Execute(null);
            args.Handled = true;
        }

        private async void txtMessage_Paste(object sender, TextControlPasteEventArgs e)
        {
            var clipboardContent = Clipboard.GetContent();
            if (clipboardContent.Contains(StandardDataFormats.Text))
            {
                return;
                //txtMessage.Text = await clipboardContent.GetTextAsync();
                //e.Handled = true;
            }
            else if (clipboardContent.Contains(StandardDataFormats.WebLink))
            {
                return;
                //var uri = await clipboardContent.GetWebLinkAsync();
                //txtMessage.Text = uri.ToString();
                //e.Handled = true;
            }
            else if (clipboardContent.Contains(StandardDataFormats.ApplicationLink))
            {
                return;
                //var uri = await clipboardContent.GetApplicationLinkAsync();
                //txtMessage.Text = uri.ToString();
                //e.Handled = true;
            }
            else if (clipboardContent.Contains(StandardDataFormats.Uri))
            {
                return;
                //var uri = await clipboardContent.GetUriAsync();
                //txtMessage.Text = uri.ToString();
                //e.Handled = true;
            }
            else if (clipboardContent.Contains(StandardDataFormats.Bitmap))
            {
                var stream = await clipboardContent.GetBitmapAsync();
                var imageBytes = await FileConverter.ToBytesAsync(await stream.OpenReadAsync());
                var sf = await ApplicationData.Current.TemporaryFolder.CreateFileAsync("tempfile.bmp", CreationCollisionOption.GenerateUniqueName);
                await FileIO.WriteBytesAsync(sf, imageBytes);
                //imageBytes = await ImageFileConverter.ConvertImageToJpegAsync(sf);
                //await FileIO.WriteBytesAsync(sf, imageBytes);
                await ViewModel.UploadImageAsync(sf);
                await sf.DeleteAsync(StorageDeleteOption.PermanentDelete);
                e.Handled = true;
            }
            else if (clipboardContent.Contains(StandardDataFormats.StorageItems))
            {
                var items = await clipboardContent.GetStorageItemsAsync();
                var file = items.FirstOrDefault();
                if (file == null) return;
                if (file is not StorageFile storageFile) return;
                if (storageFile.FileType == ".jpg" || storageFile.FileType == ".png" || storageFile.FileType == ".bmp")
                    await ViewModel.UploadImageAsync(storageFile);
                else if (storageFile.FileType == ".mp4")
                    await ViewModel.UploadVideoAsync(storageFile);
                e.Handled = true;
            }
        }
    }

}
