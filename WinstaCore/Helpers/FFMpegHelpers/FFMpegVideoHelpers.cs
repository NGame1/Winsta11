using FFmpegInteropX;
using Windows.Foundation;

namespace WinstaCore.Helpers.FFMpegHelpers
{
    public static class FFMpegVideoHelpers
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sizeX"></param>
        /// <param name="sizeY"></param>
        /// <param name="planes"></param>
        public static void AverageBlur(this FFmpegMediaSource source, int sizeX = 0, int sizeY = 0, int planes = 15)
        {
            source.SetFFmpegVideoFilters($"avgblur=sizeX={sizeX}:sizeY={sizeY}:planes={planes}");
        }

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
        /// Crop the input video to given dimensions.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="r">The cropping rect</param>
        public static void CropVideo(this FFmpegMediaSource source, Rect r)
        {
            source.SetFFmpegVideoFilters($"crop=w={r.Width}:h={r.Height}:x={r.X}:y={r.Y}");
        }

        /// <summary>
        /// Sets the fast motion effect on video by settings PTS
        /// </summary>
        /// <param name="source"></param>
        public static void FastMotion(this FFmpegMediaSource source)
        {
            SetPresentationTimestamp(source, 0.5);
        }

        /// <summary>
        /// Convert the video to specified constant frame rate by duplicating or dropping frames as necessary
        /// </summary>
        /// <param name="source"></param>
        /// <param name="fps">The desired output frame rate. The default is 25.</param>
        public static void FramePerSecond(this FFmpegMediaSource source, int fps = 25)
        {
            source.SetFFmpegVideoFilters($"fps={fps}");
        }

        /// <summary>
        /// Convert source to grayscale
        /// </summary>
        /// <param name="source"></param>
        public static void GrayScale(this FFmpegMediaSource source)
        {
            source.SetFFmpegVideoFilters($"colorchannelmixer=.3:.4:.3:0:.3:.4:.3:0:.3:.4:.3");
        }

        /// <summary>
        /// Apply Gaussian blur filter
        /// </summary>
        /// <param name="source"></param>
        /// <param name="sigma">Set horizontal sigma, standard deviation of Gaussian blur. Default is 0.5</param>
        /// <param name="steps">Set number of steps for Gaussian approximation. Default is 1</param>
        /// <param name="sigmaV">Set which planes to filter. By default all planes are filtered</param>
        /// <param name="planes">Set vertical sigma, if negative it will be same as sigma. Default is -1</param>
        public static void GaussianBlur(this FFmpegMediaSource source,
            double sigma = 0.5, double steps = 1, double sigmaV = -1, string planes = "")
        {
            source.SetFFmpegVideoFilters($"gblur=sigma={sigma}:steps={steps}:sigmaV={sigmaV}");
        }

        /// <summary>
        /// Scale (resize) the input video, using the libswscale library
        /// </summary>
        /// <param name="source"></param>
        /// <param name="width">Set the output video dimension expression.</param>
        /// <param name="height">Set the output video dimension expression.</param>
        public static void Scale(this FFmpegMediaSource source,
            int width, int height)
        {
            source.SetFFmpegVideoFilters($"scale=w={width}:h={height}");
        }

        /// <summary>
        /// Change the PTS (presentation timestamp) of the input frames
        /// </summary>
        /// <param name="source"></param>
        /// <param name="PtsFactor">default is 1.0</param>
        public static void SetPresentationTimestamp(this FFmpegMediaSource source, double PtsFactor = 1.0)
        {
            source.SetFFmpegVideoFilters($"setpts={PtsFactor}*PTS");
        }

        /// <summary>
        /// Sets the slow motion effect on video by settings PTS
        /// </summary>
        /// <param name="source"></param>
        public static void SlowMotion(this FFmpegMediaSource source)
        {
            SetPresentationTimestamp(source, 2.0);
        }
    }
}
