using Windows.ApplicationModel.Resources;

namespace WinstaNext
{
    public class LanguageManager
    {
        public static LanguageManager Instance { get; }
        public GeneralStrings General { get; } = new();
        public InstagramStrings Instagram { get; } = new();
        public SettingsStrings Settings { get; } = new();
        public UnitsStrings Units { get; } = new();

        private LanguageManager() { }
        static LanguageManager()
        {
            Instance = new LanguageManager();
        }
    }

    public class GeneralStrings
    {
        ResourceLoader _resource = new("General");

        public string ApplicationName { get => _resource.GetString(nameof(ApplicationName)); }
        public string AppTheme { get => _resource.GetString(nameof(AppTheme)); }
        public string AppThemeDescription { get => _resource.GetString(nameof(AppThemeDescription)); }
        public string Call { get => _resource.GetString(nameof(Call)); }
        public string Cancel { get => _resource.GetString(nameof(Cancel)); }
        public string Close { get => _resource.GetString(nameof(Close)); }
        public string Copy { get => _resource.GetString(nameof(Copy)); }
        public string Dark { get => _resource.GetString(nameof(Dark)); }
        public string Download { get => _resource.GetString(nameof(Download)); }
        public string Email { get => _resource.GetString(nameof(Email)); }
        public string Error { get => _resource.GetString(nameof(Error)); }
        public string Find { get => _resource.GetString(nameof(Find)); }
        public string Home { get => _resource.GetString(nameof(Home)); }
        public bool IsRightToLeft { get => bool.Parse(_resource.GetString(nameof(IsRightToLeft))); }
        public string Language { get => _resource.GetString(nameof(Language)); }
        public string LanguageDescription { get => _resource.GetString(nameof(LanguageDescription)); }
        public string Light { get => _resource.GetString(nameof(Light)); }
        public string Loading { get => _resource.GetString(nameof(Loading)); }
        public string Login { get => _resource.GetString(nameof(Login)); }
        public string Message { get => _resource.GetString(nameof(Message)); }
        public string More { get => _resource.GetString(nameof(More)); }
        public string Off { get => _resource.GetString(nameof(Off)); }
        public string Ok { get => _resource.GetString(nameof(Ok)); }
        public string No { get => _resource.GetString(nameof(No)); }
        public string On { get => _resource.GetString(nameof(On)); }
        public string Password { get => _resource.GetString(nameof(Password)); }
        public string Phone { get => _resource.GetString(nameof(Phone)); }
        public string Register { get => _resource.GetString(nameof(Register)); }
        public string Search { get => _resource.GetString(nameof(Search)); }
        public string SearchPlaceHolder { get => _resource.GetString(nameof(SearchPlaceHolder)); }
        public string Settings { get => _resource.GetString(nameof(Settings)); }
        public string System { get => _resource.GetString(nameof(System)); }
        public string UserIdentifier { get => _resource.GetString(nameof(UserIdentifier)); }
        public string Username { get => _resource.GetString(nameof(Username)); }
        public string Verify { get => _resource.GetString(nameof(Verify)); }
        public string Warning { get => _resource.GetString(nameof(Warning)); }
        public string Yes { get => _resource.GetString(nameof(Yes)); }

        internal GeneralStrings() { }
    }

    public class InstagramStrings
    {
        ResourceLoader _resource = new("Instagram");

