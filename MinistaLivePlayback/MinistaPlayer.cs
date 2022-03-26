﻿using System;
using System.Threading.Tasks;
using MinistaLivePlayback.Models;
using Windows.UI.Xaml.Controls;
using Windows.Media.Core;
using MinistaLivePlayback.Util;
using Windows.Media.Streaming.Adaptive;
using Windows.Data.Xml.Dom;
using Windows.Storage;

namespace MinistaLivePlayback
{
    public class MinistaPlayer : ILivePlayer
    {
        public MediaElement MediaPlayer { get; private set; }
        Player player;
        private bool haveSetLiveOffset;
        private TimeSpan liveOffset;

        /// <summary>
        /// Sets the offset of where to start playback from the 
        /// most recent available segment.
        /// </summary>
        /// <param name="offset">the offset duration, in hns</param>
        public void DesiredLiveOffset(TimeSpan offset)
        {
            haveSetLiveOffset = true;
            liveOffset = offset;
        }

        /// <summary>
        /// Goes to the live edge. This is the most recent available
        /// segment minus the desired live offset
        /// </summary>
        public void GoToLive()
        {
            if (player != null)
            {
                if (haveSetLiveOffset)
                {
                    player.HasLiveOffsetValue = true;
                    player.LiveOffset = liveOffset;
                }
                player.GoToLiveEdge();
            }
        }

        /// <summary>
        /// Downloads the manifest from the given source, parses the manifest
        /// and sets the source on the media element 
        /// </summary>
        /// <param name="source">The URL of the source MPD</param>
        /// <param name="mediaElement">The MediaElement to start playback</param>
        /// <returns></returns>
        public async Task Initialize(Uri source, MediaElement mediaElement)
        {
            //1) Download manifest
            var sourceUrlText = source.AbsoluteUri;
            try
            {
                MediaPlayer = mediaElement;
                var manifest = new Manifest(sourceUrlText);
                var document = await manifest.LoadManifestAsync(sourceUrlText);

                //2) Parse manifest
                DashManifestParser mpdParser = new DashManifestParser(document, ref manifest);
                if (mpdParser.Parse())
                {

                    if (!manifest.IsSupportedProfile)
                    {
#if DEBUG
                        Logger.Log("The profiles attribute does not contain the \"urn:mpeg:dash:profile:isoff-live:2011\" profile, so it may not work as expected.");
#endif
                    }
                    if (manifest.IsLive)
                    {
                        //3) Play using MSE if it is live
                        MseStreamSource mseSource = new MseStreamSource();
                        player = new Player(mediaElement, mseSource, manifest);
                        if (haveSetLiveOffset && manifest.IsLive)
                        {
                            player.HasLiveOffsetValue = true;
                            player.LiveOffset = liveOffset;
                        }
                        player.Initialize();
                    }
                    else
                    {
                        // Otherwise, use our Adaptive Media Source for on demand content
                        var result = await AdaptiveMediaSource.CreateFromUriAsync(source);
                        if (result.Status != AdaptiveMediaSourceCreationStatus.Success)
                        {
                            throw new Exception("Unable to create media source because: " + result.Status);
                        }
                        var adaptiveSource = result.MediaSource;

                        mediaElement.SetMediaStreamSource(adaptiveSource);
                        mediaElement.Play();
                    }


                }

                else
                {
#if DEBUG
                    Logger.Log("The Parser failed to parse this mpd");
#endif
                    return;
                }
            }
            catch (Exception e)
            {
#if DEBUG
                Logger.Log("Exception when initializing player: " + e.Message + " " + Logger.Display(e));
#endif
            }
        }

        /// <summary>
        /// Stops the Timer
        /// </summary>
        public void Close()
        {
            if (player != null)
            {
                player.Close();
                player = null;
            }
        }
    }
}
