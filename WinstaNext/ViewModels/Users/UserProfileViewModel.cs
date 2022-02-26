using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Users
{
    public class UserProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        public UserProfileViewModel() : base()
        {
        }

        public override Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            (NavigationService.Content as UserProfileView).SizeChanged += UserProfileViewModel_SizeChanged;
            return base.OnNavigatedToAsync(e);
        }

        private void UserProfileViewModel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewHeight = e.NewSize.Height;
            ViewWidth = e.NewSize.Width;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            base.OnNavigatedTo(e);
        }
    }
}
