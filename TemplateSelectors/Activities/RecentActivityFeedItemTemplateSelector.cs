using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors;

public class RecentActivityFeedItemTemplateSelector : DataTemplateSelector
{
    public DataTemplate CommentLikeTemplate { get; set; }
    public DataTemplate FollowTemplate { get; set; }
    public DataTemplate RequestedToFollowYouTemplate { get; set; }
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
        return activity.Type switch
        {
            InstaActivityFeedType.CommentLike => CommentLikeTemplate,
            InstaActivityFeedType.Follow => FollowTemplate,
            InstaActivityFeedType.StoryLike => StoryLikeTemplate,
            InstaActivityFeedType.RequestedToFollowYou => RequestedToFollowYouTemplate,
            InstaActivityFeedType.FriendRequest => FriendRequestTemplate,
            InstaActivityFeedType.LikedTagged => LikedTaggedTemplate,
            InstaActivityFeedType.LoginActivity => LoginActivityTemplate,
            InstaActivityFeedType.SharedPost => SharedPostTemplate,
            InstaActivityFeedType.TaggedOrLikedOrCommentedYou => TaggedOrLikedOrCommentedYouTemplate,
            InstaActivityFeedType.Unknown => UnknownTemplate,
            _ => UnknownTemplate,
        };
    }
}
