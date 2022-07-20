using InstagramApiSharp.Classes;
using System;

namespace WinstaCore.Models.ConfigureDelays
{
    public class VideoConfigureMediaDelay : IConfigureMediaDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(10);
    }
}
