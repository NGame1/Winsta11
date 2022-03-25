﻿using System;
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
            RichText = ToMarkdownText(RichText);

            var pattern = @"\{(.*?)\}";
            //var pattern2 = @"\=(.*?)}";
            RichText = Regex.Replace(RichText, pattern, ReplaceRichText);

            //RichText = Regex.Replace(Uri.UnescapeDataString(RichText),
            //    @"._",
            //    new MatchEvaluator(FixUnrecognizedUnderlines));


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

            //caption = Regex.Replace(Uri.UnescapeDataString(caption),
            //    @"._",
            //    new MatchEvaluator(FixUnrecognizedUnderlines));

            return caption;
        }

        static string FixUnrecognizedUnderlines(Match match)
        {
            var matchstr = match.Value;
            if (matchstr.StartsWith("\\")) return matchstr;
            return matchstr.Replace("_", "\\_");
        }

        static string Replacer(Match match)
        {
            var matchstr = match.Value;
            if (matchstr.Contains("_")) matchstr = matchstr.Replace("_", "\\_");
            return $"[{matchstr}]({match.Value})";
        }

        static string ReplaceRichText(Match match)
        {
            var matchstr = match.Groups[1].Value;
            if (matchstr.ToLower().Contains("user?"))
            {
                var username = matchstr.Split('|').FirstOrDefault();
                var usernameVal = username;
                if (username.Contains("_")) username = username.Replace("_", "\\_");
                var rep = $"[@{username}](@{usernameVal})";
                return rep;
            }
            else
            {
                return matchstr;
            }
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
