﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Collections.Generic;

namespace WinstaBackgroundHelpers.Mqtt.Packets
{
    public sealed class UnsubscribePacket : PacketWithId
    {
        public UnsubscribePacket()
        {
        }

        public UnsubscribePacket(ushort packetId, params string[] topicFilters)
        {
            this.PacketId = packetId;
            this.TopicFilters = topicFilters;
        }

        public override PacketType PacketType => PacketType.UNSUBSCRIBE;

        public override QualityOfService QualityOfService => QualityOfService.AtLeastOnce;

        public IEnumerable<string> TopicFilters { get; set; }
    }
}