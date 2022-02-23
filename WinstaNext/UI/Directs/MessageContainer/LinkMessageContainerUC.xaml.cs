using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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
            var sp = DirectItem.Text.Split(DirectItem.LinkMedia.LinkContext.LinkUrl);
            txtHere.Inlines.Add(new Run() {Text = sp.FirstOrDefault() });
            var hyper = new Hyperlink();
            hyper.NavigateUri = new Uri(DirectItem.LinkMedia.LinkContext.LinkUrl, UriKind.RelativeOrAbsolute);
            hyper.Inlines.Add(new Run() { Text = DirectItem.LinkMedia.LinkContext.LinkUrl });
            txtHere.Inlines.Add(hyper);
            txtHere.Inlines.Add(new Run() { Text = sp.LastOrDefault() });
        }
    }
}
