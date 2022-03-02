/*
 * Created by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2021 Winter [ Zemestan 1399 ]
 */
using Newtonsoft.Json;
namespace InstagramApiSharp.API.RealTime.Responses.Wrappers
{
    public class InstaRealtimeRespondResponse
    {
        [JsonProperty("publish_metadata")]
        public InstaRealtimePublishMetadataResponse PublishMetadata { get; set; }
        [JsonProperty("lazy")]
        public bool? Lazy { get; set; }
        [JsonProperty("data")]
        public InstaRealtimeDataResponse[] Data { get; set; }
        [JsonProperty("event")]
        public string Event { get; set; }
        [JsonProperty("num_endpoints")]
        public int NumEndpoints { get; set; }
    }
    public class InstaRealtimePublishMetadataResponse
    {
        [JsonProperty("publish_time_ms")]
        public string PublishTimeMS { get; set; }
        [JsonProperty("topic_publish_id")]
        public long? TopicPublishId { get; set; }
    }
    public class InstaRealtimeDataResponse
    {
        [JsonProperty("doublePublish")]
        public bool? DoublePublish { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("path")]
        public string Path { get; set; }
        [JsonProperty("op")]
        public string Op { get; set; }

        public bool IsTyping => Path?.Contains("/activity_indicator_id/") ?? false;
        public bool IsThreadItem => Path?.Contains("/items/") ?? false;
        public bool IsThreadParticipants => Path?.Contains("/participants/") ?? false;
        public bool IsBroadcast => Path?.Contains("/broadcast/") ?? false;
        public bool HasItemInValue => Value?.Contains("{") ?? false;

    }














    public class InstaRealtimeTypingResponse
    {
        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }
        [JsonProperty("sender_id")]
        public string SenderId { get; set; }
        [JsonProperty("ttl")]
        public int Ttl { get; set; }
        [JsonProperty("activity_status")]
        public int ActivityStatus { get; set; }
    } 














}
