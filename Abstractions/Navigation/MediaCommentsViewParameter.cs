using InstagramApiSharp.Classes.Models;

namespace Abstractions.Navigation
{
    public class MediaCommentsViewParameter
    {
        public MediaCommentsViewParameter(InstaMedia media, string targetCommentId = "")
        {
            TargetCommentId = targetCommentId;
            Media = media;
        }

        public InstaMedia Media { get; }
        public string TargetCommentId { get; }
    }
}
