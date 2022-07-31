using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Search
{
    public sealed partial class PlaceSearchUC : UserControl
    {
        public static readonly DependencyProperty PlaceProperty = DependencyProperty.Register(
             nameof(Place),
             typeof(InstaPlace),
             typeof(PlaceSearchUC),
             new PropertyMetadata(null));

        public InstaPlace Place
        {
            get { return (InstaPlace)GetValue(PlaceProperty); }
            set { SetValue(PlaceProperty, value); }
        }

        public PlaceSearchUC()
        {
            this.InitializeComponent();
        }
    }
}
