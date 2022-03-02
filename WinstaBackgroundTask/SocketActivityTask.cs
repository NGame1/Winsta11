using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.API.Push;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Logger;
using Newtonsoft.Json;
using NotificationHandler;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Notifications;
using WinstaBackgroundHelpers.Push;
using WinstaCore;

namespace WinstaBackgroundTask
{
    public sealed class SocketActivityTask : IBackgroundTask
    {
        readonly CS CS = new CS();
        BackgroundTaskDeferral deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            deferral = taskInstance.GetDeferral();

            try
            {
                var details = (SocketActivityTriggerDetails)taskInstance.TriggerDetails;
                Debug.WriteLine($"{details.Reason}");
                var internetProfile = NetworkInformation.GetInternetConnectionProfile();
                if (internetProfile == null)
                {
                    Debug.WriteLine("No internet..");
                    return;
                }
                string selectedUser = null;

                if (ApplicationData.Current.LocalSettings.Values.TryGetValue("LastLoggedUser", out var LastLoggedUser))
                {
                    selectedUser = LastLoggedUser.ToString();
                }

                await CS.Load();
                A.InstaApiList = CS.InstaApiList;
                var api = !string.IsNullOrEmpty(selectedUser) ?
                    (CS.InstaApiList.FirstOrDefault(x => x.GetLoggedUser().LoggedInUser.UserName.ToLower() == selectedUser.ToLower()) ?? CS.InstaApiList[0]) : CS.InstaApiList[0];


                //foreach (var api in CS.InstaApiList)
                {
                    try
                    {
                        var push = new PushClient(CS.InstaApiList, api)
                        {
                            IsRunningFromBackground = true
                        };
                        push.MessageReceived += A.OnMessageReceived;
                        push.OpenNow();
                        //push.Start();

                        switch (details.Reason)
                        {
                            case SocketActivityTriggerReason.SocketClosed:
                                {
                                    await Task.Delay(TimeSpan.FromSeconds(5));
                                    await push.StartFresh();
                                    break;
                                }
                            default:
                                {
                                    var socket = details.SocketInformation.StreamSocket;
                                    await push.StartWithExistingSocket(socket);
                                    break;
                                }
                        }
                        await Task.Delay(TimeSpan.FromSeconds(5));
                        await push.TransferPushSocket();
                    }
                    catch { }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e);
                Debug.WriteLine($"{typeof(SocketActivityTask).FullName}: Can't finish push cycle. Abort.");
            }
            finally
            {
                deferral.Complete();
            }
        }

    }
    internal sealed class A
    {
        public static List<IInstaApi> InstaApiList { get; set; } = new List<IInstaApi>();

        public static void OnMessageReceived(object sender, PushReceivedEventArgs e)
        {
            PushHelper.HandleNotify(e.NotificationContent, InstaApiList);
        }
    }

    internal class CS
    {
        internal static DebugLogger DebugLogger;
        public static IInstaApi BuildApi(string username = null, string password = null)
        {
            UserSessionData sessionData;
            if (string.IsNullOrEmpty(username))
                sessionData = UserSessionData.ForUsername("FAKEUSER").WithPassword("FAKEPASS");
            else
                sessionData = new UserSessionData { UserName = username, Password = password };

            DebugLogger = new DebugLogger(LogLevel.All);
            var api = InstaApiBuilder.CreateBuilder()
                      .SetUser(sessionData)
                  //.SetDevice(new UniversalDevice())
#if DEBUG
                  .UseLogger(DebugLogger)
#endif

                      .Build();
#if ARM
            api.SetEncryptedPasswordEncryptor(new PekhoPokh.EncryptedPasswordEncryptor());
#endif
            return api;
        }
        public static readonly StorageFolder LocalFolder = ApplicationData.Current.LocalFolder;
        public List<IInstaApi> InstaApiList { get; set; } = new List<IInstaApi>();

        public async Task Load()
        {
            var users = GetUsersList();
            for (int i = 0; i < users.Count; i++)
            {
                var user = users.ElementAt(i);
                var session = await GetUserSession(user.Key);
                var api = BuildApi();
                await api.LoadStateDataFromStringAsync(session);
                InstaApiList.Add(api);
            }
        }

        public async Task<string> GetUserSession(string userPk)
        {
            var folder = await LocalFolder.CreateFolderAsync("UserSessions", CreationCollisionOption.OpenIfExists);
            try
            {
                var file = await folder.GetFileAsync(userPk);
                var str = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
                return CryptoHelper.DecryptString(str);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Dictionary<string, string> GetUsersList()
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue("UserNames", out var users))
            {
                if (users != null)
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(users.ToString());
                }
            }
            return null;
        }
    }

}
