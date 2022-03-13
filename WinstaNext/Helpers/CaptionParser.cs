using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WinstaNext.Helpers
{
    public static class CaptionParser
    {
        public static string ToMarkdownText(this string caption)
        {
            if (string.IsNullOrEmpty(caption)) return string.Empty;

            var hashtags = caption.GetHashtags().GroupBy(x => x.Value).Select(x => x.Key).ToArray();
            var usernames = caption.GetUsernames().GroupBy(x => x.Value).Select(x => x.Key).ToArray();

            for (int i = 0; i < hashtags.Length; i++)
            {
                var h = hashtags.ElementAt(i);
                caption = caption.Replace(h, $"[{h}]({h})");
            }
            for (int i = 0; i < usernames.Length; i++)
            {
                var u = usernames.ElementAt(i);
                caption = caption.Replace(u, $"[{u}]({u})");
            }

            return caption;
        }

        public static MatchCollection GetHashtags(this string CaptionText)
           => Regex.Matches(Uri.UnescapeDataString(CaptionText), @"(?:#)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)");

        public static MatchCollection GetUsernames(this string CaptionText)
            => Regex.Matches(Uri.UnescapeDataString(CaptionText), @"(?:@)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)");

        public static bool IsRightToLeft(this string strCompare)
        {
            if (string.IsNullOrEmpty(strCompare)) return false;
            char[] chars = strCompare.ToCharArray();
            foreach (char ch in chars)
                if (ch >= '\u0600' && ch <= '\u06FF') return true;
            return false;
        }
    }
}
