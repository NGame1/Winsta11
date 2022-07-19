using InstagramApiSharp.Classes.Models;
using WinstaCore.Attributes;

namespace Abstractions.Navigation
{
    public class IncrementalMediaViewParameter
    {
        public IncrementalMediaViewParameter(RangePlayerAttribute source, InstaMedia target)
        {
            MediaSource = source;
            TargetMedia = target;
            //TargetIndex = index;
        }

        public void SetTargetMedia(InstaMedia media)
        {
            TargetMedia = media;
        }

        public RangePlayerAttribute MediaSource { get; }
        //public int TargetIndex { get; }
        public InstaMedia TargetMedia { get; private set; }
    }
}
