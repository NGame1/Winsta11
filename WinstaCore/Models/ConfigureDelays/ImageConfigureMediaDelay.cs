using InstagramApiSharp.Classes;
using System;

namespace WinstaCore.Models.ConfigureDelays
{
    public class ImageConfigureMediaDelay : IConfigureMediaDelay
    {
        public TimeSpan Value => TimeSpan.FromSeconds(1);
    }
}
