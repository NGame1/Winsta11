using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

    public static readonly DependencyProperty ShowUserProperty = DependencyProperty.Register(
                nameof(ShowUser),
                typeof(bool),
                typeof(StaggeredTileUC),
                new PropertyMetadata(null));

    public bool ShowUser
    {
        get { return (bool)GetValue(ShowUserProperty); }
        set { SetValue(ShowUserProperty, value); }
    }

    public InstaMedia Media
    {
        get { return (InstaMedia)GetValue(MediaProperty); }
        set { SetValue(MediaProperty, value); ViewModel.Media = value; }
    }

    public InstaMediaPresenterUCViewModel ViewModel { get; } = new();

    public AsyncRelayCommand LikeMediaCommand { get => ViewModel.LikeMediaCommand; }

    public StaggeredTileUC()
    {
        this.InitializeComponent();
    }

}
