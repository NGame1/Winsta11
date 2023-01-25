using Abstractions.Navigation;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Services;
using WinstaNext.ViewModels.Media;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media;

[AddINotifyPropertyChangedInterface]
public sealed partial class StaggeredTileUC : UserControl
{
    public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
                nameof(Media),
                typeof(InstaMedia),
                typeof(StaggeredTileUC),
                new PropertyMetadata(null));

    public InstaMedia Media
    {
        get { return (InstaMedia)GetValue(MediaProperty); }
        set { SetValue(MediaProperty, value); ViewModel.Media = value; }
    }

    public InstaMediaPresenterUCViewModel ViewModel { get; } = new();

    public AsyncRelayCommand LikeMediaCommand { get => ViewModel.LikeMediaCommand; }
    public AsyncRelayCommand TapCommand { get; set; }
    public RelayCommand DoubleTappedCommand { get; set; }

    public bool LoadLikeAnimation { get => ViewModel.LoadLikeAnimation; }
    public bool LoadUnLikeAnimation { get => ViewModel.LoadUnLikeAnimation; }

    public StaggeredTileUC()
    {
        this.InitializeComponent();
        TapCommand = new(TapHandler);
        DoubleTappedCommand = new(DoubleTappedHandler);
    }

    public async void NavigateToMedia(RangePlayerAttribute rangePlayer)
    {
        await Task.Delay(410);
        if (TapCommand.IsRunning) return;
        DisposeStopwatch();
        var NavigationService = App.Container.GetRequiredService<NavigationService>();
        var mediaView = App.Container.GetRequiredService<IIncrementalInstaMediaView>();
        NavigationService.Navigate(mediaView, new IncrementalMediaViewParameter(rangePlayer, Media));
    }

    Stopwatch? Stopwatch { get; set; }
    int TapsCount { get; set; } = 0;
    async Task TapHandler()
    {
        Stopwatch ??= Stopwatch.StartNew();
        Stopwatch.Restart();
        TapsCount++;
        await Task.Delay(400);

        if (TapsCount >= 2)
        {
            DisposeStopwatch();
            await ViewModel.LikeMediaCommand.ExecuteAsync(null);
        }
    }

    void DoubleTappedHandler()
    {
        TapsCount = 2;
    }

    void DisposeStopwatch()
    {
        Stopwatch?.Stop();
        Stopwatch = null;
        TapsCount = 0;
    }
}
