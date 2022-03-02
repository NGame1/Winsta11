using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using System;
using System.Collections.Generic;

// ReSharper disable StringLiteralTypo

namespace WinstaBackgroundHelpers.Push
{
    public sealed class FbnsUserAgent
    {
        const string FBNS_APPLICATION_NAME = "MQTT";
#pragma warning disable IDE0051 // Remove unused private members
        const string INSTAGRAM_APPLICATION_NAME = "Instagram";  // for Realtime features
#pragma warning restore IDE0051 // Remove unused private members

        // todo: implement Realtime status like "message seen"
        public static string BuildFbUserAgent(IInstaApi insta, string appName = FBNS_APPLICATION_NAME, string userLocale = "en_US")
        {
            var device = insta.GetCurrentDevice();
            var dpi = int.Parse(device.Dpi.Replace("dpi", ""));
            var res = device.Resolution.Split('x');
            var width = res[0];
            var height = res[1];
            var fields = new Dictionary<string, string>
            {
                {"FBAN", appName},
                {"FBAV", insta.GetApiVersionInfo().AppVersion},
                {"FBBV",  insta.GetApiVersionInfo().AppApiVersionCode},
                {"FBDM",
                    $"{{density={Math.Round(dpi/ 160f, 1):F1},width={width},height={height}}}"
                },
                {"FBLC", userLocale},
                {"FBCR", ""},   // We don't have cellular
                {"FBMF", device.HardwareManufacturer},
                {"FBBD", device.HardwareManufacturer},
                {"FBPN", InstaApiConstants.INSTAGRAM_PACKAGE_NAME},
                {"FBDV", device.HardwareModel},
                {"FBSV", device.AndroidVer.VersionNumber},
                {"FBLR", "0"},  // android.hardware.ram.low
                {"FBBK", "1"},  // Const (at least in 10.12.0)
                {"FBCA", AndroidDevice.CPU_ABI}
            };
            var mergeList = new List<string>();
            foreach (var field in fields)
            {
                mergeList.Add($"{field.Key}/{field.Value}");
            }

            var userAgent = "";
            foreach (var field in mergeList)
            {
                userAgent += field + ';';
            }

            return '[' + userAgent + ']';
        }
        public static string BuildRTUserAgent(IInstaApi insta)
        {
            var device = insta.GetCurrentDevice();
            var dpi = int.Parse(device.Dpi.Replace("dpi", ""));
            var res = device.Resolution.Split('x');
            var width = res[0];
            var height = res[1];


            var dic = new Dictionary<string, string>
            {
                {"FBAN", "MQTT"},
                {"FBAV", insta.GetApiVersionInfo().AppVersion},
                {"FBBV", "567310203415052" /*insta.GetApiVersionInfo().AppApiVersionCode*/},
                {"FBDM",
                    $"{{density={Math.Round(dpi/ 160f, 1):F1},width={width},height={height}}}"
                },
                {"FBLC", InstaApiConstants.ACCEPT_LANGUAGE.Replace("-","_")},
                {"FBCR", ""},   // We don't have cellular
                {"FBMF", device.HardwareManufacturer},
                {"FBBD", device.HardwareManufacturer},
                {"FBPN", InstaApiConstants.INSTAGRAM_PACKAGE_NAME},
                {"FBDV", device.HardwareModel.Replace(" ", "")},
                {"FBSV", device.AndroidVer.VersionNumber},
                {"FBLR", "0"},  // android.hardware.ram.low
                {"FBBK", "1"},  // Const (at least in 10.12.0)
                {"FBCA", AndroidDevice.CPU_ABI}
            };
            var mergeList = new List<string>();
            foreach (var field in dic)
            {
                mergeList.Add($"{field.Key}/{field.Value}");
            }

            var userAgent = "";
            foreach (var field in mergeList)
            {
                userAgent += field + ';';
            }

            return '[' + userAgent + ']';
        }

    }
}
