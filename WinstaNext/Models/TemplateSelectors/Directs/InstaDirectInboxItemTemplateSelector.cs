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
        public DataTemplate ClipTemplate { get; set; }
        public DataTemplate IGTVShareTemplate { get; set; }
        public DataTemplate LikeTemplate { get; set; }
        public DataTemplate LinkMessageDataTemplate { get; set; }
        public DataTemplate MediaMessageType { get; set; }
        public DataTemplate MediaShareTemplate { get; set; }
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

                    //Shared Video
                case InstaDirectThreadItemType.Media:
                    return MediaMessageType;

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
                    break;
                case InstaDirectThreadItemType.RavenMedia:
                    break;
                case InstaDirectThreadItemType.StoryShare:
                    break;
                case InstaDirectThreadItemType.ActionLog:
                    break;
                case InstaDirectThreadItemType.Location:
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

                default:
                    return NotSupportedMessageType;
            }
            return NotSupportedMessageType;
        }
    }
}
