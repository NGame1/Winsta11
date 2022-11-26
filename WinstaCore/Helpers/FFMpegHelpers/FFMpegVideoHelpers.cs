using FFmpegInteropX;

namespace WinstaCore.Helpers.FFMpegHelpers
{
    public static class FFMpegVideoHelpers
    {
        /// <summary>
        /// Crop the input video to given dimensions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width">The width of the output video. It defaults to iw. This expression is evaluated only once during the filter configuration, or when the ‘w’ or ‘out_w’ command is sent.</param>
        /// <param name="height">The height of the output video. It defaults to ih. This expression is evaluated only once during the filter configuration, or when the ‘h’ or ‘out_h’ command is sent.</param>
        /// <param name="x">The horizontal position, in the input video, of the left edge of the output video. It defaults to (in_w-out_w)/2. This expression is evaluated per-frame.</param>
        /// <param name="y">The vertical position, in the input video, of the top edge of the output video. It defaults to (in_h-out_h)/2. This expression is evaluated per-frame.</param>
        public static void CropVideo(this FFmpegMediaSource source, int width, int height, int x, int y)
        {
            source.SetFFmpegVideoFilters($"crop=w={width}:h={height}:x={x}:y={y}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="planes"></param>
        public static void Blur(this FFmpegMediaSource source, int sizeX = 0, int sizeY = 0, int planes = 15)
        {
            source.SetFFmpegVideoFilters($"avgblur=sizeX={sizeX}:sizeY={sizeY}:planes={planes}");
        }
    }
}
