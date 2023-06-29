using InstagramApiSharp;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
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
using WinstaCore;
using WinstaCore.Attributes;
using WinstaCore.Interfaces.Views;
using WinstaCore.Services;

namespace Core.Collections.IncrementalSources.Media
{
    public class IncrementalHomeMedia : IIncrementalSource<InstaMedia>
    {
        [OnChangedMethod(nameof(OnRefreshRequestedChanged))]
        public bool RefreshRequested { get; set; } = false;

        public bool IsDark { get; set; } = false;

        PaginationParameters Pagination { get; set; }

        public IncrementalHomeMedia(bool isDark)
        {
            IsDark = isDark;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
        }

        bool nomoreitems = false;
        public async Task<IEnumerable<InstaMedia>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default)
        {
            if (nomoreitems) return null;
            var report = Battery.AggregateBattery.GetReport();
            var charge = report.RemainingCapacityInMilliwattHours / report.FullChargeCapacityInMilliwattHours * 100;
            var isCharging = report.Status == BatteryStatus.Charging;
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                var result = await Api.FeedProcessor.GetUserTimelineFeedAsync(Pagination,
                    removeAds: ApplicationSettingsManager.Instance.GetRemoveFeedAds(),
                    batteryLevel: charge.HasValue ? (ushort)charge : (ushort)100,
                    cancellationToken: cancellationToken,
                    seenMediaIds: GetSeenMediaIds(),
                    refreshRequest: RefreshRequested,
                    isCharging: isCharging,
                    isDarkMode: IsDark,
                    willSoundOn: true);
                if (!result.Succeeded && result.Info.Exception is not TaskCanceledException)
                    throw result.Info.Exception;
                if (!result.Value.MoreAvailable) nomoreitems = true;
                RefreshRequested = false;
                return result.Value.Medias;
            }
        }

        public string[] GetSeenMediaIds()
        {
            var Nav = AppCore.Container.GetService<NavigationService>();
            if (Nav.Content is not IHomeView home)
                return null;
            var src = home.Medias;
            var playingItem = src.Where(x => x.Play);
            if (!playingItem.Any()) return null;
            var seenMedias = src.IndexOf(playingItem.FirstOrDefault());
            var seenarr = src.Take(seenMedias);
            var mediaids = seenarr.Select(x => x.InstaIdentifier).ToArray();
            return mediaids;
        }

        void OnRefreshRequestedChanged()
        {
            if (!RefreshRequested) return;
            Pagination = PaginationParameters.MaxPagesToLoad(1);
            RefreshRequested = true;
        }

    }
}
