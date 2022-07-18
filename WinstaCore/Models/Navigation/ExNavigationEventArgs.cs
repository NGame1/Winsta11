using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace WinstaCore.Models.Navigation
{
    public class ExNavigationEventArgs
    {
        public Control Content { get; set; }
        public NavigationMode NavigationMode { get; set; }
        public object Parameter { get; set; }
        public Type SourcePageType { get; set; }

    }
}
