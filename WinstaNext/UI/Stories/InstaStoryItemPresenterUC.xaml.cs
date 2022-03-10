using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    //[AddINotifyPropertyChangedInterface]
    public sealed partial class InstaStoryItemPresenterUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty StoryProperty = DependencyProperty.Register(
             "Story",
             typeof(InstaStoryItem),
             typeof(InstaStoryItemPresenterUC),
             new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        [OnChangedMethod(nameof(OnStoryChanged))]
        public InstaStoryItem Story
        {
            get { return (InstaStoryItem)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        RelayCommand<object> NavigateToUserProfileCommand { get; set; }
        AsyncRelayCommand LikeStoryCommand { get; set; }
        AsyncRelayCommand ReplyStoryCommand { get; set; }

        public bool LoadImage { get; set; } = false;
        public bool LoadMediaElement { get; set; } = false;
        public string ReplyText { get; set; }

        public event EventHandler<bool> TimerEnded;
        DispatcherTimer timer;
        public InstaStoryItemPresenterUC()
        {
            this.InitializeComponent();
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            LikeStoryCommand = new(LikeStoryAsync);
            ReplyStoryCommand = new(ReplyStoryAsync);
        }

        async Task ReplyStoryAsync()
        {
            if (ReplyStoryCommand.IsRunning) return;
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

        void NavigateToUserProfile(object obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }

        void OnStoryChanged()
        {
            LoadMediaElement = LoadImage = false;
            if (Story.MediaType == InstaMediaType.Video)
                LoadMediaElement = true;
            else LoadImage = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadImage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadMediaElement)));
        }

        public void StartTimer()
        {
            timer = new DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(7000) };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        public void StopTimer()
        {
            if (timer == null) return;
            timer.Tick -= Timer_Tick;
            timer.Stop();
            timer = null;
        }

        private void Timer_Tick(object sender, object e)
        {
            TimerEnded?.Invoke(this, true);
        }

        private void videoplayer_MediaEnded(object sender, RoutedEventArgs e)
        {
            TimerEnded?.Invoke(this, true);
        }
    }
}
