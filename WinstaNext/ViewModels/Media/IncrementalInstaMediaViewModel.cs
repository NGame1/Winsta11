using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections.IncrementalSources.Media;
using WinstaNext.Core.Navigation;

namespace WinstaNext.ViewModels.Media
{
    internal class IncrementalInstaMediaViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public ISupportIncrementalLoading MediaSource { get; set; }
        InstaMedia TargetMedia { get; set; }
        //int TargetIndex { get; set; }

        public AsyncRelayCommand<ListView> ListViewLoadedCommand { get; set; }

        public IncrementalInstaMediaViewModel()
        {
            ListViewLoadedCommand = new(ListViewLoadedAsync);
        }

        async Task ListViewLoadedAsync(ListView lst)
        {
            if (ListViewLoadedCommand.IsRunning) return;
            lst.ScrollIntoView(TargetMedia);
            await Task.Delay(100);
            await lst.SmoothScrollIntoViewWithItemAsync(TargetMedia,
                      itemPlacement: ScrollItemPlacement.Top,
                      disableAnimation: true);
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not IncrementalMediaViewParameter parameter)
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }
            MediaSource = parameter.MediaSource;
            TargetMedia = parameter.TargetMedia;
            //TargetIndex = parameter.TargetIndex;
            base.OnNavigatedTo(e);
        }

        void OnpropertyChanged(PropertyChangedEventArgs e)
        {

        }
    }
}
