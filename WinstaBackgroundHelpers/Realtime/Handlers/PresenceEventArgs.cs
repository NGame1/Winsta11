/*
 * Created by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2020 Summer [ Tabestaan 1399 ]
 */

using System;
using Newtonsoft.Json;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.Classes.Models;

namespace InstagramApiSharp.API.RealTime.Handlers
{
    internal class PresenceContainer
    {
        [JsonProperty("presence_event")]
        public PresenceEventEventArgs PresenceEvent { get; set; }
        internal bool IsPresence => PresenceEvent != null;
    }

    public class PresenceEventEventArgs : EventArgs
    {
        private string _lastActivityAtMs;
        [JsonProperty("user_id")]
        public string UserId { get; set; }
        [JsonProperty("is_active")]
        public bool IsActive { get; set; }
        [JsonProperty("last_activity_at_ms")]
        public string LastActivityAtMs
        {
            get => _lastActivityAtMs;
            set
            {
                LastActivityAt = DateTimeHelper.FromUnixTimeMiliSeconds(long.Parse(value));
                _lastActivityAtMs = value;
            }
        }
        [JsonIgnore()]
        public DateTime LastActivityAt { get; set; }
        [JsonProperty("in_threads")]
        public object InThreads { get; set; }
    }

    public class InstaBroadcastEventArgs : EventArgs
    {
        [JsonProperty("broadcast_id")]
        public string BroadcastId { get; set; }
        [JsonProperty("compound_media_id")]
        public string CompoundMediaId { get; set; }
        [JsonProperty("published_time")]
        public long? PublishedTime { get; set; }
        [JsonProperty("is_periodic")]
        public bool? IsPeriodic { get; set; }
        [JsonProperty("broadcast_message")]
        public string BroadcastMessage { get; set; }
        [JsonProperty("display_notification")]
        public bool? DisplayNotification { get; set; }
        [JsonProperty("copyright_violation")]
        public bool? CopyrightViolation { get; set; }
        [JsonProperty("event_type")]
        public string EventType { get; set; }
        [JsonProperty("add_to_home_tray")]
        public bool? AddToHomeTray { get; set; }
        [JsonProperty("status")]
        public string BroadcastStatus { get; set; }
        [JsonProperty("user")]
        internal InstaUserShortResponse UserInternal { get; set; }
        public InstaUserShort User => Converters.ConvertersFabric.Instance.GetUserShortConverter(UserInternal).Convert();
    }


}
