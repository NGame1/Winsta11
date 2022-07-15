using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.UI.Xaml.Data;

namespace WinstaCore.Attributes
{
    public class RangePlayerAttribute : IncrementalLoadingCollection<IIncrementalSource<InstaMedia>, InstaMedia>, IItemsRangeInfo
    {
        public RangePlayerAttribute(IIncrementalSource<InstaMedia> source) : base(source)
        {

        }

        public int FirstVisibleItemIndex { get; private set; } = -1;
        //int LastVisibleItemIndex { get; set; } = -1;
        //public event EventHandler<int> FirstVisibleItemIndexChanged;
        //public event EventHandler<int> LastVisibleItemIndexChanged;

        public void RangesChanged(ItemIndexRange visibleRange, IReadOnlyList<ItemIndexRange> trackedItems)
        {
            if (visibleRange.FirstIndex == FirstVisibleItemIndex) return;
            //var _tempfirst = FirstVisibleItemIndex;
            //var _templast = LastVisibleItemIndex;
            //FirstVisibleItemIndexChanged?.Invoke(this, visibleRange.FirstIndex);
            Medias_FirstVisibleItemIndexChanged(this, visibleRange.FirstIndex);
            FirstVisibleItemIndex = visibleRange.FirstIndex;
            //LastVisibleItemIndexChanged?.Invoke(this, visibleRange.LastIndex);
            //LastVisibleItemIndex = visibleRange.LastIndex;
        }

        public void Dispose()
        {

        }

        private void Medias_FirstVisibleItemIndexChanged(object sender, int e)
        {
            var playingItems = Items.Where(x => x.Play);
            if (playingItems.Any())
            {
                for (int i = 0; i < playingItems.Count(); i++)
                {
                    playingItems.ElementAt(i).Play = false;
                }
            }
            Items.ElementAt(e).Play = true;
        }
    }
}
