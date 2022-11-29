using FFmpegInteropX;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Effects;
using Windows.Media.MediaProperties;
using Windows.Media.Transcoding;
using Windows.Storage;
using Windows.UI.Core;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaCore.Models.Core;

namespace ViewModels.Media.Upload
{
    public partial class VideoTranscoder : IDisposable
    {
        MediaEncodingProfile profile = null;
        MediaTranscoder transcoder = null;
        bool _istrimneeded = false;
        int _trimpart = 0;
        int trimstep = 60;
        StorageFile source;
        TimeSpan sourcevideoduration = TimeSpan.Zero;
        StorageFile destination;
        string _destinationfilename = string.Empty;
        public List<StorageFile> ResultFiles { get; } = new List<StorageFile>();

        public event EventHandler<double> ProgressValueChanged;

        public async Task TranscodeVideoAsync(StorageFile _sf, StorageFile _desf,  bool IsStory = false, int height = 0, int width = 0, List<KeyValuePair<string, PropertySet>> VideoEffects = null)
        {
            if (IsStory) trimstep = 15;
            source = _sf;
            destination = _desf;
            _destinationfilename = destination.DisplayName;
            var sourcevideoprops = await source.Properties.GetVideoPropertiesAsync();

            profile = MediaEncodingProfile.CreateMp4(VideoEncodingQuality.Wvga);
            profile.Video.Bitrate = sourcevideoprops.Bitrate;
            profile.Audio.Bitrate = 128;
            if (IsStory)
            {
                var r = AspectRatioHelper.FindNearestAspectRatioIGTVStory(sourcevideoprops.Height, sourcevideoprops.Width);
                var nGCD = AspectRatioHelper.GetGreatestCommonDivisor((int)r.Height, (int)r.Width);
                if (height == 0 && width == 0)
                {
                    profile.Video.Height = Convert.ToUInt32(r.Height);
                    profile.Video.Width = Convert.ToUInt32(r.Width);
                }
                else
                {
                    profile.Video.Height = Convert.ToUInt32(height);
                    profile.Video.Width = Convert.ToUInt32(width);
                }
            }
            else
            {
                if (height != 0 && width != 0)
                {
                    profile.Video.Height = Convert.ToUInt32(height);
                    profile.Video.Width = Convert.ToUInt32(width);
                }
            }
            //profile.Video.Height = sourcevideoprops.Height;
            //profile.Video.Width = sourcevideoprops.Width;
            transcoder = new MediaTranscoder();
            if (sourcevideoprops.Duration.TotalSeconds > trimstep)
            {
                //Trim
                sourcevideoduration = sourcevideoprops.Duration;
                _istrimneeded = true;
                var start = new TimeSpan(0, 0, 0);
                transcoder.TrimStartTime = start;
                transcoder.TrimStopTime = start.Add(new TimeSpan(0, 0, trimstep));
            }
            if (VideoEffects != null)
            {
                foreach (var item in VideoEffects)
                {
                    if (item.Value["Effect"].ToString().ToLower() != "crop")
                        transcoder.AddVideoEffect(item.Key, true, item.Value);
                    else
                    {
                        var Rect = (Rect)item.Value["Rect"];
                        Rect = new Rect((int)(Rect.X - 1), (int)(Rect.Y - 1), (int)(Rect.Width - 1), (int)(Rect.Height - 1));
                        var videoEffect = new VideoTransformEffectDefinition();
                        var activatableClassId = videoEffect.ActivatableClassId;
                        videoEffect.CropRectangle = Rect;
                        transcoder.AddVideoEffect(activatableClassId, true, videoEffect.Properties);

                    }
                }
            }
            try
            {
                var prepareOp = await transcoder.PrepareFileTranscodeAsync(source, destination, profile);
                if (prepareOp.CanTranscode)
                {
                    var transcodeOp = prepareOp.TranscodeAsync();
                    transcodeOp.Progress += new AsyncActionProgressHandler<double>(TranscodeProgress);
                    transcodeOp.Completed += new AsyncActionWithProgressCompletedHandler<double>(TranscodeComplete);
                    while (!_transcodecompleted)
                    {
                        await Task.Delay(500);
                    }
                }
                else
                {
                    switch (prepareOp.FailureReason)
                    {
                        case TranscodeFailureReason.CodecNotFound:
                            Debug.WriteLine("Codec not found.");
                            break;
                        case TranscodeFailureReason.InvalidProfile:
                            Debug.WriteLine("Invalid profile.");
                            break;
                        default:
                            Debug.WriteLine("Unknown failure.");
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        bool _transcodecompleted = false;
        private async void TranscodeComplete(IAsyncActionWithProgress<double> asyncInfo, AsyncStatus asyncStatus)
        {
            if (asyncStatus == AsyncStatus.Error)
            {
                try
                {
                    asyncInfo.GetResults();
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("0xC00D4A45") || ex.Message.Contains("0xC00D4A44"))
                    {
                        _transcodecompleted = true;
                        ResetValues();
                        return;
                    }
                    else
                    {
                        throw ex;
                    }
                }
            }
            asyncInfo.GetResults();
            if (asyncInfo.Status == AsyncStatus.Completed)
            {
                // Display or handle complete info.
                if (_istrimneeded)
                {
                    //Go to next part
                    _trimpart++;
                    if (sourcevideoduration.TotalSeconds > (_trimpart * trimstep))
                    {
                        ResultFiles.Add(destination);
                        var start = new TimeSpan(0, 0, (_trimpart * trimstep));
                        transcoder.TrimStartTime = start;
                        transcoder.TrimStopTime = start.Add(new TimeSpan(0, 0, trimstep));
                        destination = await (await destination.GetParentAsync()).CreateFileAsync(_destinationfilename + $"_part{(_trimpart + 1)}.mp4");
                        var prepareOp = await transcoder.PrepareFileTranscodeAsync(source, destination, profile);
                        if (prepareOp.CanTranscode)
                        {
                            var transcodeOp = prepareOp.TranscodeAsync();
                            transcodeOp.Progress += new AsyncActionProgressHandler<double>(TranscodeProgress);
                            transcodeOp.Completed += new AsyncActionWithProgressCompletedHandler<double>(TranscodeComplete);
                            return;
                        }
                        else
                        {
                            switch (prepareOp.FailureReason)
                            {
                                case TranscodeFailureReason.CodecNotFound:
                                    Debug.WriteLine("Codec not found.");
                                    break;
                                case TranscodeFailureReason.InvalidProfile:
                                    Debug.WriteLine("Invalid profile.");
                                    break;
                                default:
                                    Debug.WriteLine("Unknown failure.");
                                    break;
                            }
                        }
                    }
                    else
                    {
                        ResultFiles.Add(destination);
                        _transcodecompleted = true;
                    }
                }
                else
                {
                    ResultFiles.Add(destination);
                    _transcodecompleted = true;
                }
            }
            else if (asyncInfo.Status == AsyncStatus.Canceled)
            {
                ResetValues();
                throw new Exception("Canceled");
                // Display or handle cancel info.
            }
            else
            {
                // Display or handle error info.
                //await MessageBox.Show(asyncInfo.Status + "\t" + asyncInfo.ErrorCode);
                ResetValues();
                throw new Exception(asyncInfo.Status + "\t" + asyncInfo.ErrorCode);
            }
        }

        private void TranscodeProgress(IAsyncActionWithProgress<double> asyncInfo, double progressInfo)
        {
            ProgressValueChanged?.Invoke(this, progressInfo);
            //try
            //{
            //    if(asyncInfo.Status == AsyncStatus.Error)
            //    {
            //
            //    }
            //}
            //catch (Exception ex)
            //{
            //
            //}

            //await AppCore.Dispatcher.RunAsync(CoreDispatcherPriority.Low, delegate
            //{
            //    //Prog.Value = progressInfo;
            //    //MediaEl.AddAudioEffect("", true, new PropertySet() { new KeyValuePair<string, object>("", "") });
            //});
        }

        void ResetValues()
        {
            profile = null;
            transcoder = null;
            _istrimneeded = false;
            _trimpart = 0;
            trimstep = 15;
            source = null;
            sourcevideoduration = TimeSpan.Zero;
            destination = null;
            _destinationfilename = string.Empty;
        }

        public void Dispose()
        {
            ResetValues();
        }
    }
}
