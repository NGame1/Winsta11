﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Windows.Storage.Streams;

namespace WinstaBackgroundHelpers.Mqtt.Packets
{
    public sealed class ConnectPacket : Packet
    {
        public override PacketType PacketType => PacketType.CONNECT;

        public string ProtocolName { get; set; }

        public byte ProtocolLevel { get; set; }

        public bool CleanSession { get; set; }

        public bool HasWill { get; set; }

        public QualityOfService WillQualityOfService { get; set; }

        public bool WillRetain { get; set; }

        public bool HasPassword { get; set; }

        public bool HasUsername { get; set; }

        public ushort KeepAliveInSeconds { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ClientId { get; set; }

        public string WillTopicName { get; set; }

        public IBuffer WillMessage { get; set; }
    }
}