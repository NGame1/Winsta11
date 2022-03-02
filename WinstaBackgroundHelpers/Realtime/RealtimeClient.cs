// Why this class has a lot of TRY...CATCH?
// BECAUSE in windows 10 mobiles anything related to this class is unstable, I don't know why, but I just put anything in TRY...CATCH

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Ionic.Zlib;
using Newtonsoft.Json;
using System.Diagnostics;
using Thrift.Transport.Client;
using Thrift.Protocol.Entities;
using Thrift.Protocol;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.API.RealTime.Handlers;
using InstagramApiSharp.API.RealTime.Responses.Models;
using InstagramApiSharp.Logger;
using WinstaBackgroundHelpers.Push.Packets;
using WinstaBackgroundHelpers.Mqtt.Packets;
using InstagramApiSharp.API.RealTime.Subs;
using Thrift;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Classes.ResponseWrappers;
using InstagramApiSharp.API.RealTime.Responses.Wrappers;
using WinstaBackgroundHelpers;

namespace InstagramApiSharp.API.RealTime
{
    public sealed partial class RealtimeClient
    {
        private const string DEFAULT_HOST = "edge-mqtt.facebook.com";
        public RealtimeResponseList Responses { get; private set; } = new RealtimeResponseList();

        public event EventHandler<object> LogReceived;
        public event EventHandler<List<InstaDirectInboxItem>> DirectItemChanged;
        public event EventHandler<PresenceEventEventArgs> PresenceChanged;
        public event EventHandler<InstaBroadcastEventArgs> BroadcastChanged;
        public event EventHandler<List<InstaRealtimeTypingEventArgs>> TypingChanged;
        //public event EventHandler<Exception> FailedToStart;
        public event EventHandler<object> OnDisconnect;
        
        //readonly FbnsConnectionData ConnectionData = new FbnsConnectionData();
        public TimeSpan WaitForResponseDelay { get; private set; } = TimeSpan.FromMilliseconds(450);
        public StreamSocket Socket { get; private set; }
        public bool Running => !(_runningTokenSource?.IsCancellationRequested ?? true);
        public const int KEEP_ALIVE = 900;    // seconds
        private const int TIMEOUT = 5;
        private CancellationTokenSource _runningTokenSource;
        private DataReader _inboundReader;
        private DataWriter _outboundWriter;
        private readonly IInstaApi _instaApi;
        private int SeqId;
        private DateTime SnapshotAt;
        public bool KeepConnectionAlive { get; set; } = true;
        public bool AppIsInBackground { get; set; } = false;
        public RealtimeClient(IInstaApi api)
        {
            _instaApi = api ?? throw new ArgumentException("Api can't be null", nameof(api));
            AppIsInBackground = false;
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
        }

        public void SetResponseDelay(TimeSpan? delay)
        {
            if (delay == null)
                WaitForResponseDelay = TimeSpan.FromMilliseconds(450);
            else
            {
                if (delay.Value.TotalMilliseconds < 450)
                    WaitForResponseDelay = TimeSpan.FromMilliseconds(450);
                else
                    WaitForResponseDelay = delay.Value;
            }
        }

