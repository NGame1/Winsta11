using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaStoryItemPresenterUC : UserControl
    {
        public static readonly DependencyProperty StoryProperty = DependencyProperty.Register(
             "Story",
             typeof(InstaStoryItem),
             typeof(InstaStoryItemPresenterUC),
             new PropertyMetadata(null));

        public InstaStoryItem Story
        {
            get { return (InstaStoryItem)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        RelayCommand<object> NavigateToUserProfileCommand { get; set; }
        AsyncRelayCommand LikeStoryCommand { get; set; }
        AsyncRelayCommand ReplyStoryCommand { get; set; }

        public string ReplyText { get; set; } = string.Empty;

        public InstaStoryItemPresenterUC()
        {
            this.InitializeComponent();
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

        void NavigateToUserProfile(object obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }

    }
}
