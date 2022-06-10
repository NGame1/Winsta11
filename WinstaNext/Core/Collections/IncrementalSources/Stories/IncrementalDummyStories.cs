using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WinstaNext.Abstractions.Stories;

namespace WinstaNext.Core.Collections.IncrementalSources.Stories
{
    internal class IncrementalDummyStories : IIncrementalSource<WinstaStoryItem>
    {
        public Task<IEnumerable<WinstaStoryItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return null;
        }
    }
}
