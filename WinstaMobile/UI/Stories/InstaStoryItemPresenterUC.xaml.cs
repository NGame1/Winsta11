﻿using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Threading.Tasks;
using WinstaCore.Services;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinstaMobile.Views.Profiles;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Stories
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaStoryItemPresenterUC : UserControl
    {
        public static readonly DependencyProperty StoryProperty = DependencyProperty.Register(
             nameof(Story),
             typeof(InstaStoryItem),
             typeof(InstaStoryItemPresenterUC),
             new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnStoryChanged))]
        public InstaStoryItem Story
        {
            get { return (InstaStoryItem)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        public bool LoadImage { get; set; } = false;
        public bool LoadVideo { get; set; } = false;

        RelayCommand<object> NavigateToUserProfileCommand { get; set; }
        AsyncRelayCommand LikeStoryCommand { get; set; }
        AsyncRelayCommand ReplyStoryCommand { get; set; }

        public string ReplyText { get; set; } = string.Empty;
        InstaStoryItemPresenterUC? presenterUC = null;

        public InstaStoryItemPresenterUC()
        {
            this.InitializeComponent();
            presenterUC = this;
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            LikeStoryCommand = new(LikeStoryAsync);
            ReplyStoryCommand = new(ReplyStoryAsync);
        }

        void SendMessageKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            ReplyStoryCommand.Execute(null);
            args.Handled = true;
        }

        async Task ReplyStoryAsync()
        {
            if (ReplyStoryCommand.IsRunning) return;
            if (string.IsNullOrEmpty(ReplyText)) return;
            using (var Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.StoryProcessor.ReplyToStoryAsync(Story.Id,
                    Story.User.Pk,
                    ReplyText,
                    Story.MediaType == InstaMediaType.Image ? InstaSharingType.Photo : InstaSharingType.Video);
                if (!result.Succeeded) throw result.Info.Exception;
                ReplyText = string.Empty;
            }
        }

        async Task LikeStoryAsync()
        {
            if (LikeStoryCommand.IsRunning) return;
            using (var Api = App.Container.GetService<IInstaApi>())
            {
                var isliked = Story.HasLiked;
                Story.HasLiked = !Story.HasLiked;
                IResult<bool> result;
                if (!isliked)
                    result = await Api.StoryProcessor.LikeStoryAsync(Story.Id);
                else result = await Api.StoryProcessor.UnlikeStoryAsync(Story.Id);
                if (!result.Succeeded)
                {
                    Story.HasLiked = isliked;
                    throw result.Info.Exception;
                }
            }
        }

        void NavigateToUserProfile(object? obj)
        {
            if (Story.User.CloseButton.HasValue && Story.User.CloseButton.Value) return;
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService?.Navigate(typeof(UserProfileView), obj);
        }

        void OnStoryChanged()
        {
            if (Story is null) return;
            LoadImage = LoadVideo = false;
            if (Story.MediaType == InstaMediaType.Video)
                LoadVideo = true;
            else LoadImage = true;
        }

        DispatcherTimer? _timer = null;
        bool imageOpened = false;
        double imagetimer = 0;
        IProgress<double>? _progress = null;
        public void Play(IProgress<double> progress)
        {
            _progress = progress;
            _timer = new() { Interval = TimeSpan.FromMilliseconds(50) };
            StickersViewGrid.InitializeView();
            if (LoadVideo)
            {
                if (videoplayer == null) FindName(nameof(videoplayer));
                videoplayer?.Play();
                _timer.Tick += VideoTimer_Tick;
                _timer.Start();
            }
            else
            {
                _timer.Tick += ImageTimer_Tick;
                if (imageOpened)
                    _timer.Start();
            }
        }

        public void Pause()
        {
            if (LoadVideo)
            {
                if (videoplayer == null) FindName(nameof(videoplayer));
                videoplayer?.Pause();
                if (_timer == null) return;
                _timer.Tick -= VideoTimer_Tick;
            }
            else
            {
                if (_timer == null) return;
                _timer.Tick -= ImageTimer_Tick;
                _timer.Stop();
            }
        }

        public void Resume()
        {
            if (_timer == null) return;
            if (LoadVideo)
            {
                if (videoplayer == null) FindName(nameof(videoplayer));
                videoplayer?.Play();
                _timer.Tick += VideoTimer_Tick;
                _timer.Start();
            }
            else
            {
                _timer.Tick += ImageTimer_Tick;
                if (imageOpened)
                    _timer.Start();
            }
        }

        public void Stop()
        {
            _progress = null;
            if (LoadVideo)
            {
                if (videoplayer == null) FindName(nameof(videoplayer));
                videoplayer?.Stop();
                if (_timer == null) return;
                _timer.Tick -= VideoTimer_Tick;
                _timer.Stop();
                _timer = null;
            }
            else
            {
                imagetimer = 0;
                if (_timer == null) return;
                _timer.Tick -= ImageTimer_Tick;
                _timer = null;
            }
        }

        private void videoplayer_MediaEnded(object? sender, RoutedEventArgs e)
        {
            _progress?.Report(100);
            Stop();
        }

        private void VideoTimer_Tick(object? sender, object? e)
        {
            if (_progress != null && _timer != null)
            {
                var rep = (videoplayer.Position.TotalMilliseconds / videoplayer.NaturalDuration.TimeSpan.TotalMilliseconds) * 100;
                if (rep != 100)
                    _progress.Report(rep);
            }
        }

        private void ImageTimer_Tick(object sender, object e)
        {
            if (_progress != null && _timer != null)
            {
                _progress.Report((imagetimer / 10000) * 100);
                imagetimer += 50;
            }
        }

        private void imageviewer_ImageOpened(object sender, RoutedEventArgs e)
        {
            imageOpened = true;
            if (_timer != null && !_timer.IsEnabled)
                _timer.Start();
        }

        private void videoplayer_Loaded(object? sender, RoutedEventArgs e)
        {
            videoplayer.Source = new(Story.Videos[0].Uri, UriKind.RelativeOrAbsolute);
        }

        private void Element_GotFocus(object sender, RoutedEventArgs e) => Pause();
        private void Element_LostFocus(object sender, RoutedEventArgs e) => Resume();
        private void Element_PointerPressed(object sender, PointerRoutedEventArgs e) => Pause();
        private void Element_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.Focus(FocusState.Programmatic);
            Resume();
        }
        private void InstaStoryItemFlyout_Opened(object sender, object e) => Pause();
        private void InstaStoryItemFlyout_Closed(object sender, object e) => Resume();
    }
}
