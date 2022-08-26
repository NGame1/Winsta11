using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Linq;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using WinstaCore.Constants;
using WinstaCore.Interfaces.Views.Profiles;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Services;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Stories.StickersView
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class StickersViewGrid : Grid
    {
        public static readonly DependencyProperty PresenterProperty = DependencyProperty.Register(
             nameof(Presenter),
             typeof(InstaStoryItemPresenterUC),
             typeof(StickersViewGrid),
             new PropertyMetadata(null));

        //[OnChangedMethod(nameof(InitializeView))]
        public InstaStoryItemPresenterUC Presenter
        {
            get { return (InstaStoryItemPresenterUC)GetValue(PresenterProperty); }
            set { SetValue(PresenterProperty, value); }
        }

        public RelayCommand? PauseTimerCommand { get; set; }
        public RelayCommand? ResumeTimerCommand { get; set; }

        public InstaStoryItem? StoryItem { get; set; }

        public StickersViewGrid()
        {
            this.InitializeComponent();
        }

        public void InitializeView()
        {
            if (Presenter == null) return;
            this.Children.Clear();
            PauseTimerCommand = new(Presenter.Pause);
            ResumeTimerCommand = new(Presenter.Resume);
            StoryItem = Presenter.Story;
            if (StoryItem.ReelMentions.Any())
            {
                for (int i = 0; i < StoryItem.ReelMentions.Count; i++)
                {
                    var mention = StoryItem.ReelMentions.ElementAt(i);
                    var rect = new Rectangle();
                    SetStickerPosition(ref rect, mention.Height, mention.Width, mention.X, mention.Y, mention.Rotation);
                    rect.DataContext = mention;
                    rect.Tapped += Mention_Tapped;
                    this.Children.Add(rect);
                }
            }
            if (StoryItem.StoryHashtags.Any())
            {
                for (int i = 0; i < StoryItem.StoryHashtags.Count; i++)
                {
                    var mention = StoryItem.StoryHashtags.ElementAt(i);
                    var rect = new Rectangle();
                    SetStickerPosition(ref rect, mention.Height, mention.Width, mention.X, mention.Y, mention.Rotation);
                    rect.DataContext = mention;
                    rect.Tapped += Mention_Tapped;
                    this.Children.Add(rect);
                }
            }
            if (StoryItem.StoryLinkStickers.Any())
            {
                for (int i = 0; i < StoryItem.StoryLinkStickers.Count; i++)
                {
                    var link = StoryItem.StoryLinkStickers.ElementAt(i);
                    var rect = new Rectangle();
                    SetStickerPosition(ref rect, link.Height, link.Width, link.X, link.Y, link.Rotation);
                    rect.DataContext = link;
                    rect.Tapped += link_Tapped;
                    this.Children.Add(rect);
                }
            }
            if (StoryItem.StoryLocations.Any())
            {
                for (int i = 0; i < StoryItem.StoryLocations.Count; i++)
                {
                    var place = StoryItem.StoryLocations.ElementAt(i);
                    var rect = new Rectangle();
                    SetStickerPosition(ref rect, place.Height, place.Width, place.X, place.Y, place.Rotation);
                    rect.DataContext = place;
                    rect.Tapped += Place_Tapped;
                    this.Children.Add(rect);
                }
            }
            if (StoryItem.StoryFeedMedia.Any())
            {
                for (int i = 0; i < StoryItem.StoryFeedMedia.Count; i++)
                {
                    var media = StoryItem.StoryFeedMedia.ElementAt(i);
                    var rect = new Rectangle();
                    SetStickerPosition(ref rect, media.Height, media.Width, media.X, media.Y, media.Rotation);
                    rect.DataContext = media;
                    rect.Tapped += Media_Tapped;
                    this.Children.Add(rect);
                }
            }
            //if (StoryItem.StoryPolls.Any())
            //{
            //    for (int i = 0; i < StoryItem.StoryPolls.Count; i++)
            //    {
            //        var poll = StoryItem.StoryPolls.ElementAt(i);
            //        var rect = new Grid() { CornerRadius = new(14) };
            //        var pollSticker = new PollStickerUC();
            //        rect.Children.Add(pollSticker);
            //        SetStickerPosition(ref rect, poll.Height, poll.Width, poll.X, poll.Y, poll.Rotation);
            //        this.Children.Add(rect);
            //        pollSticker.SetBinding(PollStickerUC.PollProperty, new Binding()
            //        {
            //            Source = poll,
            //            Mode = BindingMode.OneWay,
            //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //        });
            //    }
            //}
            //if (StoryItem.StoryQuestions.Any())
            //{
            //    for (int i = 0; i < StoryItem.StoryQuestions.Count; i++)
            //    {
            //        var question = StoryItem.StoryQuestions.ElementAt(i);
            //        var rect = new Grid() { CornerRadius = new(14) };
            //        var questionSticker = new QuestionStickerUC();
            //        rect.Children.Add(questionSticker);
            //        SetStickerPosition(ref rect, question.Height, question.Width, question.X, question.Y, question.Rotation);
            //        this.Children.Add(rect);
            //        questionSticker.SetBinding(QuestionStickerUC.QuestionProperty, new Binding()
            //        {
            //            Source = question,
            //            Mode = BindingMode.OneWay,
            //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //        });
            //    }
            //}
            //if (StoryItem.StorySliders.Any())
            //{
            //    for (int i = 0; i < StoryItem.StorySliders.Count; i++)
            //    {
            //        var slider = StoryItem.StorySliders.ElementAt(i);
            //        var rect = new Grid() { CornerRadius = new(5) };
            //        var slidernSticker = new SliderStickerUC();
            //        rect.Children.Add(slidernSticker);
            //        SetStickerPosition(ref rect, slider.Height, slider.Width, slider.X, slider.Y, slider.Rotation);
            //        this.Children.Add(rect);
            //        slidernSticker.SetBinding(SliderStickerUC.SliderProperty, new Binding()
            //        {
            //            Source = slider,
            //            Mode = BindingMode.OneWay,
            //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //        });
            //    }
            //}
            //if (StoryItem.StoryQuizs.Any())
            //{
            //    for (int i = 0; i < StoryItem.StoryQuizs.Count; i++)
            //    {
            //        var quiz = StoryItem.StoryQuizs.ElementAt(i);
            //        var rect = new Grid() { CornerRadius = new(5) };
            //        var quizSticker = new QuizStickerUC();
            //        rect.Children.Add(quizSticker);
            //        SetStickerPosition(ref rect, quiz.Height, quiz.Width, quiz.X, quiz.Y, quiz.Rotation);
            //        this.Children.Add(rect);
            //        quizSticker.SetBinding(QuizStickerUC.QuizProperty, new Binding()
            //        {
            //            Source = quiz,
            //            Mode = BindingMode.OneWay,
            //            UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            //        });
            //    }
            //}
        }

        void Media_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PauseTimerCommand == null) return;
            PauseTimerCommand.Execute(null);
            var target = (FrameworkElement)e.OriginalSource;
            if (target.DataContext is not InstaStoryFeedMedia media) return;

            var font = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            tip.Title = "";
            tip.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource() { FontFamily = font, Glyph = FluentRegularFontCharacters.Image };
            tip.Subtitle = "Tap to see the post";
            tip.DataContext = media;
            tip.ActionButtonContent = "See post";
            tip.Target = target;
            tip.IsOpen = true;
        }

        void Mention_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PauseTimerCommand == null) return;
            PauseTimerCommand.Execute(null);
            var target = (FrameworkElement)e.OriginalSource;
            if (target.DataContext is not InstaReelMention mention) return;

            var font = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            tip.Title = mention.User != null ? mention.User.UserName : mention.Hashtag.Name;
            tip.IconSource = mention.User != null ? new Microsoft.UI.Xaml.Controls.ImageIconSource() { ImageSource = new BitmapImage(new(mention.User.ProfilePicture)) { DecodePixelHeight = 35, DecodePixelWidth = 35 } } :
                                                    new Microsoft.UI.Xaml.Controls.FontIconSource() { FontFamily = font, Glyph = FluentRegularFontCharacters.Hashtag };
            tip.Subtitle = "Tap to see the profile";
            tip.DataContext = mention.User != null ? mention.User : mention.Hashtag;
            tip.ActionButtonContent = "See profile";
            tip.Target = target;
            tip.IsOpen = true;
        }

        void link_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PauseTimerCommand == null) return;
            PauseTimerCommand.Execute(null);
            var target = (FrameworkElement)e.OriginalSource;
            if (target.DataContext is not InstaStoryLinkStickerItem link) return;

            var font = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            tip.Title = link.StoryLink.LinkTitle;
            tip.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource() { FontFamily = font, Glyph = FluentRegularFontCharacters.Web };
            tip.Subtitle = "Tap to see the link";
            tip.DataContext = link.StoryLink;
            tip.ActionButtonContent = "See link";
            tip.Target = target;
            tip.IsOpen = true;
        }

        void Place_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (PauseTimerCommand == null) return;
            PauseTimerCommand.Execute(null);
            var target = (FrameworkElement)e.OriginalSource;
            if (target.DataContext is not InstaStoryLocation place) return;

            var font = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];
            tip.Title = place.Location.Name;
            tip.IconSource = new Microsoft.UI.Xaml.Controls.FontIconSource() { FontFamily = font, Glyph = FluentRegularFontCharacters.Location };
            tip.Subtitle = "Tap to see the place";
            tip.DataContext = place.Location;
            tip.ActionButtonContent = "See place";
            tip.Target = target;
            tip.IsOpen = true;
        }

        void SetStickerPosition(ref Rectangle rect, double height, double width, double x, double y, double rotation)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var actwidth = this.ActualWidth;
            var actheight = this.ActualHeight;
            var size = CalculateSizeInBox(StoryItem.OriginalWidth, StoryItem.OriginalHeight, actheight, actwidth);
            var trans = new CompositeTransform() { CenterX = (size.Width * width / 2), CenterY = (size.Height * height / 2), Rotation = rotation * 360 };
            var marg = new Thickness(((x * size.Width) - ((width / 2) * size.Width)),
                ((y * size.Height) - ((height / 2) * size.Height)), 0, 0);
