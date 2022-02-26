using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Collections;
using Microsoft.Toolkit.Uwp.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace WinstaNext.Core.Collections.IncrementalSources.Media
{
    public class IncrementalHomeMedia : IIncrementalSource<InstaMedia>
    {
        public bool RefreshRequested { get; set; } = false;

        PaginationParameters Pagination { get; set; }

        public IncrementalHomeMedia()
        {
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            var report = Windows.Devices.Power.Battery.AggregateBattery.GetReport();
            var charge = report.RemainingCapacityInMilliwattHours / report.FullChargeCapacityInMilliwattHours * 100;
            var isCharging = report.Status == Windows.System.Power.BatteryStatus.Charging;
            var isDark = new ThemeListener().CurrentTheme == ApplicationTheme.Dark;
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.FeedProcessor.GetUserTimelineFeedAsync(Pagination,
                    removeAds: ApplicationSettingsManager.Instance.GetRemoveFeedAds(),
                    batteryLevel: charge.HasValue ? (ushort)charge : (ushort)100,
                    cancellationToken: cancellationToken,
                    refreshRequest: RefreshRequested,
                    isCharging: isCharging,
                    isDarkMode: isDark,
                    willSoundOn: true);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                if (!result.Value.MoreAvailable) nomoreitems = true;
                RefreshRequested = false;
                return result.Value.Medias;
            }
        }

    }
}
