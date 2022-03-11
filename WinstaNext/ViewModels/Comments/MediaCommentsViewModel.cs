using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Collections.IncrementalSources.Comments;
using WinstaNext.Core.Dialogs;
using WinstaNext.Core.Navigation;

namespace WinstaNext.ViewModels.Comments
{
    public class MediaCommentsViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.MediaComments;

        [AlsoNotifyFor(nameof(IsSendCommentButtonEnabled))]
        public string CommentText { get; set; } = "";

        bool _isSendenabled = false;
        public bool IsSendCommentButtonEnabled { get => CommentText.Length != 0 || _isSendenabled; }

        IncrementalMediaComments Instance { get; set; }
        public IncrementalLoadingCollection<IncrementalMediaComments, InstaComment> Comments { get; private set; }
        public InstaUserShort Me { get; }
        public InstaMedia Media { get; set; }

        public AsyncRelayCommand<ListView> AddCommentCommand { get; set; }

        public MediaCommentsViewModel()
        {
            Me = App.Container.GetService<InstaUserShort>();
            AddCommentCommand = new(AddCommentAsync);
        }

        async Task AddCommentAsync(ListView lst)
        {
            if (AddCommentCommand.IsRunning) return;
            if (!IsSendCommentButtonEnabled) return;
            try
            {
                InstaCommentContainerModuleType containerModule = InstaCommentContainerModuleType.ModalCommentComposerFeedTimeline;
                var Frame = NavigationService.Content;
                var isCarouselBumpedPost = Media.MediaType == InstaMediaType.Carousel;
                int? carouselIndex = null;
                if (isCarouselBumpedPost)
                    carouselIndex = 0;
                uint feedPosition = 0;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.CommentProcessor.CommentMediaAsync(Media.InstaIdentifier, CommentText,
                          containerModule: containerModule,
                          feedPosition: feedPosition,
                          isCarouselBumpedPost: isCarouselBumpedPost,
                          carouselIndex: carouselIndex);

                    if (!result.Succeeded)
                    {
                        MessageDialogHelper.Show(result.Info.Message);
                        return;
                    }
                    Comments.Insert(0, result.Value);
                    CommentText = string.Empty;
                    if (lst != null)
                    {
                        await lst.SmoothScrollIntoViewWithItemAsync(result.Value,
                                  itemPlacement: ScrollItemPlacement.Top);
                    }
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

            Media = parameter.Media;

            Instance = new(Media.InstaIdentifier,
                           parameter.TargetCommentId);

            Comments = new(Instance);

            base.OnNavigatedTo(e);
        }

    }
}
