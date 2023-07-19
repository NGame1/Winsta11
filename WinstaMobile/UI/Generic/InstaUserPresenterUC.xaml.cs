using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Generic
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaUserPresenterUC : UserControl
    {
        public static readonly DependencyProperty UserShortProperty = DependencyProperty.Register(
          nameof(UserShort),
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
          nameof(User),
          typeof(InstaUser),
          typeof(InstaUserPresenterUC),
          new PropertyMetadata(null));

        public InstaUser User
        {
            get { return (InstaUser)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public event EventHandler<RoutedEventArgs> RemoveButtonClick;

        void OnUserShortChanged()
        {
            User = new(UserShort);
        }

        public InstaUserPresenterUC()
        {
            this.InitializeComponent();
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            RemoveButtonClick?.Invoke(sender, e);
        }
    }
}
