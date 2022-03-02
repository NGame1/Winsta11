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
using InstagramApiSharp.API.RealTime.Responses.Wrappers;

namespace InstagramApiSharp.API.RealTime.Handlers
{
    internal class RealtimeDataContainer
    {
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("data")]
        public InstaRealtimeDataResponse[] Data { get; set; }
        public bool IsThreadTyping => !string.IsNullOrEmpty(Event);
    }
}
