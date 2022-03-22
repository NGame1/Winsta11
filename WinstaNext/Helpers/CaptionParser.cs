using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace WinstaNext.Helpers
{
    public static class CaptionParser
    {
        public static string RichTextToMarkdownText(this string RichText)
        {
            var pattern = @"\{(.*?)\}";
            //var pattern2 = @"\=(.*?)}";
            var matches = Regex.Matches(RichText, pattern);

            foreach (Match m in matches)
            {
                var matchstr = m.Groups[1].Value;
                if (matchstr.ToLower().Contains("user?"))
                {
                    var username = matchstr.Split('|').FirstOrDefault();
                    RichText = RichText.Replace(m.Captures[0].Value, $"[{username}](@{username})");
                }

            }

            return RichText;
        }

        public static string ToMarkdownText(this string caption)
        {
            if (string.IsNullOrEmpty(caption)) return string.Empty;

            //var hashtags = caption.GetHashtags();
            //var usernames = caption.GetUsernames();

            caption = caption.Replace("\n", Environment.NewLine + Environment.NewLine);
            caption = caption.Replace("\t", string.Empty);

            //Replace Hashtags
            caption = Regex.Replace(Uri.UnescapeDataString(caption),
                @"(?:#)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)", 
                new MatchEvaluator(Replacer));

            //Replace Usernames
            caption = Regex.Replace(Uri.UnescapeDataString(caption),
                @"(?:@)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)",
                new MatchEvaluator(Replacer));

            return caption;
        }

        static string Replacer(Match match)
        {
            return $"[{match.Value}]({match.Value})";
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
