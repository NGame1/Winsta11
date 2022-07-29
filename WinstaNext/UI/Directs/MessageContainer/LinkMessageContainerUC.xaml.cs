using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs.MessageContainer
{
    public sealed partial class LinkMessageContainerUC : MessageContainerUC
    {
        public LinkMessageContainerUC()
        {
            this.InitializeComponent();
            this.DataContextChanged += LinkMessageContainerUC_DataContextChanged;
        }

        private void LinkMessageContainerUC_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (args.NewValue == null) return;
            this.DataContextChanged -= LinkMessageContainerUC_DataContextChanged;
#if WINSTA11
            var sp = DirectItem.Text.Split(DirectItem.LinkMedia.LinkContext.LinkUrl);
#else
            string[] strings = new[] { DirectItem.LinkMedia.LinkContext.LinkUrl };
            var sp = DirectItem.Text.Split(strings, StringSplitOptions.None);
#endif
            txtHere.Inlines.Add(new Run() {Text = sp.FirstOrDefault() });
            var hyper = new Hyperlink();
            hyper.NavigateUri = new Uri(DirectItem.LinkMedia.LinkContext.LinkUrl, UriKind.RelativeOrAbsolute);
            hyper.Inlines.Add(new Run() { Text = DirectItem.LinkMedia.LinkContext.LinkUrl });
            txtHere.Inlines.Add(hyper);
            txtHere.Inlines.Add(new Run() { Text = sp.LastOrDefault() });
        }
    }
}
