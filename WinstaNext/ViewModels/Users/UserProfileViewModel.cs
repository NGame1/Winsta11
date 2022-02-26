using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections.IncrementalSources.Users;
using WinstaNext.Services;
using WinstaNext.Views;
using WinstaNext.Views.Profiles;

namespace WinstaNext.ViewModels.Users
{
    public class UserProfileViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        public double ViewHeight { get; set; }
        public double ViewWidth { get; set; }

        public IncrementalUserMedias MediasInstance { get; set; }
        public IncrementalUserReels ReelsInstance { get; set; }
        public IncrementalLoadingCollection<IncrementalUserReels, InstaMedia> UserReels{ get; private set; }
        public IncrementalLoadingCollection<IncrementalUserMedias, InstaMedia> UserMedias { get; private set; }

        public InstaUser User { get; private set; }
        public ScrollViewer ListViewScroll { get; set; }
        public UserProfileViewModel() : base()
        {
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            if (e.Parameter is long userId)
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByIdAsync(userId,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value.Adapt<InstaUser>();
                }
            }
            else if(e.Parameter is string username && !string.IsNullOrEmpty(username))
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.GetUserInfoByUsernameAsync(username,
                                       surfaceType: InstaMediaSurfaceType.Profile);
                    if (!result.Succeeded)
                    {
                        if (NavigationService.CanGoBack)
                            NavigationService.GoBack();
                        throw result.Info.Exception;
                    }
                    User = result.Value.Adapt<InstaUser>();
                }
            }
            else if(e.Parameter is InstaUserShortFriendshipFull user)
            {
                User = new(user);
            }
            else if(e.Parameter is InstaCurrentUser currentUser)
            {
                User = new(currentUser);
            }
            else
            {
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
                throw new ArgumentOutOfRangeException(nameof(e.Parameter));
            }

            ReelsInstance = new(User.Pk);
            MediasInstance = new(User.Pk);
            UserReels = new(ReelsInstance);
            UserMedias = new(MediasInstance);
            await base.OnNavigatedToAsync(e);
        }

        private void UserProfileViewModel_SizeChanged(object sender, Windows.UI.Xaml.SizeChangedEventArgs e)
        {
            ViewHeight = e.NewSize.Height;
            ViewWidth = e.NewSize.Width;
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            SetHeader();
            (NavigationService.Content as UserProfileView).SizeChanged += UserProfileViewModel_SizeChanged;
            base.OnNavigatedTo(e);
        }

    }
}
