using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Stories
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
            var UserProfileView = App.Container.GetService<IUserProfileView>();
            NavigationService.Navigate(UserProfileView, obj);
        }
    }
}
