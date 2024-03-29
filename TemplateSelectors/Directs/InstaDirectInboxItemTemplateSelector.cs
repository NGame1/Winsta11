﻿using Abstractions.Direct.Models;
using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors
{
    public class InstaDirectInboxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AnimatedGifMessageDataTemplate { get; set; }
        public DataTemplate ClipTemplate { get; set; }
        public DataTemplate IGTVShareTemplate { get; set; }
        public DataTemplate LikeTemplate { get; set; }
        public DataTemplate LinkMessageDataTemplate { get; set; }
        public DataTemplate MediaMessageType { get; set; }
        public DataTemplate MediaShareTemplate { get; set; }
        public DataTemplate XmaMediaShareTemplate { get; set; }
        public DataTemplate NotSupportedMessageType { get; set; }
        public DataTemplate PlaceholderMessageDataTemplate { get; set; }
        public DataTemplate ProfileMessageDataTemplate { get; set; }
        public DataTemplate StoryReplyDataTemplate { get; set; }
        public DataTemplate StoryShareDataTemplate { get; set; }
        public DataTemplate TextMessageDataTemplate { get; set; }
        public DataTemplate VoiceMessageDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not InstaDirectInboxItemFullModel di) return NotSupportedMessageType;
            switch (di.ItemType)
            {

                case InstaDirectThreadItemType.Link:
                    if (di.LinkMedia.LinkContext.LinkUrl == null)
                        return TextMessageDataTemplate;
                    return LinkMessageDataTemplate;

                case InstaDirectThreadItemType.Text:
                    return TextMessageDataTemplate;

                case InstaDirectThreadItemType.VoiceMedia:
                    return VoiceMessageDataTemplate;

                case InstaDirectThreadItemType.Profile:
                    return ProfileMessageDataTemplate;

                case InstaDirectThreadItemType.AnimatedMedia:
                    return AnimatedGifMessageDataTemplate;

                case InstaDirectThreadItemType.Placeholder:
                    return PlaceholderMessageDataTemplate;

                //Shared Video
                case InstaDirectThreadItemType.Media:
                    return MediaMessageType;

                //New Post share template
                case InstaDirectThreadItemType.XmaMediaShare:
                    return XmaMediaShareTemplate;

                //Post shared
                case InstaDirectThreadItemType.MediaShare:
                    return MediaShareTemplate;

                //IGTV
                case InstaDirectThreadItemType.FelixShare:
                    return IGTVShareTemplate;

                //Reels
                case InstaDirectThreadItemType.Clip:
                    return ClipTemplate;


                case InstaDirectThreadItemType.Like:
                    return LikeTemplate;

                case InstaDirectThreadItemType.ReelShare:
                    //Story Reply
                    return StoryReplyDataTemplate;

                case InstaDirectThreadItemType.StoryShare:
                    return StoryShareDataTemplate;

                case InstaDirectThreadItemType.RavenMedia:
                    return NotSupportedMessageType;

                case InstaDirectThreadItemType.ActionLog:
                    return NotSupportedMessageType;

                case InstaDirectThreadItemType.Location:
                    return NotSupportedMessageType;
                case InstaDirectThreadItemType.Hashtag:
                    return NotSupportedMessageType;
                case InstaDirectThreadItemType.LiveViewerInvite:
                    return NotSupportedMessageType;
                case InstaDirectThreadItemType.VideoCallEvent:
                    return NotSupportedMessageType;
                case InstaDirectThreadItemType.ProductShare:
                    return NotSupportedMessageType;
                case InstaDirectThreadItemType.ArEffect:
                    return NotSupportedMessageType;

                default:
                    return NotSupportedMessageType;
            }

        }
    }
}
