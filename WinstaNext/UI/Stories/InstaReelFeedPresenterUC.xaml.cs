using Abstractions.Stories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Views.Profiles;
using WinstaCore.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    public sealed partial class InstaReelFeedPresenterUC : UserControl
    {
        public static readonly DependencyProperty ReelFeedProperty = DependencyProperty.Register(
             nameof(ReelFeed),
             typeof(WinstaReelFeed),
             typeof(InstaReelFeedPresenterUC),
             new PropertyMetadata(null));

        public WinstaReelFeed ReelFeed
        {
            get { return (WinstaReelFeed)GetValue(ReelFeedProperty); }
            set { SetValue(ReelFeedProperty, value); }
        }

        public RelayCommand<object> NavigateToUserProfileCommand { get; set; }

        public InstaReelFeedPresenterUC()
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
