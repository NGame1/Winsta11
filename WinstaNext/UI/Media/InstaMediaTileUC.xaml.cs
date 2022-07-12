using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media
{
    public sealed partial class InstaMediaTileUC : UserControl
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
                nameof(Media),
                typeof(InstaMedia),
                typeof(InstaMediaTileUC),
                new PropertyMetadata(null));

        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        public InstaMediaTileUC()
        {
            this.InitializeComponent();
        }
    }
}