        public string Accounts { get => _resource.GetString(nameof(Accounts)); }
        public string Activities { get => _resource.GetString(nameof(Activities)); }
        public string AddAccount { get => _resource.GetString(nameof(AddAccount)); }
        public string Archive { get => _resource.GetString(nameof(Archive)); }
        public string Biography { get => _resource.GetString(nameof(Biography)); }
        public string Comment { get => _resource.GetString(nameof(Comment)); }
        public string Comments { get => _resource.GetString(nameof(Comments)); }
        public string CommentPlaceholder { get => _resource.GetString(nameof(CommentPlaceholder)); }
        public string CopyCaption { get => _resource.GetString(nameof(CopyCaption)); }
        public string CopyURL { get => _resource.GetString(nameof(CopyURL)); }
        public string DeletePost { get => _resource.GetString(nameof(DeletePost)); }
        public string Directs { get => _resource.GetString(nameof(Directs)); }
        public string DisableCommenting { get => _resource.GetString(nameof(DisableCommenting)); }
        public string EditProfile { get => _resource.GetString(nameof(EditProfile)); }
        public string EditPost { get => _resource.GetString(nameof(EditPost)); }
        public string EnableCommenting { get => _resource.GetString(nameof(EnableCommenting)); }
        public string Explore { get => _resource.GetString(nameof(Explore)); }
        public string FailureToastNotification { get => _resource.GetString(nameof(FailureToastNotification)); }
        public string Feed { get => _resource.GetString(nameof(Feed)); }
        public string Follow { get => _resource.GetString(nameof(Follow)); }
        public string FollowBack { get => _resource.GetString(nameof(FollowBack)); }
        public string Followers { get => _resource.GetString(nameof(Followers)); }
        public string Following { get => _resource.GetString(nameof(Following)); }
        public string Followings { get => _resource.GetString(nameof(Followings)); }
        public string Hashtags { get => _resource.GetString(nameof(Hashtags)); }
        public string Highlights { get => _resource.GetString(nameof(Highlights)); }
        public string IGTV { get => _resource.GetString(nameof(IGTV)); }
        public string IsFollowingYou { get => _resource.GetString(nameof(IsFollowingYou)); }
        public string Like { get => _resource.GetString(nameof(Like)); }
        public string Likes { get => _resource.GetString(nameof(Likes)); }
        public string LoadMoreComments { get => _resource.GetString(nameof(LoadMoreComments)); }
        public string MediaComments { get => _resource.GetString(nameof(MediaComments)); }
        public string MediaLikers { get => _resource.GetString(nameof(MediaLikers)); }
        public string MessagePlaceholder { get => _resource.GetString(nameof(MessagePlaceholder)); }
        public string MutingOptions { get => _resource.GetString(nameof(MutingOptions)); }
        public string MutePosts { get => _resource.GetString(nameof(MutePosts)); }
        public string MuteStories { get => _resource.GetString(nameof(MuteStories)); }
        public string NoDirectsYet { get => _resource.GetString(nameof(NoDirectsYet)); }
        public string Places { get => _resource.GetString(nameof(Places)); }
        public string Posts { get => _resource.GetString(nameof(Posts)); }
        public string Recent { get => _resource.GetString(nameof(Recent)); }
        public string Reels { get => _resource.GetString(nameof(Reels)); }
        public string ReplyPlaceholder { get => _resource.GetString(nameof(ReplyPlaceholder)); }
        public string Requested { get => _resource.GetString(nameof(Requested)); }
        public string SelectDirectToLoad { get => _resource.GetString(nameof(SelectDirectToLoad)); }
        public string SendVerificationCode { get => _resource.GetString(nameof(SendVerificationCode)); }
        public string Stories { get => _resource.GetString(nameof(Stories)); }
        public string Share { get => _resource.GetString(nameof(Share)); }
        public string Story { get => _resource.GetString(nameof(Story)); }
        public string SuccessToastNotification { get => _resource.GetString(nameof(SuccessToastNotification)); }
        public string Tagged { get => _resource.GetString(nameof(Tagged)); }
        public string Top { get => _resource.GetString(nameof(Top)); }
        public string TrustThisDevice { get => _resource.GetString(nameof(TrustThisDevice)); }
        public string TwoFactorAuthentication { get => _resource.GetString(nameof(TwoFactorAuthentication)); }
        public string Unfollow { get => _resource.GetString(nameof(Unfollow)); }
        public string UnmutePosts { get => _resource.GetString(nameof(UnmutePosts)); }
        public string UnmuteStories { get => _resource.GetString(nameof(UnmuteStories)); }
        public string UnsupportedMessageType { get => _resource.GetString(nameof(UnsupportedMessageType)); }
        public string VerificationCode { get => _resource.GetString(nameof(VerificationCode)); }
        public string VerificationMethod { get => _resource.GetString(nameof(VerificationMethod)); }
        public string ViewProfile { get => _resource.GetString(nameof(ViewProfile)); }

