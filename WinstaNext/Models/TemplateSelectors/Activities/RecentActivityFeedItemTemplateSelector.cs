using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.Models.TemplateSelectors.Activities
{
    internal class RecentActivityFeedItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CommentLikeTemplate { get; set; }
        public DataTemplate FollowTemplate { get; set; }
        public DataTemplate FriendRequestTemplate { get; set; }
        public DataTemplate LikedTaggedTemplate { get; set; }
        public DataTemplate LoginActivityTemplate { get; set; }
        public DataTemplate SharedPostTemplate { get; set; }
        public DataTemplate StoryLikeTemplate { get; set; }
        public DataTemplate TaggedOrLikedOrCommentedYouTemplate { get; set; }
        public DataTemplate UnknownTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not InstaRecentActivityFeed activity) return null;
            switch (activity.Type)
            {
                case InstaActivityFeedType.CommentLike: return CommentLikeTemplate;
                case InstaActivityFeedType.Follow: return FollowTemplate;
                case InstaActivityFeedType.StoryLike: return StoryLikeTemplate;
                case InstaActivityFeedType.FriendRequest: return FriendRequestTemplate;
                case InstaActivityFeedType.LikedTagged: return LikedTaggedTemplate;
                case InstaActivityFeedType.LoginActivity: return LoginActivityTemplate;
                case InstaActivityFeedType.SharedPost: return SharedPostTemplate;
                case InstaActivityFeedType.TaggedOrLikedOrCommentedYou: return TaggedOrLikedOrCommentedYouTemplate;
                case InstaActivityFeedType.Unknown: return UnknownTemplate;
                default: return UnknownTemplate;
            }
        }
    }
}
