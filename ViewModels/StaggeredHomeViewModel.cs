using System;
using System.Threading.Tasks;

namespace ViewModels
{
    public class StaggeredHomeViewModel : HomeViewModel
    {
        public override async Task RefreshAsync()
        {
            await base.RefreshAsync();
            await Medias.LoadMoreItemsAsync(1);
            await Medias.LoadMoreItemsAsync(1);
            await Medias.LoadMoreItemsAsync(1);
        }
    }
}