        internal InstagramStrings() { }
    }

    public class SettingsStrings
    {
        ResourceLoader _resource = new("Settings");

        public string Autoplay { get => _resource.GetString(nameof(Autoplay)); }
        public string AutoplayDescription { get => _resource.GetString(nameof(AutoplayDescription)); }
        public string ForceThreeColumns { get => _resource.GetString(nameof(ForceThreeColumns)); }
        public string ForceThreeColumnsDescription { get => _resource.GetString(nameof(ForceThreeColumnsDescription)); }
        public string RemoveFeedAds { get => _resource.GetString(nameof(RemoveFeedAds)); }
        public string RemoveFeedAdsDescription { get => _resource.GetString(nameof(RemoveFeedAdsDescription)); }
    }

    public class UnitsStrings
    {
        ResourceLoader _resource = new("Units");

        public string Ago { get => _resource.GetString(nameof(Ago)); }
        public string Billion { get => _resource.GetString(nameof(Billion)); }
        public string Day { get => _resource.GetString(nameof(Day)); }
        public string DayAbbreviation { get => _resource.GetString(nameof(DayAbbreviation)); }
        public string Days { get => _resource.GetString(nameof(Days)); }
        public string DaysAbbreviation { get => _resource.GetString(nameof(DaysAbbreviation)); }
        public string Hour { get => _resource.GetString(nameof(Hour)); }
        public string HourAbbreviation { get => _resource.GetString(nameof(HourAbbreviation)); }
        public string Hours { get => _resource.GetString(nameof(Hours)); }
        public string HoursAbbreviation { get => _resource.GetString(nameof(HoursAbbreviation)); }
        public string Kilo { get => _resource.GetString(nameof(Kilo)); }
        public string Million { get => _resource.GetString(nameof(Million)); }
        public string Minute { get => _resource.GetString(nameof(Minute)); }
        public string MinuteAbbreviation { get => _resource.GetString(nameof(MinuteAbbreviation)); }
        public string Minutes { get => _resource.GetString(nameof(Minutes)); }
        public string MinutesAbbreviation { get => _resource.GetString(nameof(MinutesAbbreviation)); }
        public string Month { get => _resource.GetString(nameof(Month)); }
        public string MonthAbbreviation { get => _resource.GetString(nameof(MonthAbbreviation)); }
        public string Months { get => _resource.GetString(nameof(Months)); }
        public string MonthsAbbreviation { get => _resource.GetString(nameof(MonthsAbbreviation)); }
        public string Recently { get => _resource.GetString(nameof(Recently)); }
        public string Second { get => _resource.GetString(nameof(Second)); }
        public string SecondAbbreviation { get => _resource.GetString(nameof(SecondAbbreviation)); }
        public string Seconds { get => _resource.GetString(nameof(Seconds)); }
        public string SecondsAbbreviation { get => _resource.GetString(nameof(SecondsAbbreviation)); }
        public string Week { get => _resource.GetString(nameof(Week)); }
        public string WeekAbbreviation { get => _resource.GetString(nameof(WeekAbbreviation)); }
        public string Weeks { get => _resource.GetString(nameof(Weeks)); }
        public string WeeksAbbreviation { get => _resource.GetString(nameof(WeeksAbbreviation)); }
        public string Year { get => _resource.GetString(nameof(Year)); }
        public string YearAbbreviation { get => _resource.GetString(nameof(YearAbbreviation)); }
        public string Years { get => _resource.GetString(nameof(Years)); }
        public string YearsAbbreviation { get => _resource.GetString(nameof(YearsAbbreviation)); }
    }
}
