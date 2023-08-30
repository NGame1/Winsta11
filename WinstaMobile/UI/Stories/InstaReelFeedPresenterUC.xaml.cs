using Microsoft.Extensions.DependencyInjection;
using WinstaCore.Interfaces.Views.Profiles;
using Microsoft.Toolkit.Mvvm.Input;
using Windows.UI.Xaml.Controls;
using System.ComponentModel;
using Abstractions.Stories;
using WinstaCore.Services;
using PropertyChanged;
using Windows.UI.Xaml;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Stories;

[AddINotifyPropertyChangedInterface]
public sealed partial class InstaReelFeedPresenterUC : UserControl, INotifyPropertyChanged
{
    public static readonly DependencyProperty ReelFeedProperty = DependencyProperty.Register(
           nameof(ReelFeed),
           typeof(WinstaReelFeed),
           typeof(InstaReelFeedPresenterUC),
           new PropertyMetadata(null));

    public event PropertyChangedEventHandler? PropertyChanged;

    [AlsoNotifyFor(nameof(Title))]
    public WinstaReelFeed ReelFeed
    {
        get { return (WinstaReelFeed)GetValue(ReelFeedProperty); }
        set { SetValue(ReelFeedProperty, value); }
    }

    public string? Title { get => ReelFeed?.User?.UserName ?? ReelFeed?.Owner.Name; }

    public RelayCommand<object> NavigateToUserProfileCommand { get; set; }

    public InstaReelFeedPresenterUC()
    {
        NavigateToUserProfileCommand = new(NavigateToUserProfile);
        this.InitializeComponent();
    }

    void NavigateToUserProfile(object? obj)
    {
        if (obj == null) return;
        var NavigationService = App.Container.GetRequiredService<NavigationService>();
        var UserProfileView = App.Container.GetService<IUserProfileView>();
        NavigationService.Navigate(UserProfileView, obj);
    }

}
