using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Converters;
using InstagramApiSharp.Helpers;
using WinstaBackgroundHelpers.Mqtt.Packets;
using WinstaBackgroundHelpers.Push.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Cryptography;

namespace InstagramApiSharp.API.RealTime
{
    public partial class RealtimeClient
    {
        /// <summary>
        ///     Send direct text message to threads
        /// </summary>
        /// <param name="threadId">Message thread ids</param>
        /// <param name="text">Message text</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectTextAsync(string threadId, string text)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "text"},
                {"text", text},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send direct text message to recipient users
        /// </summary>
        /// <param name="recipientId">Comma-separated users PK</param>
        /// <param name="text">Message text</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectTextToRecipientAsync(string recipientId, string text)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "text"},
                {"text", text},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send link address to direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="text">Text to send</param>
        /// <param name="link">Link to send (only one link will approved)</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLinkAsync(string threadId, string text, string link)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "link"},
                {"link_text", text ?? string.Empty},
                {"link_urls", ($"[{ExtensionHelper.EncodeList(new[]{ link })}]")},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send link address to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="text">Text to send</param>
        /// <param name="link">Link to send (only one link will approved)</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLinkToRecipientsAsync(string recipientId, string text, string link)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "link"},
                {"link_text", text ?? string.Empty},
                {"link_urls", ($"[{ExtensionHelper.EncodeList(new[]{ link })}]")},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send location to direct thread
        /// </summary>
        /// <param name="threadId">Thread ids</param>
        /// <param name="externalId">External id (get it from <seealso cref="Processors.ILocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLocationAsync(string threadId, string externalId)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "location"},
                {"venue_id", externalId},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send location to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="externalId">External id (get it from <seealso cref="Processors.ILocationProcessor.SearchLocationAsync"/></param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLocationToRecipientsAsync(string recipientId, string externalId)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "location"},
                {"venue_id", externalId},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send profile to direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="userIdToSend">User id to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectProfileAsync(string threadId, long userIdToSend)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "profile"},
                {"profile_user_id", userIdToSend.ToString()},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send profile to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="userIdToSend">User id to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectProfileToRecipientsAsync(string recipientId, long userIdToSend)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "profile"},
                {"profile_user_id", userIdToSend.ToString()},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Share media to direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="mediaId">Media id</param>
        /// <param name="text">Text to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> ShareMediaToThreadAsync(string threadId, string mediaId, string text = null)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "media_share"},
                {"media_id", mediaId},
                {"unified_broadcast_format", "1"},
                {"text", text ?? string.Empty}
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Share media to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="mediaId">Media id</param>
        /// <param name="text">Text to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> ShareMediaToUserAsync(string recipientId, string mediaId, string text = null)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "media_share"},
                {"media_id", mediaId},
                {"unified_broadcast_format", "1"},
                {"text", text ?? string.Empty}
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send felix share (ig tv) to direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="mediaId">Media identifier to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectFelixShareAsync(string threadId, string mediaId)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "felix_share"},
                {"media_id", mediaId},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send felix share (ig tv) to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="mediaId">Media identifier to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectFelixShareToRecipientsAsync(string recipientId, string mediaId)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "felix_share"},
                {"media_id", mediaId},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send hashtag to direct thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="text">Text to send</param>
        /// <param name="hashtag">Hashtag to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectHashtagAsync(string threadId, string text, string hashtag)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "hashtag"},
                {"hashtag", hashtag},
                {"text", text ?? string.Empty},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send hashtag to recipients
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        /// <param name="text">Text to send</param>
        /// <param name="hashtag">Hashtag to send</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectHashtagToRecipientsAsync(string recipientId, string text, string hashtag)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "hashtag"},
                {"hashtag", hashtag},
                {"text", text ?? string.Empty},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send a like to the conversation
        /// </summary>
        /// <param name="threadId">Thread id</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLikeAsync(string threadId)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "like"},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Send a like to the conversation
        /// </summary>
        /// <param name="recipientId">Recipient id</param>
        public async Task<IResult<InstaDirectRespondPayload>> SendDirectLikeToRecipientsAsync(string recipientId)
        {
            var data = new Dictionary<string, string>
            {
                {"recipient_users",  "[[" + recipientId + "]]"},
                {"item_type", "like"},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Mark direct message as seen
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Message id (item id)</param>
        public async Task<IResult<InstaDirectRespondPayload>> MarkDirectThreadAsSeenAsync(string threadId, string itemId)
        {
            var data = new Dictionary<string, string>
            {
                {"thread_id",  threadId},
                {"item_type", "mark_seen"},
                {"item_id", itemId},
            };
            return await SendDirectItem(data);
        }
        /// <summary>
        ///     Like a message in thread
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="itemId">Item (message) id</param>
        public async Task<IResult<bool>> LikeThreadMessageAsync(string threadId, string itemId)
        {
            try
            {
                var token = ExtensionHelper.GetThreadToken();
                var data = new Dictionary<string, string>
                {
                    {"action", "send_item"},
                    {"item_type", "reaction"},
                    {"reaction_type", "like"},
                    {"node_type", "item"},
                    {"reaction_status", "created"},
                    {"thread_id", threadId},
                    {"client_context", token},
                    {"item_id", itemId},
                };
                var json = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
                {
                    Payload = ZlibHelper.Compress(bytes).AsBuffer(),
                    PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                    TopicName = "132"
                };
                await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);

                return Result.Success(true);
            }
            catch (SocketException socketException)
            {
                return Result.Fail(socketException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception);
            }
        }
        /// <summary>
        ///     Indicate activity
        /// </summary>
        /// <param name="threadId">Thread id</param>
        /// <param name="isActive">Active status</param>
        public async Task<IResult<bool>> IndicateActivityAsync(string threadId, bool isActive)
        {
            try
            {
                var token = ExtensionHelper.GetThreadToken();

                var data = new Dictionary<string, string>
                {
                    {"action", "indicate_activity"},
                    {"item_type", "indicate_activity"},
                    {"thread_id", threadId},
                    {"client_context", token},
                    {"activity_status", isActive ? "1" : "0"},
                };
                var json = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
                {
                    Payload = ZlibHelper.Compress(bytes).AsBuffer(),
                    PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                    TopicName = "132"
                };
                await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);

                return Result.Success(true);

            }
            catch (SocketException socketException)
            {
                return Result.Fail(socketException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception);
            }
        }

        public async Task<IResult<bool>> ReactionMessageAsync(string threadId, string itemId, string emoji)
        {
            try
            {
                //{"action":"item_ack","status_code":"404","payload":{"client_context":"6687658745131972483","message":"target item is not supported"},"status":"fail"}

                //{"action":"item_ack","status_code":"400","payload":{"client_context":"6685052289622163080","message":"unknown reaction type"},"status":"fail"}
                var token = ExtensionHelper.GetThreadToken();
                var data = new Dictionary<string, string>
                {
                    {"action", "send_item"},
                    {"item_type", "reaction"},
                    {"reaction_type", "like"},
                    {"node_type", "item"},
                    {"reaction_status", "created"},
                    {"thread_id", threadId},
                    {"client_context", token},
                    {"item_id", itemId},
                    {"emoji", emoji},
                };
                var json = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(json);
                var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
                {
                    Payload = ZlibHelper.Compress(bytes).AsBuffer(),
                    PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                    TopicName = "132"
                };
                await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);

                return Result.Success(true);
            }
            catch (SocketException socketException)
            {
                return Result.Fail(socketException, default(bool), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<bool>(exception);
            }
        }
        private async Task<IResult<InstaDirectRespondPayload>> SendDirectItem(Dictionary<string, string> dic, string token = null)
        {
            try
            {
                if (string.IsNullOrEmpty(token))
                    token = ExtensionHelper.GetThreadToken();

                var data = new Dictionary<string, string>
                {
                    {"action", "send_item"},
                    {"client_context", token},
                };
                foreach (var item in dic)
                    data.Add(item.Key, item.Value);
                var json = JsonConvert.SerializeObject(data);
                var bytes = Encoding.UTF8.GetBytes(json);

                //id: '132',
                //path: '/ig_send_message',
                var publishPacket = new PublishPacket(QualityOfService.AtLeastOnce, false, false)
                {
                    Payload = ZlibHelper.Compress(bytes).AsBuffer(),
                    PacketId = (ushort)CryptographicBuffer.GenerateRandomNumber(),
                    TopicName = "132"
                };
                await FbnsPacketEncoder.EncodePacket(publishPacket, _outboundWriter);
                await Task.Delay(WaitForResponseDelay);

                var responseItem = Responses.GetItem(token);
                if (responseItem != null)
                {
                    Responses.Remove(responseItem);
                    return responseItem.IsSucceed ?
                        Result.Success(ConvertersFabric.Instance.GetDirectRespondConverter(responseItem).Convert().Payload) :
                        Result.Fail(responseItem.Payload?.Message, ConvertersFabric.Instance
                        .GetDirectRespondConverter(responseItem).Convert().Payload);
                }
                else
                    return Result.Fail<InstaDirectRespondPayload>("Couldn't get any response in the waiting time...\nMessage might sent after this period");
            }
            catch (SocketException socketException)
            {
                return Result.Fail(socketException, default(InstaDirectRespondPayload), ResponseType.NetworkProblem);
            }
            catch (Exception exception)
            {
                return Result.Fail<InstaDirectRespondPayload>(exception);
            }
        }

    }
}
