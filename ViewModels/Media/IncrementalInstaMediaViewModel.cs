using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Attributes;
using Abstractions.Navigation;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using WinstaCore.Helpers.ExtensionMethods;

namespace ViewModels.Media
{
    public class IncrementalInstaMediaViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }

        public RangePlayerAttribute MediaSource { get; set; }
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
            base.OnNavigatedTo(e);
        }

        void OnpropertyChanged(PropertyChangedEventArgs e)
        {

        }
    }
}
