using InstagramApiSharp.API;
using InstagramApiSharp.Helpers;
using NotificationHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.ApplicationModel.Background;
using Windows.Foundation.Collections;
using Windows.UI.Notifications;
namespace WinstaBackgroundTask
{
    public sealed class NotifyQuickReplyTask : IBackgroundTask
    {
        readonly CS CS = new CS();

        BackgroundTaskDeferral Deferral;
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();
            if (!(taskInstance.TriggerDetails is ToastNotificationActionTriggerDetail details))
            {
                Deferral.Complete();
                //BackgroundTaskStorage.PutError("TriggerDetails was not ToastNotificationActionTriggerDetail.");
                return;
            }

            string arguments = details.Argument;
            if (arguments == "dismiss=True")
            {
                Deferral.Complete();
                return;
            }

            await CS.Load();
            A.InstaApiList = CS.InstaApiList;
            //System.Diagnostics.Debug.WriteLine(arguments);
            //var f = details.UserInput?.FirstOrDefault();
            //if (f == null) return;

            await NotificationActivationHelper.HandleActivationAsync(CS.InstaApiList[0], CS.InstaApiList, arguments, details.UserInput);
            Deferral.Complete();

        }

        //async Task HandleActivation(string args, ValueSet valuePairs)
        //{
        //    try
        //    {
        //        var queries = HttpUtility.ParseQueryString(args, out string type);
        //        if (queries?.Count > 0)
        //        {
        //            var currentUser = queries["currentUser"];
        //            var collapsedKey = queries["collapseKey"];
        //            var sourceUserId = queries["sourceUserId"];

        //            IInstaApi api;
        //            if (CS.InstaApiList.Count == 1)
        //                api = CS.InstaApiList[0];
        //            else
        //            api = CS.InstaApiList.FirstOrDefault(x => x.GetLoggedUser().LoggedInUser.Pk.ToString() == currentUser);

        //            if (api == null)
        //                return;

        //            //comments_v2?media_id=2437384931159496017_44428109093&target_comment_id=17887778494788574&permalink_enabled=True
        //            //direct_v2?id=340282366841710300949128136069129367828&x=29641789960564789887017672389951488
        //            //broadcast?id=17861965853258603&reel_id=1647718432&published_time=1606884766
        //            //media?id=2455052815714850188_1647718432&media_id=2455052815714850188_1647718432
        //            //direct_v2?id=340282366841710300949128136069129367828&x=29641841166106199869991401113518080
        //            //
        //            if (type == "direct_v2")
        //            {
        //                //direct_v2?id=340282366841710300949128136069129367828&x=29641841166106199869991401113518080
        //                //textBox : "00000005544666"
        //                var thread = queries["id"];
        //                var itemId = queries["x"];

        //                if (valuePairs?.Count > 0)
        //                {
        //                    var text = valuePairs["textBox"].ToString();

        //                    await api.MessagingProcessor.SendDirectTextAsync(null, thread, text.Trim());
        //                }
        //            }
        //            else if (collapsedKey == "private_user_follow_request" && queries["action"] is string action)
        //            {
        //                //user?username=ministaapp
        //                // Minista App (@ministaapp) has requested to follow you.
        //                //"user?username=rmtjj73&currentUser=44579170833&sourceUserId=14564882672&
        //                //collapseKey=private_user_follow_request&action=openProfile"
        //                long userPk = -1;
        //                long.TryParse(sourceUserId, out userPk);
        //                if (userPk <= 0)
        //                {
        //                    var userResult = await api.UserProcessor.GetUserAsync(queries["username"]);
        //                    if (!userResult.Succeeded) return;
        //                    userPk = userResult.Value.Pk;
        //                }
        //                if (action == "acceptFriendshipRequest")
        //                    await api.UserProcessor.AcceptFriendshipRequestAsync(userPk);
        //                else if (action == "declineFriendshipRequest")
        //                    await api.UserProcessor.IgnoreFriendshipRequestAsync(userPk);
        //                //else
        //                //    Helper.OpenProfile(userPk);
        //            }
        //        }
        //    }
        //    catch { }
        //}

    }

}
