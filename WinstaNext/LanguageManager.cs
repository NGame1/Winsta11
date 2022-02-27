using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace WinstaNext
{
    [AddINotifyPropertyChangedInterface]
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
        ResourceLoader _resource = new ResourceLoader("General");

        public string ApplicationName { get => _resource.GetString(nameof(ApplicationName)); }
        public string AppTheme { get => _resource.GetString(nameof(AppTheme)); }
        public string AppThemeDescription { get => _resource.GetString(nameof(AppThemeDescription)); }
        public string Call { get => _resource.GetString(nameof(Call)); }
        public string Cancel { get => _resource.GetString(nameof(Cancel)); }
        public string Close { get => _resource.GetString(nameof(Close)); }
        public string Copy { get => _resource.GetString(nameof(Copy)); }
        public string Dark { get => _resource.GetString(nameof(Dark)); }
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
        ResourceLoader _resource = new ResourceLoader("Instagram");

        public string Accounts { get => _resource.GetString(nameof(Accounts)); }
        public string Activities { get => _resource.GetString(nameof(Activities)); }
        public string Biography { get => _resource.GetString(nameof(Biography)); }
        public string Comment { get => _resource.GetString(nameof(Comment)); }
        public string CommentPlaceholder { get => _resource.GetString(nameof(CommentPlaceholder)); }
        public string Directs { get => _resource.GetString(nameof(Directs)); }
        public string Explore { get => _resource.GetString(nameof(Explore)); }
        public string Feed { get => _resource.GetString(nameof(Feed)); }
        public string Follow { get => _resource.GetString(nameof(Follow)); }
        public string FollowBack { get => _resource.GetString(nameof(FollowBack)); }
        public string Followers { get => _resource.GetString(nameof(Followers)); }
        public string Followings { get => _resource.GetString(nameof(Followings)); }
        public string Hashtags { get => _resource.GetString(nameof(Hashtags)); }
        public string Highlights { get => _resource.GetString(nameof(Highlights)); }
        public string IsFollowingYou { get => _resource.GetString(nameof(IsFollowingYou)); }
        public string Like { get => _resource.GetString(nameof(Like)); }
        public string Likes { get => _resource.GetString(nameof(Likes)); }
        public string LoadMoreComments { get => _resource.GetString(nameof(LoadMoreComments)); }
        public string MediaComments { get => _resource.GetString(nameof(MediaComments)); }
        public string MediaLikers { get => _resource.GetString(nameof(MediaLikers)); }
        public string NoDirectsYet { get => _resource.GetString(nameof(NoDirectsYet)); }
        public string Places { get => _resource.GetString(nameof(Places)); }
        public string Posts { get => _resource.GetString(nameof(Posts)); }
        public string SelectDirectToLoad { get => _resource.GetString(nameof(SelectDirectToLoad)); }
        public string Stories { get => _resource.GetString(nameof(Stories)); }
        public string Share { get => _resource.GetString(nameof(Share)); }
        public string Story { get => _resource.GetString(nameof(Story)); }
        public string SendVerificationCode { get => _resource.GetString(nameof(SendVerificationCode)); }
        public string Top { get => _resource.GetString(nameof(Top)); }
        public string TrustThisDevice { get => _resource.GetString(nameof(TrustThisDevice)); }
        public string TwoFactorAuthentication { get => _resource.GetString(nameof(TwoFactorAuthentication)); }
        public string Unfollow { get => _resource.GetString(nameof(Unfollow)); }
        public string UnsupportedMessageType { get => _resource.GetString(nameof(UnsupportedMessageType)); }
        public string VerificationCode { get => _resource.GetString(nameof(VerificationCode)); }
        public string VerificationMethod { get => _resource.GetString(nameof(VerificationMethod)); }

        internal InstagramStrings() { }
    }
    public class SettingsStrings
    {
        ResourceLoader _resource = new ResourceLoader("Settings");

        public string Autoplay { get => _resource.GetString(nameof(Autoplay)); }
        public string AutoplayDescription { get => _resource.GetString(nameof(AutoplayDescription)); }
    }

    public class UnitsStrings
    {
        ResourceLoader _resource = new ResourceLoader("Units");

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
