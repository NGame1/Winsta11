using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI.Controls;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore.Helpers.ExtensionMethods;
using WinstaCore.Services;
using WinstaNext.Views.Comments;
using WinstaNext.Views.Profiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Comments
{
    public sealed partial class InstaCommentPresenterUC : UserControl
    {
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(
          nameof(Comment),
          typeof(InstaComment),
          typeof(InstaCommentPresenterUC),
          new PropertyMetadata(null));

        public InstaComment Comment
        {
            get { return (InstaComment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        PaginationParameters Pagination { get; set; } = PaginationParameters.MaxPagesToLoad(1);

        public AsyncRelayCommand LikeCommecntCommand { get; set; }
        public AsyncRelayCommand LoadMoreCommentsCommand { get; set; }
        public RelayCommand ReplyCommand { get; set; }

        public RelayCommand<object> NavigateToUserProfileCommand { get; set; }
        public RelayCommand NavigateToCommentLikersCommand { get; set; }

        public RelayCommand<LinkClickedEventArgs> CaptionLinkClickedCommand { get; set; }

        public InstaCommentPresenterUC()
        {
            CaptionLinkClickedCommand = new(CaptionLinkClicked);
            LoadMoreCommentsCommand = new(LoadMoreCommentsAsync);
            LikeCommecntCommand = new(LikeCommentAsync);
            ReplyCommand = new(Reply);
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            NavigateToCommentLikersCommand = new(NavigateToCommentLikers);
            this.InitializeComponent();
        }

        void CaptionLinkClicked(LinkClickedEventArgs obj)
            => obj.HandleClickEvent();

        async Task LikeCommentAsync()
        {
            IResult<bool> result = null;
            bool liked = Comment.HasLikedComment;
            var likesCount = Comment.LikesCount;
            Comment.HasLikedComment = !Comment.HasLikedComment;
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    if (liked)
                    {
                        Comment.LikesCount--;
                        result = await Api.CommentProcessor.UnlikeCommentAsync(Comment.Pk.ToString());
                    }
                    else
                    {
                        Comment.LikesCount++;
                        result = await Api.CommentProcessor.LikeCommentAsync(Comment.Pk.ToString());
                    }
                }
            }
            finally
            {
                if (!result.Succeeded)
                {
                    Comment.HasLikedComment = liked;
                    Comment.LikesCount = likesCount;
                }
            }
        }

        async Task LoadMoreCommentsAsync()
        {
            if (LoadMoreCommentsCommand.IsRunning) return;
            var NavigationService = App.Container.GetService<NavigationService>();
            string mediaId = string.Empty;
            if (NavigationService.Content is MediaCommentsView view)
            {
                mediaId = view.MediaId;
            }
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.CommentProcessor.GetMediaRepliesCommentsAsync(mediaId,
                                                            Comment.Pk.ToString(), Pagination);

                    if (!result.Succeeded) throw result.Info.Exception;

                    var childs = result.Value.ChildComments;
                    //childs.Reverse();
                    for (int i = 0; i < childs.Count; i++)
                    {
                        if (!Comment.ChildComments.Any(x => x.Pk == childs.ElementAt(i).Pk))
                            Comment.ChildComments.Add( childs.ElementAt(i));
                    }
                    Comment.HasMoreTailChildComments = result.Value.HasMoreTailChildComments;
                    Comment.HasMoreHeadChildComments = result.Value.HasMoreHeadChildComments;
                }
            }
            finally
            {
            }
        }

        void Reply()
        {
            try
            {
                var NavigationService = App.Container.GetService<NavigationService>();
                string mediaId = string.Empty;
                if (NavigationService.Content is MediaCommentsView view)
                {
                    mediaId = view.MediaId;
                    view.ViewModel.ReplyedComment = this.Comment;
                }

            }
            finally
            {
            }
        }

        void NavigateToUserProfile(object obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }

        void NavigateToCommentLikers()
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(CommentLikersView), this.Comment.Pk.ToString());
        }
    }
}
