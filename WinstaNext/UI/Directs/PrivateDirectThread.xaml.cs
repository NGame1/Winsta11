using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs
{
    public sealed partial class PrivateDirectThread : UserControl
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
       nameof(DirectThread),
       typeof(InstaDirectInboxThread),
       typeof(PrivateDirectThread),
       new PropertyMetadata(null));

        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set { SetValue(DirectThreadProperty, value); }
        }

        public PrivateDirectThread()
        {
            this.InitializeComponent();
        }

    }
}
