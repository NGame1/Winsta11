using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;

namespace NotificationHandler
{
    static public class NotificationActivationHelper
    {
        public static async void HandleActivation(IInstaApi defaultApi, List<IInstaApi> apiList, string args,
            ValueSet valuePairs, bool wait = false,
            Action<long> profileAction = null, Action<string> liveAction = null,
            Action<string, InstaUserShortFriendship> threadAction = null, Action<string> commentAction = null,
            Action<InstaMedia> tvAction = null)
            =>
            await HandleActivationAsync(defaultApi, apiList, args, valuePairs, wait,
                profileAction, liveAction, threadAction, commentAction, tvAction);

        public static async Task HandleActivationAsync(IInstaApi defaultApi, List<IInstaApi> apiList, string args,
            ValueSet valuePairs, bool wait = false,
            Action<long> profileAction = null, Action<string> liveAction = null,
            Action<string, InstaUserShortFriendship> threadAction = null, Action<string> commentAction = null,
            Action<InstaMedia> tvAction = null)
        {
            try
            {
                if (wait)
                    await Task.Delay(7500); // Wait for loading information

                if (args == "dismiss=True") return;

                var queries = HttpUtility.ParseQueryString(args, out string type);
                if (queries?.Count > 0)
                {
                    var action = queries.GetValueIfPossible("action");
                    var currentUser = queries.GetValueIfPossible("currentUser");
                    var collapsedKey = queries.GetValueIfPossible("collapseKey");
                    var sourceUserId = queries.GetValueIfPossible("sourceUserId");
                    var pushCategory = queries.GetValueIfPossible("pushCategory");
                    var id = queries.GetValueIfPossible("id")?.Trim();
                    var itemId = queries.GetValueIfPossible("x");

                    IInstaApi api;
                    if (apiList?.Count > 1)
                        api = apiList.FirstOrDefault(x => x.GetLoggedUser().LoggedInUser.Pk.ToString() == currentUser);
                    else
                        api = defaultApi;

                    if (api == null) return;

                    //comments_v2?media_id=2437384931159496017_44428109093&target_comment_id=17887778494788574&permalink_enabled=True
                    //direct_v2?id=340282366841710300949128136069129367828&x=29641789960564789887017672389951488
                    //broadcast?id=17861965853258603&reel_id=1647718432&published_time=1606884766
                    //media?id=2455052815714850188_1647718432&media_id=2455052815714850188_1647718432
                    //direct_v2?id=340282366841710300949128136069129367828&x=29641841166106199869991401113518080
                    if (type == "direct_v2")
                    {
                        //direct_v2?id=340282366841710300949128136069129367828&x=29641841166106199869991401113518080
                        if (string.IsNullOrEmpty(pushCategory) || pushCategory == "direct_v2_message") // messaging
                        {
                            //textBox : "00000005544666"
                            if (valuePairs?.Count > 0)
                            {
                                var text = valuePairs["textBox"].ToString();

                                await api.MessagingProcessor.SendDirectTextAsync(null, id, text.Trim());

                            }
                        }
                        else if (pushCategory == "direct_v2_pending")// pending requests
                        {
                            // Accept   Delete   Block  Dismiss
                            long userPk = await GetUserId(api, sourceUserId, null);
                            if (userPk == -1) return;

                            if (action == "acceptDirectRequest")
                                await api.MessagingProcessor.ApproveDirectPendingRequestAsync(id);
                            else if (action == "deleteDirectRequest")
                                await api.MessagingProcessor.DeclineDirectPendingRequestsAsync(id);
                            else if (action == "blockDirectRequest")
                            {
                                await api.MessagingProcessor.DeclineDirectPendingRequestsAsync(id);
                                await api.UserProcessor.BlockUserAsync(userPk);
                            }
                            else //openPendingThread
                            {
                                var userInfo = await api.UserProcessor.GetUserInfoByIdAsync(userPk);
                                if (!userInfo.Succeeded) return;
                                var u = userInfo.Value;
                                var userShortFriendship = new InstaUserShortFriendship
                                {
                                    UserName = u.UserName,
                                    Pk = u.Pk,
                                    ProfilePicture = u.ProfilePicture,
                                    ProfilePicUrl = u.ProfilePicUrl,
                                    IsPrivate = u.IsPrivate,
                                    IsBestie = u.IsBestie,
                                    IsVerified = u.IsVerified,
                                    FullName = u.FullName,
                                };
                                if (u.FriendshipStatus != null)
                                    userShortFriendship.FriendshipStatus = new InstaFriendshipShortStatus
                                    {
                                        Following = u.FriendshipStatus.Following,
                                        IncomingRequest = u.FriendshipStatus.IncomingRequest,
                                        IsBestie = u.FriendshipStatus.IsBestie,
                                        IsPrivate = u.FriendshipStatus.IsPrivate,
                                        OutgoingRequest = u.FriendshipStatus.OutgoingRequest,
                                        Pk = u.Pk
                                    };
                                else
                                    userShortFriendship.FriendshipStatus = new InstaFriendshipShortStatus
                                    {
                                        Pk = u.Pk
                                    };
                                threadAction?.Invoke(id, userShortFriendship);
                            }
                        }
                    }
                    else if (collapsedKey == "private_user_follow_request")
                    {
                        //user?username=ministaapp
                        // Minista App (@ministaapp) has requested to follow you.
                        //"user?username=rmtjj73&currentUser=44579170833&sourceUserId=14564882672&
                        //collapseKey=private_user_follow_request&action=openProfile"
                        long userPk = await GetUserId(api, sourceUserId, queries["username"]);
                        if (userPk == -1) return;
                        if (action == "acceptFriendshipRequest")
                            await api.UserProcessor.AcceptFriendshipRequestAsync(userPk);
                        else if (action == "declineFriendshipRequest")
                            await api.UserProcessor.IgnoreFriendshipRequestAsync(userPk);
                        else
                            profileAction?.Invoke(userPk);

                    }
                    else if (type == "broadcast" && collapsedKey == "live_broadcast")
                    {
                        //broadcast?id=18035667694304049&reel_id=1647718432&published_time=1607056892
                        liveAction?.Invoke(id);
                    }
                    else if (collapsedKey == "post")
                    {
                        // media?id=2455052815714850188_1647718432&media_id=2455052815714850188_1647718432
                        if (action == "likeMedia")
                            await api.MediaProcessor.LikeMediaAsync(id);
                        else if (action == "commentMedia" && valuePairs?.Count > 0)
                        {
                            var text = valuePairs["textBox"].ToString();

                            await api.CommentProcessor.CommentMediaAsync(id, text.Trim());
                        }
                        else
                            commentAction?.Invoke(id);
                    }
                    else if (collapsedKey == "comment")
                    {
                        var mediaId = id ?? queries.GetValueIfPossible("media_id");
                        var targetMediaId = queries.GetValueIfPossible("target_comment_id");

                        if (string.IsNullOrEmpty(mediaId)) return;
                        if (action != "openComment" && string.IsNullOrEmpty(targetMediaId)) return;
                        // comments_v2?media_id=2450763156807842703_44428109093&target_comment_id=17915232835518492&permalink_enabled=True
                        if (action == "likeComment")
                            await api.CommentProcessor.LikeCommentAsync(targetMediaId);
                        else if (action == "commentMedia" && valuePairs?.Count > 0)
                        {
                            var text = valuePairs["textBox"].ToString();

                            await api.CommentProcessor.ReplyCommentMediaAsync(mediaId, targetMediaId, text.Trim());
                        }
                        else
                            commentAction?.Invoke(mediaId);
                    }
                    else if (collapsedKey == "subscribed_igtv_post")
                    {
                        if (string.IsNullOrEmpty(id)) return;
                        // tv_viewer?id=2457476214378560971
                        var mediaInfo = await api.MediaProcessor.GetMediaByIdAsync(id);
                        if (!mediaInfo.Succeeded) return;
                        var mediaId = mediaInfo.Value.InstaIdentifier;
                        if (action == "likeMedia")
                            await api.CommentProcessor.LikeCommentAsync(mediaId);
                        else if (action == "commentMedia" && valuePairs?.Count > 0)
                        {
                            var text = valuePairs["textBox"].ToString();
                            await api.CommentProcessor.CommentMediaAsync(mediaId, text.Trim());
                        }
                        else
                            tvAction?.Invoke(mediaInfo.Value);
                    }
                    else if (collapsedKey == "two_factor_trusted_notification")
                    {
                        //push.IgAction += $"&location={push.Location}";
                        //push.IgAction += $"&deviceId={push.DeviceId}";
                        //push.IgAction += $"&longitude={push.Longitude}";
                        //push.IgAction += $"&latitude={push.Latitude}";
                        //push.IgAction += $"&deviceName={push.DeviceName}";
                        //push.IgAction += $"&timezoneOffset={push.TimezoneOffset}";
                        //push.IgAction += $"&twoFactorIdentifier={push.TwoFactorIdentifier}";
                        //push.IgAction += $"&originalTime={push.OriginalTime}";
                        var twoFactorIdentifier = Uri.UnescapeDataString(queries.GetValueIfPossible("twoFactorIdentifier"));
                        var deviceId = queries.GetValueIfPossible("deviceId");
                        //var deviceName = queries.GetValueIfPossible("deviceName");
                        Debug.WriteLine(twoFactorIdentifier);
                        Debug.WriteLine(deviceId);

                        await api.AccountProcessor.CheckNewLoginRequestNotificationAsync(twoFactorIdentifier, deviceId);
                        //&action=approveLoginRequest
                        //&action=denyLoginRequest
                        if (action == "approveLoginRequest")
                            await api.AccountProcessor.ApproveNewLoginRequestAsync(twoFactorIdentifier, deviceId);
                        else if (action == "denyLoginRequest")
                            await api.AccountProcessor.DenyNewLoginRequestAsync(twoFactorIdentifier, deviceId);
                    }
                }
            }
            catch { }
        }
        static async Task<long> GetUserId(IInstaApi api, string sourceUserId, string username)
        {
            long.TryParse(sourceUserId, out long userPk);
            if (userPk <= 0)
            {
                if (string.IsNullOrEmpty(username)) return -1;
                var userResult = await api.UserProcessor.GetUserAsync(username);
                if (!userResult.Succeeded) return -1;
                userPk = userResult.Value.Pk;
            }
            return userPk;
        }
        static string GetValueIfPossible(this Dictionary<string, string> keyValuePairs, string key)
        {
            if (keyValuePairs.Any(x => x.Key == key))
                return keyValuePairs[key];
            else
                return null;
        }
    }
}
