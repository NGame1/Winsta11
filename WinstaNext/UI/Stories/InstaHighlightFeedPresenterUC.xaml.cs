using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaHighlightFeedPresenterUC : UserControl
    {
        public static readonly DependencyProperty HighlightFeedProperty = DependencyProperty.Register(
          nameof(HighlightFeed),
          typeof(InstaHighlightFeed),
          typeof(InstaHighlightFeedPresenterUC),
          new PropertyMetadata(null));

        public InstaHighlightFeed HighlightFeed
        {
            get { return (InstaHighlightFeed)GetValue(HighlightFeedProperty); }
            set { SetValue(HighlightFeedProperty, value); }
        }

        public RelayCommand<object> NavigateToUserProfileCommand { get; set; }

        public InstaHighlightFeedPresenterUC()
        {
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            this.InitializeComponent();
        }

        void NavigateToUserProfile(object obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }
    }
}
