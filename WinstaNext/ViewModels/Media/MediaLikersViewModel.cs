using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Media;
using WinstaNext.Core.Collections.IncrementalSources.Media;

namespace WinstaNext.ViewModels.Media
{
    internal class MediaLikersViewModel : BaseViewModel
    {
        public IncrementalMediaLikers Instance { get; private set; }
        public IncrementalLoadingCollection<IncrementalMediaLikers, WinstaMediaLikerUser> MediaLikers { get; private set; }
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.MediaLikers;

        public MediaLikersViewModel()
        {

        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is string mediaId)
            {
                if (string.IsNullOrEmpty(mediaId))
                {
                    NavigationService.GoBack();
                    return;
                }
                Instance = new(mediaId);
                MediaLikers = new(Instance);
                return;
            }

            NavigationService.GoBack();
            return;
        }
    }
}
