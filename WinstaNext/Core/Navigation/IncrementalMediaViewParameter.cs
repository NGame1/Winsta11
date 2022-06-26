using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using Windows.UI.Xaml.Data;
using WinstaNext.Core.Attributes;

namespace WinstaNext.Core.Navigation
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
