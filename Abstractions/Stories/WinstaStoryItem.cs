using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System.ComponentModel;
#nullable enable

namespace Abstractions.Stories
{
    [AddINotifyPropertyChangedInterface]
    public class WinstaStoryItem : INotifyPropertyChanged
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

        public WinstaStoryItem(InstaHighlightFeed highlight)
        {
            HighlightStory = new(highlight);
        }

        public WinstaStoryItem(InstaReelFeed reelFeed)
        {
            ReelFeed = new(reelFeed);
        }

        public InstaBroadcast? Broadcast { get; }
        public InstaHashtagStory? HashtagStory { get; }
        public WinstaHighlightFeed? HighlightStory { get; }
        public WinstaReelFeed? ReelFeed { get; }

        public bool IsSelected { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}
