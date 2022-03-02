/*
 * Created by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2020 Summer [ Tabestaan 1399 ]
 */
using System;
using System.Collections.Generic;

namespace InstagramApiSharp.API.RealTime.Responses.Models
{
    public class InstaRealtimeRespond
    {
        public InstaRealtimePublishMetadata PublishMetadata { get; set; }
        public bool Lazy { get; set; }
        public List<InstaRealtimeData> Data { get; set; } = new List<InstaRealtimeData>();
        public string Event { get; set; }
        public int NumEndpoints { get; set; }
    }
    public class InstaRealtimePublishMetadata
    {
        public string PublishTimeMS { get; set; }
        public long TopicPublishId { get; set; }
    }
    public class InstaRealtimeData
    {
        public bool DoublePublish { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
        public string Op { get; set; }
    }



    public class InstaRealtimeTypingEventArgs : EventArgs
    {
        public string RealtimePath { get; set; }
        public string RealtimeOp { get; set; } = "add";
        public long TimestampUnix { get; set; }
        public DateTime Timestamp { get; set; }
        public string SenderId { get; set; }
        public int Ttl { get; set; }
        public int ActivityStatus { get; set; }
    }
}
