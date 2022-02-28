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
        }

        public ISupportIncrementalLoading MediaSource { get; }
        public InstaMedia TargetMedia { get; }
    }
}