        public static bool InternetAvailable()
        {
            var internetProfile = NetworkInformation.GetInternetConnectionProfile();
            return internetProfile != null;
        }
        async void Retry()
        {
            if (AppIsInBackground) return;
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(2.5));
                if (!InternetAvailable() || Running) return;
                await StartFresh();
            }
            catch
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                try
                {
                    Retry();
                }
                catch { }
            }
        }
        private void OnNetworkStatusChanged(object sender)
        {
            Retry();
        }
   
        public async Task Start(int seqId, DateTime snapshotAt)
        {
            if (AppIsInBackground) return;
            try
            {
                if (!_instaApi.IsUserAuthenticated || Running) return;

                if (seqId == 0)
                {
                    var inbox = await _instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                    if (inbox.Succeeded)
                    {
                        seqId = inbox.Value.SeqId;
                        snapshotAt = inbox.Value.SnapshotAt;
                    }
                    else
                        return;
                }
                SeqId = seqId;
                SnapshotAt = snapshotAt;
                try
                {
                    await StartFresh().ConfigureAwait(false);
                }
                catch (Exception)
                {
                    await StartFresh().ConfigureAwait(false);
                }
            }
            catch { }
        }
        public async Task StartFresh()
        {
            Log("Starting fresh");
            if (!InternetAvailable())
            {
                Log("Internet not available. Exiting.");
                return;
            }

            if (Running)
            {
                Log("realtime client is already running");
                return;
            }

            if (AppIsInBackground)
            {
                Log("Application is in background");
                return;
            }
            if (Socket != null)
            {
                Shutdown();
                await Task.Delay(350);
            }
            try
            {
                var connectPacket = new FbnsConnectPacket
                {
                    Payload = await RealtimePayload.BuildPayload(_instaApi)
                };
                Socket = new StreamSocket();
                Socket.Control.KeepAlive = true;
                Socket.Control.NoDelay = true;
                Socket.Control.OutboundBufferSizeInBytes = 20 * 1024 * 1024;
                await Socket.ConnectAsync(new HostName(DEFAULT_HOST), "443", SocketProtectionLevel.Tls12);
                _inboundReader = new DataReader(Socket.InputStream);
                _outboundWriter = new DataWriter(Socket.OutputStream);
                _inboundReader.ByteOrder = ByteOrder.BigEndian;
                _inboundReader.InputStreamOptions = InputStreamOptions.Partial;
                _outboundWriter.ByteOrder = ByteOrder.BigEndian;
                _runningTokenSource = new CancellationTokenSource();
                await FbnsPacketEncoder.EncodePacket(connectPacket, _outboundWriter);
            }
            catch (Exception e)
            {
                e.PrintInDebug();
                Restart();
                return;
            }
            StartPollingLoop();
        }
        async Task SubscribeForDM()
        {
            var messageSync = new SubscriptionRequest("/ig_message_sync", QualityOfService.AtLeastOnce);
            var sendMessageResp = new SubscriptionRequest("/ig_send_message_response", QualityOfService.AtLeastOnce);
            var subIrisResp = new SubscriptionRequest("/ig_sub_iris_response", QualityOfService.AtLeastOnce);
            var subscribePacket = new SubscribePacket((ushort)CryptographicBuffer.GenerateRandomNumber(), messageSync, sendMessageResp, subIrisResp);

            await FbnsPacketEncoder.EncodePacket(subscribePacket, _outboundWriter);
        }

        async Task RealtimeSub()
        {
            var user = _instaApi.GetLoggedUser().LoggedInUser;
            var dic = new Dictionary<string, List<string>>
            {
                {  "sub",
                    new List<string>
                    {
                        GraphQLSubscriptions.GetAppPresenceSubscription(),
                        GraphQLSubscriptions.GetZeroProvisionSubscription(_instaApi.GetCurrentDevice().DeviceGuid.ToString()),
                        GraphQLSubscriptions.GetDirectStatusSubscription(),
                        GraphQLSubscriptions.GetDirectTypingSubscription(user?.Pk.ToString()),
                        GraphQLSubscriptions.GetAsyncAdSubscription(user?.Pk.ToString())
                    }
                }
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);
            var dataStream = new MemoryStream(512);
            using (var zlibStream = new ZlibStream(dataStream, CompressionMode.Compress, CompressionLevel.Level9, true))
            {
                await zlibStream.WriteAsync(bytes, 0, bytes.Length);
            }

            var compressed = dataStream.GetWindowsRuntimeBuffer(0, (int)dataStream.Length);
            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = compressed,
                PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                TopicName = "/ig_realtime_sub"
            };
            await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);
        }

        async Task PubSub()
        {
            var user = _instaApi.GetLoggedUser().LoggedInUser;
            var dic = new Dictionary<string, List<string>>
            {
                {  "sub",
                    new List<string>
                    {
                        SkyWalker.DirectSubscribe(user?.Pk.ToString()),
                        SkyWalker.LiveSubscribe(user?.Pk.ToString()),
                    }
                }
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);
            var dataStream = new MemoryStream(512);
            using (var zlibStream = new ZlibStream(dataStream, CompressionMode.Compress, CompressionLevel.Level9, true))
            {
                await zlibStream.WriteAsync(bytes, 0, bytes.Length);
            }

            var compressed = dataStream.GetWindowsRuntimeBuffer(0, (int)dataStream.Length);
            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = compressed,
                PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                TopicName = "/pubsub"
            };
            await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);
        }

        async Task IrisSub()
        {
            var dic = new Dictionary<string, object>
            {
                {"seq_id", SeqId.ToString()},
                {"sub", new List<string>()},
                {"snapshot_at_ms", SnapshotAt.ToUnixTimeMiliSeconds()}
            };
            var json = JsonConvert.SerializeObject(dic);
            var bytes = Encoding.UTF8.GetBytes(json);
            var dataStream = new MemoryStream(512);
            using (var zlibStream = new ZlibStream(dataStream, CompressionMode.Compress, CompressionLevel.Level9, true))
            {
                await zlibStream.WriteAsync(bytes, 0, bytes.Length);
            }

            var compressed = dataStream.GetWindowsRuntimeBuffer(0, (int)dataStream.Length);
            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = compressed,
                PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                TopicName = "/ig_sub_iris"
            };
            await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);
        }
        public void Shutdown()
        {
            Log("Stopping realtime server");
            try
            {
                NetworkInformation.NetworkStatusChanged -= OnNetworkStatusChanged;
            }
            catch { }
            try
            {
                _runningTokenSource?.Cancel();
            }
            catch { }
            try
            {
                _inboundReader?.Dispose();
                _outboundWriter?.DetachStream();
                _outboundWriter?.Dispose();
            }
            catch { }
            try
            {
                Socket?.Dispose();
            }
            catch { }
        }

        private async void Restart()
        {
            Log("Restarting realtime server");
            Shutdown();
            if (AppIsInBackground) return;
            await Task.Delay(TimeSpan.FromSeconds(3));
            if (Running) return;
            try
            {
                var inbox = await _instaApi.MessagingProcessor.GetDirectInboxAsync(PaginationParameters.MaxPagesToLoad(1));
                if(inbox.Succeeded)
                {
                    SeqId = inbox.Value.SeqId;
                    SnapshotAt = inbox.Value.SnapshotAt;
                }
            }
            catch { }
            try
            {
                await StartFresh();
                NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            }
            catch { }
        }

        private async void StartPollingLoop()
        {
            try
            {
                while (Running && !AppIsInBackground)
                {
                    try
                    {
                        var reader = _inboundReader;
                        await reader.LoadAsync(PacketDecoder.PACKET_HEADER_LENGTH);
                        var packet = await PacketDecoder.DecodePacket(reader);
                        await OnPacketReceived(packet);
                    }
                    catch (Exception e)
                    {
                        if (Running && !AppIsInBackground)
                        {
                            e.PrintInDebug();
                            OnDisconnect?.Invoke(this, true);
                            Restart();
                        }
                        return;
                    }
                }
            }
            catch { }
        }
        public enum TopicIds
        {
            MessageSync = 146,          //      /ig_message_sync
            PubSub = 88,                //      /pubsub,
            RealtimeSub = 149,          //      /ig_realtime_sub
            SendMessageResponse = 133,  //      /ig_send_message_response,
        }

        private async Task OnPacketReceived(Packet msg)
        {
            if (!Running) return;
            if (AppIsInBackground) return;
            try
            {
                var writer = _outboundWriter;
                switch (msg.PacketType)
                {
                    case PacketType.CONNACK:
                        this.Log("Received CONNACK");
                        await SubscribeForDM();
                        await RealtimeSub();
                        await PubSub();
                        if (SnapshotAt != null && SeqId <= 0)
                            await IrisSub();
                        StartKeepAliveLoop();
                        break;

                    case PacketType.PUBLISH:
                        this.Log("Received PUBLISH");
                        var publishPacket = (PublishPacket)msg;
                        if (publishPacket.Payload == null)
                            throw new Exception($"{nameof(RealtimeClient)}: Publish packet received but payload is null");
                        if (publishPacket.QualityOfService == QualityOfService.AtLeastOnce)
                        {
                            await FbnsPacketEncoder.EncodePacket(PubAckPacket.InResponseTo(publishPacket), writer);
                        }


                        var payload = DecompressPayload(publishPacket.Payload);
                        var json = await GetJsonFromThrift(payload);
                        this.Log($"MQTT json: {json}");
                        if (string.IsNullOrEmpty(json)) break;
                        try
                        {
                            Debug.WriteLine("");
                            Debug.WriteLine($"Unknown topic received:{msg.PacketType} :  {publishPacket.TopicName} : {json}");


                            Debug.WriteLine("");
                            Debug.WriteLine(json);
                            switch (publishPacket.TopicName)
                            {
                                case "150": break;
                                case "133": //      /ig_send_message_response
                                    try
                                    {
                                        Responses.AddItem(JsonConvert.DeserializeObject<InstaDirectRespondResponse>(json));
                                    }
                                    catch
                                    {
                                        try
                                        {
                                            var o = JsonConvert.DeserializeObject<InstaDirectRespondV2Response>(json);
                                            Responses.AddItem(new InstaDirectRespondResponse
                                            {
                                                Action = o.Action,
                                                Message = o.Message,
                                                Status = o.Status,
                                                StatusCode = o.StatusCode,
                                                Payload = o.Payload[0]
                                            });
                                        }
                                        catch { }
                                    }
                                    break;
                                case "88":
                                    {
                                        var obj = JsonConvert.DeserializeObject<InstaRealtimeRespondResponse>(json);
                                        if (obj?.Data?.Length > 0)
                                        {
                                            var typing = new List<InstaRealtimeTypingEventArgs>();
                                            var dm = new List<InstaDirectInboxItem>();
                                            for (int i = 0; i < obj.Data.Length; i++)
                                            {
                                                var item = obj.Data[i];
                                                if (item != null)
                                                {
                                                    if (item.IsTyping)
                                                    {
                                                        var typingResponse = JsonConvert.DeserializeObject<InstaRealtimeTypingResponse>(item.Value);
                                                        if (typingResponse != null)
                                                        {
                                                            try
                                                            {
                                                                var tr = new InstaRealtimeTypingEventArgs
                                                                {
                                                                    SenderId = typingResponse.SenderId,
                                                                    ActivityStatus = typingResponse.ActivityStatus,
                                                                    RealtimeOp = item.Op,
                                                                    RealtimePath = item.Path,
                                                                    TimestampUnix = typingResponse.Timestamp,
                                                                    Timestamp = DateTimeHelper.FromUnixTimeMiliSeconds(typingResponse.Timestamp),
                                                                    Ttl = typingResponse.Ttl
                                                                };
                                                                typing.Add(tr);
                                                            }
                                                            catch { }
                                                        }
                                                    }
                                                    else if (item.IsBroadcast)
                                                    {
                                                        if (item.HasItemInValue)
                                                        {
                                                            var broadcastEventArgs = JsonConvert.DeserializeObject<InstaBroadcastEventArgs>(item.Value);
                                                            if (broadcastEventArgs != null)
                                                                BroadcastChanged?.Invoke(this, broadcastEventArgs);
                                                        }
                                                    }
                                                    else if (item.IsThreadItem || item.IsThreadParticipants)
                                                    {
                                                        if (item.HasItemInValue)
                                                        {
                                                            var directItemResponse = JsonConvert.DeserializeObject<InstaDirectInboxItemResponse>(item.Value);
                                                            if (directItemResponse != null)
                                                            {
                                                                try
                                                                {
                                                                    var dI = ConvertersFabric.Instance.GetDirectThreadItemConverter(directItemResponse).Convert();
                                                                    dI.RealtimeOp = item.Op;
                                                                    dI.RealtimePath = item.Path;
                                                                    dm.Add(dI);
                                                                }
                                                                catch { }
                                                            }
                                                        }
                                                        else
                                                        {
                                                            var dI = new InstaDirectInboxItem
                                                            {
                                                                RealtimeOp = item.Op,
                                                                RealtimePath = item.Path,
                                                                ItemId = item.Value
                                                            };
                                                            dm.Add(dI);
                                                        }
                                                    }
                                                }
                                            }
                                            if (typing.Count > 0)
                                                OnTypingChanged(typing);
                                            if (dm.Count > 0)
                                                OnDirectItemChanged(dm);
                                        }

                                    }
                                    break;

                            }
                        }
                        catch { }
                        break;

                    case PacketType.PUBACK:
                        this.Log("Received PUBACK");
                        break;

                    case PacketType.PINGRESP:
                        this.Log("Received PINGRESP");
                        break;

                    default:
                        Debug.WriteLine($"Unknown topic received:{msg.PacketType}");
                        break;
                }
            }
            catch (Exception e)
            {
                e.PrintInDebug();
            }
        }
        async Task<string> GetJsonFromThrift(byte[] bytes)
        {
            try
            {
                var _memoryBufferTransport = new TMemoryBufferTransport(bytes, new TConfiguration());
                var _thrift = new TCompactProtocol(_memoryBufferTransport);
                while (true)
                {
                    var field = await _thrift.ReadFieldBeginAsync(CancellationToken.None);

                    if (field.Type == TType.Stop)
                        break;

                    if (field.Type == TType.String)
                    {
                        var str = await _thrift.ReadStringAsync(CancellationToken.None);
                        if (!string.IsNullOrEmpty(str))
                        {
                            if (str.StartsWith("{") && str.EndsWith("}"))
                                return str;
                        }
                    }
                    await _thrift.ReadFieldEndAsync(CancellationToken.None);
                }
            }
            catch { }
            var json = Encoding.UTF8.GetString(bytes);
            try
            {
                if (json.Contains("{") && json.Contains("}"))
                {
                    var str = json.Substring(json.IndexOf("{"));
                    str = str.Substring(0, str.LastIndexOf("}") + 1);
                    return str;
                }
            }
            catch { }
            return json;

        }

        private async void StartKeepAliveLoop()
        {
            if (_runningTokenSource == null) return;
            try
            {
                while (!_runningTokenSource.IsCancellationRequested && !AppIsInBackground)
                {
                    await Task.Delay(TimeSpan.FromMinutes(2), _runningTokenSource.Token);
                    try
                    {
                        await SendPing();
                    }
                    catch { }
                }
            }
            catch (TaskCanceledException)
            {
                // pass
            }
        }
        private async Task SendPing()
        {
            try
            {
                if (_outboundWriter == null) return;
                var packet = PingReqPacket.Instance;
                await FbnsPacketEncoder.EncodePacket(packet, _outboundWriter);
                Log($"[{_instaApi?.GetLoggedUser()?.UserName}] " + "Pinging realtime server");
            }
            catch (Exception)
            {
                Log($"[{_instaApi?.GetLoggedUser()?.UserName}] " + "Failed to ping realtime server. Shutting down.");
                Shutdown();
                await Task.Delay(5000);
                Restart();
            }
        }
        private byte[] DecompressPayload(IBuffer payload)
        {
            var compressedStream = payload.AsStream();

            var decompressedStream = new MemoryStream(256);
            using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress, true))
            {
                zlibStream.CopyTo(decompressedStream);
            }

            var data = decompressedStream.GetWindowsRuntimeBuffer(0, (int)decompressedStream.Length);
            return data.ToArray();
        }
        void Log(object message)
        {
            Debug.WriteLine($"[{DateTime.Now.ToString(CultureInfo.CurrentCulture)} ]: {message}");
            try
            {
                LogReceived?.Invoke(this, message);
            }
            catch { }
        }

        internal void OnPresenceChanged(PresenceEventEventArgs args)
        {
            try
            {
                PresenceChanged?.Invoke(this, args);
            }
            catch { }
        }
        internal void OnTypingChanged(List<InstaRealtimeTypingEventArgs> args)
        {
            try
            {
                TypingChanged?.Invoke(this, args);
            }
            catch { }
        }
        internal void OnDirectItemChanged(List<InstaDirectInboxItem> args)
        {
            try
            {
                DirectItemChanged?.Invoke(this, args);
            }
            catch { }
        }
    }
}
