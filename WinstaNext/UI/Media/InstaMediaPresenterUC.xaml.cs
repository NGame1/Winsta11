using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaNext.Helpers;
using WinstaNext.ViewModels.Media;
using WinstaNext.Views.Profiles;
#nullable enable
// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media;

[AddINotifyPropertyChangedInterface]
public sealed partial class InstaMediaPresenterUC : UserControl
{
    public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
      nameof(Media),
      typeof(InstaMedia),
      typeof(InstaMediaPresenterUC),
      new PropertyMetadata(null));

    [OnChangedMethod(nameof(OnMediaChanged))]
    public InstaMedia Media
    {
        get { return (InstaMedia)GetValue(MediaProperty); }
        set
        {
            SetValue(MediaProperty, value);
            if (ViewModel == null) return;
            try
            {
                ViewModel.Media = value;
            }
            catch (Exception ex)
            {
                var TypeName = ex.GetType().FullName;
            }
        }
    }

    public InstaMediaPresenterUCViewModel ViewModel { get; private set; } = new();

    public InstaUserShort Me { get; private set; }

    public RelayCommand<TappedRoutedEventArgs?> ShowUserTagsCommand { get; set; }

    internal bool AnyUserTags { get => Media == null ? false : Media.UserTags == null ? false : Media.UserTags.Any(); }
    bool showTags = false;

    public InstaMediaPresenterUC()
    {
        this.InitializeComponent();
        ShowUserTagsCommand = new(ShowHideUserTAgs);
        Me = App.Container.GetService<InstaUserShort>();
    }

    bool eventRegistered = false;
    private void OnMediaChanged()
    {
        ViewModel.ImagePresenterLoaded =
            ViewModel.VideoPresenterLoaded =
                ViewModel.CarouselPresenterLoaded = false;
        switch (Media.MediaType)
        {
            case InstaMediaType.Image:
                ViewModel.ImagePresenterLoaded = true;
                break;
            case InstaMediaType.Video:
                ViewModel.VideoPresenterLoaded = true;
                if (eventRegistered)
                {
                    Media.PropertyChanged -= Media_PropertyChanged;
                }
                Media.PropertyChanged += Media_PropertyChanged;
                eventRegistered = true;
                break;
            case InstaMediaType.Carousel:
                ViewModel.CarouselPresenterLoaded = true;
                if (eventRegistered)
                {
                    Media.PropertyChanged -= Media_PropertyChanged;
                }
                Media.PropertyChanged += Media_PropertyChanged;
                //if (Media.Carousel.Any(x => x.MediaType == InstaMediaType.Video))
                //    carouselPresenter.Gallery.SelectionChanged += Gallery_SelectionChanged;
                eventRegistered = true;
                break;
            default:
                break;
        }
        Presenter_SizeChanged(null, null);
    }

    private void Gallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ShowHideUserTAgs(null);
        if (!ApplicationSettingsManager.Instance.GetAutoPlay() || !Media.Play) return;
        var gallery = carouselPresenter.Gallery;
        for (int i = 0; i < gallery.Items.Count; i++)
        {
            var item = (FlipViewItem)gallery.ContainerFromIndex(i);
            if (item == null) continue;
            if (item.ContentTemplateRoot is not InstaMediaVideoPresenterUC videoPresenter) continue;
            videoPresenter.mediaPlayer.Pause();
        }
        var fvi = (FlipViewItem)gallery.ContainerFromIndex(gallery.SelectedIndex);
        if (fvi == null) return;
        if (fvi.ContentTemplateRoot is not InstaMediaVideoPresenterUC videoPresenterUC) return;
        videoPresenterUC.mediaPlayer.Play();
    }

    void HandleVideoPlayback(InstaMediaVideoPresenterUC videoPresenter)
    {
        if (videoPresenter == null) return;
        if (!Media.Play)
            videoPresenter.mediaPlayer.Pause();
        else videoPresenter.mediaPlayer.Play();
    }

    void HandleCarouselVideos()
    {
        if (!Media.Play)
        {
            for (int i = 0; i < Media.Carousel.Count; i++)
            {
                var container = carouselPresenter.Gallery.ContainerFromIndex(i);
                var fvi = (FlipViewItem)container;
                if (container == null || fvi == null) continue;
                if (fvi.ContentTemplateRoot is InstaMediaVideoPresenterUC videoPresenterUC)
                    videoPresenterUC.mediaPlayer.Pause();
            }
        }
        else
        {
            var i = carouselPresenter.Gallery.SelectedIndex;
            var container = carouselPresenter.Gallery.ContainerFromIndex(i);
            var fvi = (FlipViewItem)container;
            if (container == null || fvi == null) return;
            if (fvi.ContentTemplateRoot is InstaMediaVideoPresenterUC videoPresenterUC)
                videoPresenterUC.mediaPlayer.Play();
        }
    }

    private void Media_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (!ApplicationSettingsManager.Instance.GetAutoPlay()) return;
        if (Media.MediaType == InstaMediaType.Carousel)
            CarouselMedia_PropertyChanged(sender, e);
        else VideoMedia_PropertyChanged(sender, e);
    }

    private async void CarouselMedia_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Media.Play)) return;
        if (Dispatcher.HasThreadAccess)
            HandleCarouselVideos();
        else await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, HandleCarouselVideos);
    }

    private async void VideoMedia_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(Media.Play)) return;
        if (Dispatcher.HasThreadAccess)
            HandleVideoPlayback(videoPresenter);
        else await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => HandleVideoPlayback(videoPresenter));
    }

    private void Presenter_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (Media == null) return;
        var parentelement = (ViewModel.NavigationService.Content as FrameworkElement);
        FrameworkElement targetElement = null;
        switch (Media.MediaType)
        {
            case InstaMediaType.Image:
                targetElement = (imagePresenter);
                break;
            case InstaMediaType.Video:
                targetElement = (videoPresenter);
                break;
            case InstaMediaType.Carousel:
                targetElement = (carouselPresenter);
                carouselPresenter.SetFlipViewSize();
                break;
        }
        if (targetElement == null) return;
        if (string.IsNullOrEmpty(Media.Height)) return;
        var minwidth = parentelement.ActualWidth;
        if (e != null)
            minwidth = parentelement.ActualWidth < e.NewSize.Width ?
                       parentelement.ActualWidth : e.NewSize.Width;
        var s = ControlSizeHelper.CalculateSizeInBox(Media.Width, int.Parse(Media.Height),
            parentelement.ActualHeight - 150, minwidth);
        targetElement.Width = s.Width;
        targetElement.Height = s.Height;
        if (UserTagsGrid != null)
        {
            UserTagsGrid.Width = s.Width;
            UserTagsGrid.Height = s.Height;
        }
    }

    void SendButtonKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        ViewModel.AddCommentCommand.Execute(null);
        args.Handled = true;
    }

    private void videoPresenter_MediaEnded(object sender, RoutedEventArgs e)
    {
        if (Media.Play && ApplicationSettingsManager.Instance.GetAutoPlay())
            videoPresenter.mediaPlayer.Play();
    }

    double CalculateXPosition(double x, double parentControlWidth, double controlWidth)
    {
        var n = x * parentControlWidth;

        if (n + controlWidth > parentControlWidth)
        {
            var more = parentControlWidth - (n + controlWidth);
            return n + more;
        }
        else if (n - controlWidth < 0)
        {

            return n + controlWidth;
        }


        return n;
    }

    double CalculateYPosition(double y, double parentControlHeight, double controlHeight)
    {
        var n = y * parentControlHeight;
        if (n + controlHeight > parentControlHeight)
        {
            var more = parentControlHeight - (n + controlHeight);
            return n + more;
        }
        else if (n - controlHeight < 0)
        {

            return n + controlHeight;
        }
        return n;
    }

    void ClearUserTags()
    {
        var tags = UserTagsGrid.Children.Where(x => x is Grid gr && gr.Name != "TagShowHide").ToList();
        foreach (var item in tags)
        {
            DoubleAnimation fade = new DoubleAnimation()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromSeconds(0.2),
                EnableDependentAnimation = true
            };
            Storyboard.SetTarget(fade, (UIElement)item);
            Storyboard.SetTargetProperty(fade, "Opacity");
            Storyboard openpane = new Storyboard();
            openpane.Children.Add(fade);
            openpane.Begin();
            openpane.Completed += new EventHandler<object>(delegate { UserTagsGrid.Children.Remove(item); });
        }
    }

    private void ShowHideUserTAgs(TappedRoutedEventArgs? obj)
    {
        List<InstaUserTag> userTags;
        if (!showTags && obj == null) return;
        showTags = obj != null ? !showTags : showTags;
        if(!showTags)
        {
            ClearUserTags();
            return;
        }
        if (Media.MediaType == InstaMediaType.Carousel)
        {
            var MediaCarousel = carouselPresenter.Gallery;
            if (MediaCarousel.SelectedIndex != -1)
                userTags = Media.Carousel[MediaCarousel.SelectedIndex].UserTags;
            else return;
        }
        else userTags = Media.UserTags;
        if (UserTagsGrid.Children.Count != 1 && obj is TappedRoutedEventArgs)
        {
            ClearUserTags();
            return;
        }
        ClearUserTags();
        foreach (var item in userTags)
        {
            var CX = CalculateXPosition(item.Position.X, UserTagsGrid.ActualWidth, 85);
            var CY = CalculateYPosition(item.Position.Y, UserTagsGrid.ActualHeight, 30);
            var trans = new CompositeTransform()
            {
                TranslateX = CX,
                TranslateY = CY,
            };
            var gr = new Grid
            {
                DataContext = item,
                RenderTransform = trans,
                Background = new SolidColorBrush(new Color() { R = 0, G = 0, B = 0, A = 185 }),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
            };
            gr.Children.Add(new TextBlock()
            {
                Foreground = new SolidColorBrush(Colors.White),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Text = item.User.UserName,
                Padding = new Thickness(2.5)
            });
            gr.Loaded += new RoutedEventHandler((s1, e1) =>
            {
                var fade = new DoubleAnimation()
                {
                    From = 0,
                    To = 1,
                    Duration = TimeSpan.FromSeconds(0.2),
                    EnableDependentAnimation = true
                };
                Storyboard.SetTarget(fade, (UIElement)s1);
                Storyboard.SetTargetProperty(fade, "Opacity");
                Storyboard openpane = new Storyboard();
                openpane.Children.Add(fade);
                openpane.Begin();
            });
            gr.Tapped += Gr_Tapped;
            UserTagsGrid.Children.Add(gr);
        }
    }

    private void Gr_Tapped(object sender, TappedRoutedEventArgs e)
    {
        if (sender is Grid Element)
        {
            ViewModel.NavigationService.Navigate(typeof(UserProfileView), (Element.DataContext as InstaUserTag).User.UserName);
        }
    }
}
