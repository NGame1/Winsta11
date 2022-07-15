using Microsoft.Toolkit.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Abstractions.Stories;

namespace Core.Collections.IncrementalSources.Stories
{
    public class IncrementalDummyStories : IIncrementalSource<WinstaStoryItem>
    {
        public Task<IEnumerable<WinstaStoryItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            return null;
        }
    }
}
