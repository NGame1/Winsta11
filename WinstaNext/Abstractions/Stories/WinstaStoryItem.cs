using InstagramApiSharp.Classes.Models;

namespace WinstaNext.Abstractions.Stories
{
    public class WinstaStoryItem
    {
        private WinstaStoryItem() { }

        public WinstaStoryItem(InstaBroadcast broadcast)
        {
            Broadcast = broadcast;
        }

        public WinstaStoryItem(InstaHashtagStory hashtagStory)
        {
            HashtagStory = hashtagStory;
        }

        public WinstaStoryItem(InstaReelFeed reelFeed)
        {
            ReelFeed = reelFeed;
        }

        public InstaBroadcast Broadcast { get; }
        public InstaHashtagStory HashtagStory { get; }
        public InstaReelFeed ReelFeed { get; }
    }
}
