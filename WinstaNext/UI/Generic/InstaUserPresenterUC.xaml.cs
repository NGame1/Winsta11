using InstagramApiSharp.Classes.Models;
using PropertyChanged;
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
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Generic
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaUserPresenterUC : UserControl
    {
        public static readonly DependencyProperty UserShortProperty = DependencyProperty.Register(
          "UserShort",
          typeof(InstaUserShort),
          typeof(InstaUserPresenterUC),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnUserShortChanged))]
        public InstaUserShort UserShort
        {
            get { return (InstaUserShort)GetValue(UserShortProperty); }
            set { SetValue(UserShortProperty, value); }
        }

        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
          "User",
          typeof(InstaUser),
          typeof(InstaUserPresenterUC),
          new PropertyMetadata(null));

        public InstaUser User
        {
            get { return (InstaUser)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        void OnUserShortChanged()
        {
            User = new(UserShort);
        }

        public InstaUserPresenterUC()
        {
            this.InitializeComponent();
        }
    }
}
