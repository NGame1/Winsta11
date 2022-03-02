using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Collections;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Core.Navigation
{
    public class IncrementalMediaViewParameter
    {
        public IncrementalMediaViewParameter(ISupportIncrementalLoading source, InstaMedia target)
        {
            MediaSource = source;
            TargetMedia = target;
            //TargetIndex = index;
        }

        public ISupportIncrementalLoading MediaSource { get; }
        //public int TargetIndex { get; }
        public InstaMedia TargetMedia { get; }
    }
}
