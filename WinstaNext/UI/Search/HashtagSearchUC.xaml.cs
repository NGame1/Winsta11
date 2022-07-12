using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Search
{
    public sealed partial class HashtagSearchUC : UserControl
    {
        public static readonly DependencyProperty HashtagProperty = DependencyProperty.Register(
          nameof(Hashtag),
          typeof(InstaHashtag),
          typeof(HashtagSearchUC),
          new PropertyMetadata(null));

        public InstaHashtag Hashtag
        {
            get { return (InstaHashtag)GetValue(HashtagProperty); }
            set { SetValue(HashtagProperty, value); }
        }

        public HashtagSearchUC()
        {
            this.InitializeComponent();
        }
    }
}