#if DEBUG
            rect.Stroke = new SolidColorBrush(Colors.Red);
            rect.StrokeThickness = 1;
#endif
            rect.Margin = marg;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.Fill = new SolidColorBrush(new Color() { A = 1 });
            rect.RenderTransform = trans;
            rect.Width = width * ActualWidth;
            rect.Height = height * ActualHeight;
        }

        void SetStickerPosition(ref Grid rect, double height, double width, double x, double y, double rotation)
        {
            var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
            var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
            var actwidth = this.ActualWidth;
            var actheight = this.ActualHeight;
            var size = CalculateSizeInBox(StoryItem.OriginalWidth, StoryItem.OriginalHeight, actheight, actwidth);
            var trans = new CompositeTransform() { CenterX = (size.Width * width / 2), CenterY = (size.Height * height / 2), Rotation = rotation * 360 };
            var marg = new Thickness(((x * size.Width) - ((width / 2) * size.Width)),
                ((y * size.Height) - ((height / 2) * size.Height)), 0, 0);
#if DEBUG
            rect.BorderBrush = new SolidColorBrush(Colors.Red);
            rect.BorderThickness = new(1);
#endif
            rect.Margin = marg;
            rect.VerticalAlignment = VerticalAlignment.Top;
            rect.HorizontalAlignment = HorizontalAlignment.Left;
            rect.Background = new SolidColorBrush(new Color() { A = 1 });
            rect.RenderTransform = trans;
            rect.Width = width * ActualWidth;
            rect.Height = height * ActualHeight;
        }

        Size CalculateSizeInBox(double imageWidth, double imageHeight, double windowHeight, double windowWidth)
        {
            double dbl = imageWidth / imageHeight;
            if (windowHeight * dbl <= windowWidth)
                return new Size((windowHeight * dbl), windowHeight);
            else
                return new Size(windowWidth, (windowWidth / dbl));
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeView();
        }

        private void tip_Closed(Microsoft.UI.Xaml.Controls.TeachingTip sender, Microsoft.UI.Xaml.Controls.TeachingTipClosedEventArgs args)
        {
            ResumeTimerCommand?.Execute(null);
        }

        async void tip_ActionButtonClick(Microsoft.UI.Xaml.Controls.TeachingTip sender, object args)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            var dt = sender.DataContext;
            sender.IsOpen = false;
            if (dt is InstaUserShort user)
            {
                var UserProfileView = App.Container.GetService<IUserProfileView>();
                NavigationService?.Navigate(UserProfileView, user);
            }
            if (dt is InstaHashtag hashtag)
            {
                var HashtagProfileView = App.Container.GetService<IHashtagProfileView>();
                NavigationService?.Navigate(HashtagProfileView, hashtag);
            }
            if (dt is InstaPlaceShort place)
            {
                var PlaceProfileView = App.Container.GetService<IPlaceProfileView>();
                NavigationService?.Navigate(PlaceProfileView, place);
            }
            if (dt is InstaStoryFeedMedia media)
            {
                var SingleInstaMediaView = App.Container.GetService<ISingleInstaMediaView>();
                NavigationService?.Navigate(SingleInstaMediaView, media.MediaId);
            }
            if (dt is InstaStoryLink link)
            {
                await Launcher.LaunchUriAsync(new(link.Url, UriKind.RelativeOrAbsolute));
            }
        }
    }
}
