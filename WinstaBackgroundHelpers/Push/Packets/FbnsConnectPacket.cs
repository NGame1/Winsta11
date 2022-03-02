﻿using Windows.Storage.Streams;
using WinstaBackgroundHelpers.Mqtt.Packets;

namespace WinstaBackgroundHelpers.Push.Packets
{
    public sealed class FbnsConnectPacket : Packet 
    {
        public override PacketType PacketType { get; } = PacketType.CONNECT;

        /// <summary>
        ///     Following flags are marked: User Name Flag, Password Flag, Clean Session
        /// </summary>
        public byte ConnectFlags { get; set; } = 194;

        public string ProtocolName { get; set; } = "MQTToT";

        public byte ProtocolLevel { get; set; } = 3;

        public ushort KeepAliveInSeconds { get; set; } = 900;

        public bool CleanSession { get; set; }

        public bool HasWill { get; set; }

        public IBuffer WillMessage { get; set; }

        public QualityOfService WillQualityOfService { get; set; }

        public bool WillRetain { get; set; }

        public bool HasPassword { get; set; }

        public bool HasUsername { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public string WillTopicName { get; set; }

        public IBuffer Payload { get; set; }
    }
}
