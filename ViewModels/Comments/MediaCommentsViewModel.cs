using Core.Collections.IncrementalSources.Comments;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using PropertyChanged;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Abstractions.Navigation;
using WinstaCore.Helpers;
using WinstaCore;
using WinstaCore.Helpers.ExtensionMethods;
using System;
#nullable enable

namespace ViewModels.Comments
{
    public class MediaCommentsViewModel : BaseViewModel
    {
        [AlsoNotifyFor(nameof(IsSendCommentButtonEnabled))]
        public string CommentText { get; set; } = "";

        bool _isSendenabled = false;
        public bool IsSendCommentButtonEnabled { get => CommentText.Length != 0 || _isSendenabled; }

        IncrementalMediaComments? Instance { get; set; }
        public IncrementalLoadingCollection<IncrementalMediaComments, InstaComment>? Comments { get; private set; }
        public InstaUserShort? Me { get; }
        public InstaMedia? Media { get; set; }

        public InstaComment? ReplyedComment { get; set; } = null;

        public AsyncRelayCommand<ListView> AddCommentCommand { get; set; }

        public RelayCommand IgnoreReplyCommand { get; set; }

        public MediaCommentsViewModel()
        {
            Me = AppCore.Container.GetService<InstaUserShort>();
            AddCommentCommand = new(AddCommentAsync);
            IgnoreReplyCommand = new(IgnoreReply);
        }

        void IgnoreReply() => ReplyedComment = null;

        async Task AddCommentAsync(ListView? lst)
        {
            if (AddCommentCommand.IsRunning) return;
            if (!IsSendCommentButtonEnabled) return;
            if (Media == null) throw new ArgumentNullException(nameof(Media));  
            try
            {
                InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.ModalCommentComposerFeedTimeline;
                var Frame = NavigationService.Content;
                var isCarouselBumpedPost = Media.MediaType == InstaMediaType.Carousel;
                int? carouselIndex = null;
                if (isCarouselBumpedPost)
                    carouselIndex = 0;
                uint feedPosition = 0;
                using (IInstaApi? Api = AppCore.Container.GetService<IInstaApi>())
                {
                    if (Api == null) return;
                    IResult<InstaComment>? result = null;
                    if (ReplyedComment == null)
                    {
                        result = await Api.CommentProcessor.CommentMediaAsync(Media.InstaIdentifier, CommentText,
                                 containerModule: containerModule,
                                 feedPosition: feedPosition,
                                 isCarouselBumpedPost: isCarouselBumpedPost,
                                 carouselIndex: carouselIndex);

                        if (!result.Succeeded)
                        {
                            MessageDialogHelper.Show(result.Info.Message);
                            return;
                        }
                        Comments?.Insert(0, result.Value);
                    }
                    else
                    {
                        result = await Api.CommentProcessor.ReplyCommentMediaAsync(Media.InstaIdentifier, 
                                 ReplyedComment.Pk.ToString(),
                                 $"@{ReplyedComment.User.UserName} {CommentText}",
                                 containerModule: containerModule,
                                 feedPosition: feedPosition,
                                 isCarouselBumpedPost: isCarouselBumpedPost,
                                 carouselIndex: carouselIndex);

                        if (!result.Succeeded)
                        {
                            MessageDialogHelper.Show(result.Info.Message);
                            return;
                        }
                        ReplyedComment.ChildComments.Insert(0, result.Value);
                    }

                    
                    CommentText = string.Empty;
                    if (lst != null)
                    {
                        await lst.SmoothScrollIntoViewWithItemAsync(result.Value);
                    }
                    ReplyedComment = null;
                }
            }
            finally { _isSendenabled = true; }
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is not MediaCommentsViewParameter parameter)
            {
                NavigationService.GoBack();
                return;
            }

            if (Media != null && Media.InstaIdentifier == parameter.Media.InstaIdentifier) return;

            Media = parameter.Media;

            Instance = new(Media.InstaIdentifier,
                           parameter.TargetCommentId);

            Comments = new(Instance);

            base.OnNavigatedTo(e);
        }

    }
}
