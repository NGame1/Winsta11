using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Search
{
    public sealed partial class PeopleSearchUC : UserControl
    {
        public static readonly DependencyProperty UserProperty = DependencyProperty.Register(
          nameof(User),
          typeof(InstaUser),
          typeof(PeopleSearchUC),
          new PropertyMetadata(null));

        public InstaUser User
        {
            get { return (InstaUser)GetValue(UserProperty); }
            set { SetValue(UserProperty, value); }
        }

        public AsyncRelayCommand FollowButtonCommand { get; set; }


        public PeopleSearchUC()
        {
            this.InitializeComponent();
            FollowButtonCommand = new(FollowAsync);
        }

        public async Task FollowAsync()
        {
            if (FollowButtonCommand.IsRunning) return;
            if (User.Pk == App.Container.GetService<InstaUserShort>().Pk)
            {
                //Edit profile
                throw new NotSupportedException();
            }
            if (User.FriendshipStatus == null)
                throw new ArgumentNullException(nameof(User.FriendshipStatus));

            var follow = !User.FriendshipStatus.Following;

            IResult<InstaFriendshipFullStatus> result;
            using (IInstaApi Api = App.Container?.GetService<IInstaApi>())
            {
                if (follow)
                    result = await Api.UserProcessor.FollowUserAsync(User.Pk,
                             surfaceType: InstaMediaSurfaceType.Profile,
                             mediaIdAttribution: null);
                else result = await Api.UserProcessor.UnFollowUserAsync(User.Pk,
                             surfaceType: InstaMediaSurfaceType.Profile,
                             mediaIdAttribution: null);
            }
            if (!result.Succeeded) throw result.Info.Exception;
            User.FriendshipStatus = result.Value.Adapt<InstaFriendshipShortStatus>();
        }
    }
}
