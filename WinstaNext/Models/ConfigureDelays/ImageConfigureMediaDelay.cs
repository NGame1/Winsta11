using InstagramApiSharp.Classes;
using System;

namespace WinstaNext.Models.ConfigureDelays
{
    internal class ImageConfigureMediaDelay : IConfigureMediaDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(1);
    }
}
