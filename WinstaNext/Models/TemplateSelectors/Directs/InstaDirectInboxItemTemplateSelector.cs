using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaNext.Abstractions.Direct.Models;

namespace WinstaNext.Models.TemplateSelectors.Directs
{
    public class InstaDirectInboxItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate AnimatedGifMessageDataTemplate { get; set; }
        public DataTemplate LinkMessageDataTemplate { get; set; }
        public DataTemplate MediaMessageType { get; set; }
        public DataTemplate NotSupportedMessageType { get; set; }
        public DataTemplate PlaceholderMessageDataTemplate { get; set; }
        public DataTemplate ProfileMessageDataTemplate { get; set; }
        public DataTemplate TextMessageDataTemplate { get; set; }
        public DataTemplate VoiceMessageDataTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is not InstaDirectInboxItemFullModel di) return NotSupportedMessageType;
            switch (di.ItemType)
            {

                case InstaDirectThreadItemType.Link:
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

                case InstaDirectThreadItemType.Media:
                    return MediaMessageType;

                case InstaDirectThreadItemType.MediaShare:
                    break;

                case InstaDirectThreadItemType.Like:
                    break;

                case InstaDirectThreadItemType.ReelShare:
                    break;
                case InstaDirectThreadItemType.RavenMedia:
                    break;
                case InstaDirectThreadItemType.StoryShare:
                    break;
                case InstaDirectThreadItemType.ActionLog:
                    break;
                case InstaDirectThreadItemType.Location:
                    break;
                case InstaDirectThreadItemType.FelixShare:
                    break;
                case InstaDirectThreadItemType.Hashtag:
                    break;
                case InstaDirectThreadItemType.LiveViewerInvite:
                    break;
                case InstaDirectThreadItemType.VideoCallEvent:
                    break;
                case InstaDirectThreadItemType.ProductShare:
                    break;
                case InstaDirectThreadItemType.ArEffect:
                    break;
                case InstaDirectThreadItemType.Clip:
                    break;
                default:
                    return NotSupportedMessageType;
            }
            return NotSupportedMessageType;
        }
    }
}
