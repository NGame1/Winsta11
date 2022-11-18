/*
 * Copyright (c) Indirect
 * https://github.com/huynhsontung/Indirect
 * 
 * Modifications: Ramtin
 */
// This class is a painful class, maybe I should deep into it in a later time...

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
using WinstaBackgroundHelpers;
using WinstaBackgroundHelpers.Mqtt.Packets;
using Ionic.Zlib;
using Newtonsoft.Json;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Android.DeviceInfo;
using InstagramApiSharp.API;
using System.Diagnostics;
using System.Globalization;
using WinstaBackgroundHelpers.Push.Packets;
using Windows.Networking.NetworkOperators;
using Windows.UI.WebUI;
using System.Linq;
using Windows.System.Profile;
using InstagramApiSharp.Helpers;
using InstagramApiSharp.API.RealTime.Handlers;
using Windows.Security.Cryptography.Certificates;
using System.Runtime.CompilerServices;
using InstagramApiSharp.API.Push;

namespace WinstaBackgroundHelpers.Push
{
    public class PushClient : IPushClient
    {
        public string LatestErr { get; private set; }
        public bool DontTransferSocket { get; set; } = false;
        public event EventHandler<object> FbnsTokenChanged;
        public event EventHandler<PushReceivedEventArgs> MessageReceived;
        //public event EventHandler<InstaBroadcastEventArgs> BroadcastChanged;
        public event EventHandler<object> LogReceived; 
        public FbnsConnectionData ConnectionData { get; private set; } = new FbnsConnectionData();
        public StreamSocket Socket { get; private set; }

        private const string HOST_NAME = "mqtt-mini.facebook.com"; 
        private const string BACKGROUND_ACTIVITY_ENTRY_NAME_POINT = "WinstaBackgroundTask.SocketActivityTask";
        private const string BACKGROUND_ACTIVITY_ENTRY_POINT = "WinstaBackgroundTask.SocketActivityTask";
        private const string SOCKET_ID = "winstamqttfbns";

        private IBackgroundTaskRegistration _socketActivityTask;
        //private IBackgroundTaskRegistration _pushNotifyTask;

