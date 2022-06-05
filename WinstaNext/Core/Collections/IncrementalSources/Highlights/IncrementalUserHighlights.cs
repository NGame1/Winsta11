﻿using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using PropertyChanged;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Power;
using Windows.System.Power;
using Windows.UI.Xaml;
using WinstaNext.Abstractions.Stories;

namespace WinstaNext.Core.Collections.IncrementalSources.Highlights
{
    public class IncrementalUserHighlights : IIncrementalSource<WinstaStoryItem>
    {
        PaginationParameters Pagination { get; set; }

        public long UserId { get; private set; } = -1;

        [OnChangedMethod(nameof(OnRefreshRequestedChanged))]
        public bool RefreshRequested { get; set; }

        public IncrementalUserHighlights(long userId)
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            UserId = userId;
        }

        bool HasMoreAvailable = true;
        public async Task<IEnumerable<WinstaStoryItem>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (!HasMoreAvailable) return null;
            IResult<InstaHighlightFeeds> result;
            try
            {
                var report = Battery.AggregateBattery.GetReport();
                var charge = report.RemainingCapacityInMilliwattHours / report.FullChargeCapacityInMilliwattHours * 100;
                var isCharging = report.Status == BatteryStatus.Charging;
                var isDark = new ThemeListener().CurrentTheme == ApplicationTheme.Dark;
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    result = await Api.StoryProcessor.GetHighlightFeedsAsync(UserId,
                             batteryLevel: charge.HasValue ? (ushort)charge : (ushort)100,
                             isCharging: isCharging,
                             isDarkMode: isDark,
                             willSoundOn: true);
                }

                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                List<WinstaStoryItem> highlights = new();
                HasMoreAvailable = false;
                for (int i = 0; i < result.Value.Items.Count; i++)
                {
                    highlights.Add(new(result.Value.Items.ElementAt(i)));
                }
                return highlights;
            }
            finally { }
        }

        void OnRefreshRequestedChanged()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            RefreshRequested = true;
        }
    }
}