﻿/*
 * Created by Ramtin Jokar [ Ramtinak@live.com ] [ https://t.me/ramtinak ]
 * Donation link: [ https://paypal.me/rmt4006 ] 
 * Donation email: RamtinJokar@outlook.com
 * 
 * Copyright (c) 2020 Summer [ Tabestaan 1399 ]
 */

using InstagramApiSharp.Classes.ResponseWrappers;
using System.Collections.Generic;
using System.Linq;

namespace WinstaBackgroundHelpers
{
    public class RealtimeHelper
    {
        internal static readonly Dictionary<string, string> Topics = new Dictionary<string, string>
        {
            {"0", "/buddy_list"},
            {"1", "/create_thread"},
            {"2", "/create_thread_response"},
            {"3", "/delete_thread_notification"},
            {"4", "/delete_messages_notification"},
            {"5", "/orca_message_notifications"},
            {"6", "/friending_state_change"},
            {"7", "/friend_request"},
            {"8", "/friend_requests_seen"},
            {"9", "/graphql"},
            {"10", "/group_msg"},
            {"11", "/group_notifs_unseen"},
            {"12", "/group_msgs_unseen"},
            {"13", "/inbox"},
            {"14", "/action_id_notification"},
            {"15", "/aura_notification"},
            {"16", "/aura_signal"},
            {"17", "/friends_locations"},
            {"18", "/mark_thread"},
            {"19", "/mark_thread_response"},
            {"20", "/mercury"},
            {"21", "/messenger_sync"},
            {"22", "/messenger_sync_ack"},
            {"23", "/messenger_sync_create_queue"},
            {"24", "/messenger_sync_get_diffs"},
            {"25", "/messaging"},
            {"26", "/messaging_events"},
            {"27", "/mobile_requests_count"},
            {"28", "/mobile_video_encode"},
            {"29", "/orca_notification_updates"},
            {"30", "/notifications_sync"},
            {"31", "/notifications_read"},
            {"32", "/notifications_seen"},
            {"33", "/push_notification"},
            {"34", "/pp"},
            {"35", "/orca_presence"},
            {"36", "/privacy_changed"},
            {"37", "/privacy_updates"},
            {"38", "/send_additional_contacts"},
            {"39", "/send_chat_event"},
            {"40", "/send_delivery_receipt"},
            {"41", "/send_endpoint_capabilities"},
            {"42", "/foreground_state"},
            {"43", "/aura_location"},
            {"44", "/send_location"},
            {"45", "/send_message2"},
            {"46", "/send_message"},
            {"47", "/send_message_response"},
            {"48", "/ping"},
            {"49", "/presence"},
            {"50", "/send_push_notification_ack"},
            {"51", "/rich_presence"},
            {"52", "/send_skype"},
            {"53", "/typing"},
            {"54", "/set_client_settings"},
            {"55", "/shoerack_notifications"},
            {"56", "/orca_ticker_updates"},
            {"57", "/orca_typing_notifications"},
            {"58", "/typ"},
            {"59", "/t_ms"},
            {"60", "/orca_video_notifications"},
            {"61", "/orca_visibility_updates"},
            {"62", "/webrtc"},
            {"63", "/webrtc_response"},
            {"64", "/subscribe"},
            {"65", "/t_p"},
            {"66", "/push_ack"},
            {"68", "/webrtc_binary"},
            {"69", "/t_sm"},
            {"70", "/t_sm_rp"},
            {"71", "/t_vs"},
            {"72", "/t_rtc"},
            {"73", "/echo"},
            {"74", "/pages_messaging"},
            {"75", "/t_omnistore_sync"},
            {"76", "/fbns_msg"},
            {"77", "/t_ps"},
            {"78", "/t_dr_batch"},
            {"79", "/fbns_reg_req"},
            {"80", "/fbns_reg_resp"},
            {"81", "/omnistore_subscribe_collection"},
            {"82", "/fbns_unreg_req"},
            {"83", "/fbns_unreg_resp"},
            {"84", "/omnistore_change_record"},
            {"85", "/t_dr_response"},
            {"86", "/quick_promotion_refresh"},
            {"87", "/v_ios"},
            {"88", "/pubsub"},
            {"89", "/get_media"},
            {"90", "/get_media_resp"},
            {"91", "/mqtt_health_stats"},
            {"92", "/t_sp"},
            {"93", "/groups_landing_updates"},
            {"94", "/rs"},
            {"95", "/t_sm_b"},
            {"96", "/t_sm_b_rsp"},
            {"97", "/t_ms_gd"},
            {"98", "/t_rtc_multi"},
            {"99", "/friend_accepted"},
            {"100", "/t_tn"},
            {"101", "/t_mf_as"},
            {"102", "/t_fs"},
            {"103", "/t_tp"},
            {"104", "/t_stp"},
            {"105", "/t_st"},
            {"106", "/omni"},
            {"107", "/t_push"},
            {"108", "/omni_c"},
            {"109", "/t_sac"},
            {"110", "/omnistore_resnapshot"},
            {"111", "/t_spc"},
            {"112", "/t_callability_req"},
            {"113", "/t_callability_resp"},
            {"116", "/t_ec"},
            {"117", "/t_tcp"},
            {"118", "/t_tcpr"},
            {"119", "/t_ts"},
            {"120", "/t_ts_rp"},
            {"121", "/t_mt_req"},
            {"122", "/t_mt_resp"},
            {"123", "/t_inbox"},
            {"124", "/p_a_req"},
            {"125", "/p_a_resp"},
            {"126", "/unsubscribe"},
            {"127", "/t_graphql_req"},
            {"128", "/t_graphql_resp"},
            {"129", "/t_app_update"},
            {"130", "/p_updated"},
            {"131", "/t_omnistore_sync_low_pri"},
            {"132", "/ig_send_message"},
            {"133", "/ig_send_message_response"},
            {"134", "/ig_sub_iris"},
            {"135", "/ig_sub_iris_response"},
            {"136", "/ig_snapshot_response"},
            {"137", "/fbns_msg_hp"},
            {"138", "/data_stream"},
            {"139", "/opened_thread"},
            {"140", "/t_typ_att"},
            {"141", "/iris_server_reset"},
            {"142", "/flash_thread_presence"},
            {"143", "/flash_send_thread_presence"},
            {"144", "/flash_thread_typing"},
            {"146", "/ig_message_sync"},
            {"148", "/t_omnistore_batched_message"},
            {"149", "/ig_realtime_sub"},
            {"150", "/t_region_hint"},
            {"151", "/t_fb_family_navigation_badge"},
            {"152", "/t_ig_family_navigation_badge"},
            {"153", "/parties_notifications"},
            {"154", "/t_assist"},
            {"155", "/t_assist_rp"},
            {"156", "/t_create_group"},
            {"157", "/t_create_group_rp"},
            {"158", "/t_create_group_ms"},
            {"159", "/t_create_group_ms_rp"},
            {"160", "/t_entity_presence"},
            {"161", "/ig_region_hint_rp"},
            {"162", "/buddylist_overlay"},
            {"163", "/setup_debug"},
            {"164", "/ig_conn_update"},
            {"165", "/ig_msg_dr"},
            {"166", "/parties_notifications_req"},
            {"167", "/omni_connect_sync"},
            {"168", "/parties_send_message"},
            {"169", "/parties_send_message_response"},
            {"170", "/omni_connect_sync_req"},
            {"172", "/br_sr"},
            {"174", "/sr_res"},
            {"175", "/omni_connect_sync_batch"},
            {"176", "/notify_disconnect"},
            {"177", "/omni_mc_ep_push_req"},
            {"180", "/fbns_msg_ack"},
            {"181", "/t_add_participants_to_group"},
            {"182", "/t_add_participants_to_group_rp"},
            {"188", "/t_aloha_session_req"},
            {"195", "/t_thread_typing"},
            {"201", "/video_rt_pipe"},
            {"202", "/t_update_presence_extra_data"},
            {"203", "/video_rt_pipe_res"},
            {"211", "/onevc"},
            {"231", "/fbns_exp_logging"},
       };
    }
    public class RealtimeResponseList : List<InstaDirectRespondResponse>
    {
        public void AddItem(InstaDirectRespondResponse item)
        {
            var flag = this.Any(x => x.Payload?.ClientContext == item.Payload.ClientContext);
            if (!flag)
                Add(item);
        }
        public InstaDirectRespondResponse GetItem(string token) => this.FirstOrDefault(x => x.Payload?.ClientContext == token);

        public void RemoveItem(string token)
        {
            var item = GetItem(token);
            if (item != null)
                Remove(item);
        }
    }
}