        bool OnlyOnce = false;
        int RetryConnection = 0;
        bool IsRetrying = false;
        private const int MAX_RETRY_COUNT = 5;
        private int RetryConnectionCountInForeground = 0;
        private const int KEEP_ALIVE = 900;    // seconds
        private const int TIMEOUT = 5;
        private bool _waitingForPubAck;
        private CancellationTokenSource _runningTokenSource;
        private DataReader _inboundReader;
        private DataWriter _outboundWriter;
        private readonly IInstaApi _instaApi;
        private readonly List<IInstaApi> ApiList;
        public bool IsRunningFromBackground { get; set; } = false;
        public PushClient(List<IInstaApi> apis, IInstaApi api)
        {
            _runningTokenSource = new CancellationTokenSource();
            ApiList = apis;
            _instaApi = api ?? throw new ArgumentException("Api can't be null", nameof(api));
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void ValidateData()
        {
            ConnectionData =  new FbnsConnectionData();
            var unix = DateTime.UtcNow.ToUnixTime();
            if (ConnectionData.FbnsTokenUpdatedAt > 0 && (unix.FromUnixTimeSeconds() - ConnectionData.FbnsTokenUpdatedAt.FromUnixTimeSeconds()).TotalHours > 24)
            {
                ConnectionData.FbnsToken = "";
                ConnectionData.FbnsTokenUpdatedAt = 0;
            }
            if (string.IsNullOrEmpty(ConnectionData.UserAgent))
                ConnectionData.UserAgent = FbnsUserAgent.BuildFbUserAgent(_instaApi);
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void OpenNow()
        {
            NetworkInformation.NetworkStatusChanged += async sender =>
            {
                var internetProfile = NetworkInformation.GetInternetConnectionProfile();
                if (internetProfile == null || _runningTokenSource.IsCancellationRequested) return;
                Shutdown();
                await StartFresh();
            };
        }
        public void UnregisterTasks() { }
        public async Task UnregisterTasksAsync()
        {
            Debug.WriteLine("----------------------BackgroundTaskRegistration.AllTasks----------------------");
            foreach (var task in BackgroundTaskRegistration.AllTasks)
            {
                Debug.WriteLine(task.Value.Name);
                switch (task.Value.Name)
                {
                    case BACKGROUND_ACTIVITY_ENTRY_POINT:
                        //_socketActivityTask = task.Value;
                        task.Value.Unregister(true);
                        break;
                    //case "MinistaBH.NotifyQuickReplyTask":
                    //    task.Value.Unregister(true);
                    //    break;
                }
            }
            await Task.Delay(15);
            Debug.WriteLine("----------------------BackgroundTaskRegistration.AllTasks----------------------");
        }


        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async Task<bool> RequestBackgroundAccess()
        {
            if (DontTransferSocket || IsRunningFromBackground) return false;

            //return false;
            var permissionResult = await BackgroundExecutionManager.RequestAccessAsync();
            if (permissionResult == BackgroundAccessStatus.DeniedByUser ||
                permissionResult == BackgroundAccessStatus.DeniedBySystemPolicy ||
                permissionResult == BackgroundAccessStatus.Unspecified)
                return false;

            await UnregisterTasksAsync();
            BackgroundTaskBuilder backgroundTaskBuilder;
            if (CanRunInBG())
            {
                if (_socketActivityTask == null)
                {
                    backgroundTaskBuilder = new BackgroundTaskBuilder
                    {
                        Name = BACKGROUND_ACTIVITY_ENTRY_NAME_POINT,
                        TaskEntryPoint = BACKGROUND_ACTIVITY_ENTRY_POINT
                    };
                    backgroundTaskBuilder.SetTrigger(new SocketActivityTrigger());
                    _socketActivityTask = backgroundTaskBuilder.Register();
                }
            }
            return true;
        }

        void PrintException(string from, Exception exception)
        {
            Log(from + "  " + SocketError.GetStatus(exception.HResult).ToString());
        }

        /// <summary>
        /// Transfer socket as well as necessary context for background push notification client. 
        /// Transfer only happens if user is logged in.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async Task TransferPushSocket()
        {
            //return;
            if (!_instaApi.IsUserAuthenticated || _runningTokenSource.IsCancellationRequested) return;
            if (DontTransferSocket) return;
            Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Transferring sockets");
            await SendPing().ConfigureAwait(false);
            await Task.Delay(TimeSpan.FromSeconds(3.5));  // grace period
            Shutdown();
            await Socket.CancelIOAsync();


            Socket.TransferOwnership(
                SOCKET_ID
                ,
                null,
                TimeSpan.FromSeconds(KEEP_ALIVE - 60)

                );
            //Socket.Dispose();
        }
        private bool CanRunInBG() 
        {
            if (IsMobile)
            {
//#if DEBUG
                return true;
//#else
//                return false;
//#endif
            }
            else
                return true;
        }
        public static bool IsMobile => AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile";
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async void Start()
        {
            if (!_instaApi.IsUserAuthenticated) return;
            try
            {
                if (CanRunInBG())
                {
                    if (SocketActivityInformation.AllSockets.TryGetValue(SOCKET_ID, out var socketInformation))
                    {
                        var socket = socketInformation.StreamSocket;
                        if (string.IsNullOrEmpty(ConnectionData.FbnsToken)) // if we don't have any push data, start fresh
                            await StartFresh().ConfigureAwait(false);
                        else
                        {
                            //socket.TransferOwnership(SOCKET_ID);
                            await StartWithExistingSocket(socket).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        await StartFresh().ConfigureAwait(false);
                    }
                }
                else
                {
                    await StartFresh().ConfigureAwait(false);
                }
            }
            catch (Exception ex)
            {
                PrintException("Start", ex);
                await StartFresh().ConfigureAwait(false);
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async Task StartWithExistingSocket(StreamSocket socket)
        {
            try
            {
                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Starting with existing socket");
                //Shutdown();
                Socket = socket;
                _inboundReader = new DataReader(socket.InputStream);
                _outboundWriter = new DataWriter(socket.OutputStream);
                _inboundReader.ByteOrder = ByteOrder.BigEndian;
                _inboundReader.InputStreamOptions = InputStreamOptions.Partial;
                _outboundWriter.ByteOrder = ByteOrder.BigEndian;
                _runningTokenSource = new CancellationTokenSource();

                StartPollingLoop();
                await SendPing().ConfigureAwait(false);
                StartKeepAliveLoop();
            }
            catch (TaskCanceledException)
            {
                // pass
            }
            catch (Exception ex)
            {
                PrintException("StartWithExistingSocket", ex);
                Shutdown();
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async Task StartFresh() => await StartFresh(false);
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async Task StartFresh(bool withBG)
        {
            try
            {
                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Starting fresh");
                var connectPacket = new FbnsConnectPacket
                {
                    Payload = await PayloadProcessor.BuildPayload(ConnectionData)
                };

                Socket = new StreamSocket();
                Socket.Control.KeepAlive = true;
                Socket.Control.NoDelay = true;
                if (await RequestBackgroundAccess() && !withBG)
                {
                    OnlyOnce = false;
                    if (CanRunInBG())
                    {
                        try
                        {
                            Socket.EnableTransferOwnership(_socketActivityTask.TaskId, SocketActivityConnectedStandbyAction.Wake);
                        }
                        catch (Exception connectedStandby)
                        {
                            Log(connectedStandby);
                            Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Connected standby not available");
                            try
                            {
                                Socket.EnableTransferOwnership(_socketActivityTask.TaskId, SocketActivityConnectedStandbyAction.DoNotWake);
                            }
                            catch (Exception e)
                            {
                                Log(e);
                                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Failed to transfer socket completely!");
                                if (RetryConnection > 3)
                                {
                                    Shutdown();
                                    await StartFresh(true);
                                    RetryConnection++;
                                    return;
                                }
                                return;
                            }
                        }
                    }
                }
                _runningTokenSource = new CancellationTokenSource();
                await Socket.ConnectAsync(new HostName(HOST_NAME), "443", SocketProtectionLevel.Tls12);
                _inboundReader = new DataReader(Socket.InputStream);
                _outboundWriter = new DataWriter(Socket.OutputStream);
                _inboundReader.ByteOrder = ByteOrder.BigEndian;
                _inboundReader.InputStreamOptions = InputStreamOptions.Partial;
                _outboundWriter.ByteOrder = ByteOrder.BigEndian;
                await Task.Delay(2500);
                await FbnsPacketEncoder.EncodePacket(connectPacket, _outboundWriter);
                await Task.Delay(2500);
                StartPollingLoop();
            }
            catch (Exception ex)
            {
                PrintException("StartFresh", ex);
                Shutdown();
                if(!OnlyOnce)
                {
                    OnlyOnce = true;
                    await StartFresh(true);
                }
            }

        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public void Shutdown()
        {
            _runningTokenSource?.Cancel();
            _inboundReader = null;
            _outboundWriter?.DetachStream();
            _outboundWriter?.Dispose();
            _outboundWriter = null;
            try
            {
                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Stopped pinging push server");
            }
            catch (Exception) { Log("[NULL!] Stopped pinging push server"); }
            Retry();
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async void Retry()
        {
            if (IsRunningFromBackground) return;
            if(RetryConnectionCountInForeground > MAX_RETRY_COUNT)
            {
                IsRetrying = true;
                try
                {
                    Socket.Dispose();
                    Socket = null;
                }
                catch { }
                RetryConnectionCountInForeground++;
                await StartFresh();
                IsRetrying = false;
            }
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async void StartPollingLoop()
        {
            int tried = 0;
            while (!(_runningTokenSource?.IsCancellationRequested ?? false) && _inboundReader != null)
            {
                var reader = _inboundReader;
                try
                {
                    await reader.LoadAsync(FbnsPacketDecoder.PACKET_HEADER_LENGTH);
                }
                catch (Exception ex)
                {
                    PrintException("StartPollingLoop", ex);
                    Log("StartPollingLoop: " + ex.Message + "\n" + ex.Source + "\n");
                    // Connection closed (most likely)
                    return;
                }
                try
                {
                    var packet = await FbnsPacketDecoder.DecodePacket(reader);
                    await OnPacketReceived(packet);
                }
                catch (Exception ex)
                {
                    PrintException("StartPollingLoop22", ex);
                    Log("StartPollingLoop22: " + ex.Message + "\n" + ex.Source + "\n");
                    if (tried > 3)
                    {
                        Shutdown();
                        Start();
                        break;
                    }
                    tried++;

                    if (tried <= 3)
                        await Task.Delay(10000); 
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        public async Task SendPing()
        {
            try
            {
                var packet = PingReqPacket.Instance;
                await FbnsPacketEncoder.EncodePacket(packet, _outboundWriter);
                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Pinging Push server");
            }
            catch (Exception ex)
            {
                PrintException("SendPing", ex);
                Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Failed to ping Push server. Shutting down.");
                Shutdown();
            }
        }

        public enum TopicIds
        {
            Message = 76,   // "/fbns_msg"
            RegReq = 79,    // "/fbns_reg_req"
            RegResp = 80    // "/fbns_reg_resp"
        }
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async Task OnPacketReceived(Packet msg)
        {
            try
            {
                switch (msg.PacketType)
                {
                    case PacketType.CONNACK:
                        Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Received CONNACK");
                        ConnectionData.UpdateAuth(((FbnsConnAckPacket)msg).Authentication);
                        await RegisterMqttClient();
                        break;

                    case PacketType.PUBLISH:
                        Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Received PUBLISH");
                        var publishPacket = (PublishPacket) msg;
                        if (publishPacket.Payload == null)
                            throw new Exception($"{nameof(PushClient)}: Publish packet received but payload is null");
                        if (publishPacket.QualityOfService == QualityOfService.AtLeastOnce)
                        {
                            await FbnsPacketEncoder.EncodePacket(PubAckPacket.InResponseTo(publishPacket), _outboundWriter);
                        }

                        var payload = DecompressPayload(publishPacket.Payload);
                        var json = Encoding.UTF8.GetString(payload);
                        Log($"[{_instaApi.GetLoggedUser().UserName}] " + $"MQTT json: {json}");
                        switch (Enum.Parse(typeof(TopicIds), publishPacket.TopicName))
                        {
                            case TopicIds.Message:
                                var message = JsonConvert.DeserializeObject<PushReceivedEventArgs>(json);
                                message.Json = json;
                                message.InstaApi = _instaApi;
                                Debug.WriteLine(message.NotificationContentJson);

                                MessageReceived?.Invoke(this, message);
                                break;
                            case TopicIds.RegResp:
                                await OnRegisterResponse(json);
                                StartKeepAliveLoop();
                                break;
                            default:
                                Log($"[{_instaApi.GetLoggedUser().UserName}] " + $"Unknown topic received: {publishPacket.TopicName}");
                                break;
                        }

                        break;

                    case PacketType.PUBACK:
                        Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Received PUBACK");
                        _waitingForPubAck = false;
                        break;

                    // todo: PingResp never arrives even though data was received. Decoder problem?
                    case PacketType.PINGRESP:
                        Log($"[{_instaApi.GetLoggedUser().UserName}] " + "Received PINGRESP");
                        break;

                    default: return;
                        //throw new NotSupportedException($"Packet type {msg.PacketType} is not supported.");
                }
            }
            catch (Exception ex)
            {
                PrintException("SendPing", ex);
                Shutdown();
            }
        }

        /// Referencing from https://github.com/mgp25/Instagram-API/blob/master/src/Push.php
        /// <summary>
        ///     After receiving the token, proceed to register over Instagram API
        /// </summary>
        private async Task OnRegisterResponse(string json)
        {
            try
            {
                var response = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                if (!string.IsNullOrEmpty(response["error"]))
                {
                    Log($"[{_instaApi.GetLoggedUser().UserName}] " + $"{response["error"]}");
                    Shutdown();
                }

                var token = response["token"];

                await RegisterClient(token);
            }
            catch (Exception e)
            {
                Log(e);
                Shutdown();
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        internal async Task RegisterClient(string token)
        {
            if (string.IsNullOrEmpty(token)) return;
            //var tokenChanged = false;
            //var a = DateTime.UtcNow.ToUnixTime().FromUnixTimeSeconds() - ConnectionData.FbnsTokenUpdatedAt.FromUnixTimeSeconds();
            //if ((a).TotalHours <= 24 && !string.IsNullOrEmpty(ConnectionData.FbnsToken))
            //{
            //    //token = ConnectionData.FbnsToken;
            //}
            //else
            //    tokenChanged = true;
            var deviceInfo = _instaApi.GetCurrentDevice();
            var user = _instaApi.GetLoggedUser();
            var uri = UriCreator.GetPushRegisterUri();
            var users = ApiList.Select(s => s.GetLoggedUser().LoggedInUser.Pk).ToList();
            if (!ApiList.Any(x => x.GetCurrentDevice().DeviceGuid.ToString() != deviceInfo.DeviceGuid.ToString()))
            {
                users.Clear();
                users.Add(user.LoggedInUser.Pk);
            }

            var fields = new Dictionary<string, string>
            {
                {"device_type", "android_mqtt"},
                {"is_main_push_channel", "true"},
                {"phone_id", deviceInfo.PhoneGuid.ToString()},
                {"device_token", token},
                {"guid", deviceInfo.PhoneGuid.ToString()},
                {"_uuid", deviceInfo.DeviceGuid.ToString()},
                {"users", string.Join(",", users)/*user.LoggedInUser.Pk.ToString()*/ }
            };
            Debug.WriteLine(JsonConvert.SerializeObject(fields, Formatting.Indented));
            var request = _instaApi.HttpHelper.GetDefaultRequest(System.Net.Http.HttpMethod.Post, uri, deviceInfo, fields);
            var response = await _instaApi.HttpRequestProcessor.SendAsync(request);
            Debug.WriteLine(await response.Content.ReadAsStringAsync());

            ConnectionData.FbnsToken = token;

            //if (tokenChanged)
            {
                ConnectionData.FbnsTokenUpdatedAt = DateTime.UtcNow.ToUnixTime();
                FbnsTokenChanged?.Invoke(this, "FbnsToken");
            }
        }

        /// <summary>
        ///     Register this client on the MQTT side stating what application this client is using.
        ///     The server will then return a token for registering over Instagram API side.
        /// </summary>
        /// <param name="ctx"></param>
        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async Task RegisterMqttClient()
        {
            var message = new Dictionary<string, string>
            {
                {"pkg_name", "com.instagram.android"},
                {"appid", "567067343352427"}
            };

            var json = JsonConvert.SerializeObject(message);
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            byte[] compressed;
            using (var compressedStream = new MemoryStream(jsonBytes.Length))
            {
                using (var zlibStream =
                    new ZlibStream(compressedStream, CompressionMode.Compress, CompressionLevel.Level9, true))
                {
                    zlibStream.Write(jsonBytes, 0, jsonBytes.Length);
                }
                compressed = new byte[compressedStream.Length];
                compressedStream.Position = 0;
                compressedStream.Read(compressed, 0, compressed.Length);
            }
            Debug.WriteLine(string.Join(",", compressed));

            var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
            {
                Payload = compressed.AsBuffer(),
                PacketId = (ushort) CryptographicBuffer.GenerateRandomNumber(),
                TopicName = ((byte)TopicIds.RegReq).ToString()
            };

            // Send PUBLISH packet then wait for PUBACK
            // Retry after TIMEOUT seconds
            await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);
            WaitForPubAck();
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async void WaitForPubAck()
        {
            _waitingForPubAck = true;
            await Task.Delay(TimeSpan.FromSeconds(TIMEOUT));
            try
            {
                if (_waitingForPubAck)
                {
                    await RegisterMqttClient();
                }
            }
            catch { }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private async void StartKeepAliveLoop()
        {
            if (_runningTokenSource == null) return;
            try
            {
                while (!_runningTokenSource.IsCancellationRequested)
                {
                    await SendPing();
                    await Task.Delay(TimeSpan.FromSeconds(KEEP_ALIVE - 60), _runningTokenSource.Token);
                }
            }
            catch (TaskCanceledException)
            {
                // pass
            }
        }

        [MethodImpl(MethodImplOptions.NoOptimization | MethodImplOptions.NoInlining)]
        private byte[] DecompressPayload(IBuffer payload)
        {
            var compressedStream = payload.AsStream();

            var decompressedStream = new MemoryStream(256);
            using (var zlibStream = new ZlibStream(compressedStream, CompressionMode.Decompress, true))
            {
                zlibStream.CopyTo(decompressedStream);
            }

            var data = decompressedStream.GetWindowsRuntimeBuffer(0, (int) decompressedStream.Length);
            return data.ToArray();
        }


        void Log(object message)
        {
            Debug.WriteLine($"[{DateTime.Now.ToString(CultureInfo.CurrentCulture)} ]: {message}");
            LogReceived?.Invoke(this, $"[{DateTime.Now.ToString(CultureInfo.CurrentCulture)} ]: {message}");

        }
    }

    internal static class PushClientX
    {
        public static void Log(this object source, object message)
        {
            Debug.WriteLine($"[{DateTime.Now.ToString(CultureInfo.CurrentCulture)} - {source?.GetType().Name}]: {message}");
        }
    }
}
