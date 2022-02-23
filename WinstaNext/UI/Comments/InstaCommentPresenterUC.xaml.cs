using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Comments
{
    public sealed partial class InstaCommentPresenterUC : UserControl
    {
        public static readonly DependencyProperty CommentProperty = DependencyProperty.Register(
          "Comment",
          typeof(InstaComment),
          typeof(InstaCommentPresenterUC),
          new PropertyMetadata(null));

        public InstaComment Comment
        {
            get { return (InstaComment)GetValue(CommentProperty); }
            set { SetValue(CommentProperty, value); }
        }

        public AsyncRelayCommand LikeCommecntCommand { get; set; }

        public InstaCommentPresenterUC()
        {
            LikeCommecntCommand = new(LikeCommentAsync);
            this.InitializeComponent();
        }

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

    }
}
