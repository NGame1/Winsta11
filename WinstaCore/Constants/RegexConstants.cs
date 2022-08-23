namespace WinstaCore.Constants
{
    public class RegexConstants
    {
        public const string WebUrlRegex = @"https?:\/\/(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)";
        public const string WebUrlWithoutPrefixRegex = @"(www\.)?[-a-zA-Z0-9@:%._\+~#=]{1,256}\.[a-zA-Z0-9()]{1,6}\b([-a-zA-Z0-9()!@:%_\+.~#?&\/\/=]*)";

        public const string HashtagsRegex = @"(?:#)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)";
        public const string UsernamesRegex = @"(?:@)([A-Za-z\u0600-\u06FF0-9_](?:(?:[A-Za-z\u0600-\u06FF0-9_]|(?:\.(?!\.))){0,28}(?:[A-Za-z\u0600-\u06FF0-9_]))?)";
    }
}
