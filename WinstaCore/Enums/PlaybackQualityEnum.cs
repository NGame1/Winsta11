using Resources;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WinstaNext.Converters;

namespace WinstaCore.Enums
{
    public enum PlaybackQualityEnum
    {
        Low = 2,
        [Display(ResourceType = typeof(SettingsStrings), Name = nameof(SettingsStrings.Medium))]
        Medium = 1,
        [Display(ResourceType = typeof(SettingsStrings), Name = nameof(SettingsStrings.High))]
        High = 0
    }
}
