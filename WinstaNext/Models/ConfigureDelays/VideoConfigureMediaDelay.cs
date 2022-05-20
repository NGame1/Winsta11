using InstagramApiSharp.Classes;
using System;

namespace WinstaNext.Models.ConfigureDelays
{
    internal class VideoConfigureMediaDelay : IConfigureMediaDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(10);
    }
}
