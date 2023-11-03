using InstagramApiSharp.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Printing3D;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Attributes;

namespace ViewModels.Media;

public class ExploreMediaLoadVM : BaseViewModel
{
    public ExploreMediaLoadVM()
    {

    }

    public override async Task OnNavigatedToAsync(NavigationEventArgs e)
    {
        if(e.NavigationMode == NavigationMode.Back)
        {
            NavigationService.GoBack();
            return;
        }
        if (e.NavigationMode == NavigationMode.Forward)
        {
            NavigationService.GoForward();
            return;
        }
        var Api = AppCore.Container.GetService<IInstaApi>();
        //Api.FeedProcessor.GetExploreChannelVideosAsync("", "", new RangePlayerAttribute(null),)
    }

}
