using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace WinstaNext.ViewModels.Users
{
    public class UserProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public UserProfileViewModel() : base()
        {

        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            base.OnNavigatedTo(e);
        }
    }
}
