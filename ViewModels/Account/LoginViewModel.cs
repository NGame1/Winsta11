using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using Microsoft.Toolkit.Mvvm.Input;
using Resources;
using System;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http.Filters;
using Windows.Web.Http;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaCore.Interfaces.Views;
using WinstaCore.Interfaces.Views.Accounts;
using Windows.UI.Xaml.Controls;
using InstagramApiSharp.Helpers;
using System.Text;
using Windows.UI.Xaml;
using InstagramApiSharp;
using System.Diagnostics;

namespace ViewModels.Account
{
    public class LoginViewModel : BaseViewModel
    {
        IInstaApi Api { get; }
        public string UserIdentifier { get; set; }
        public string Password { get; set; }
        public AsyncRelayCommand LoginCommand { get; }
        public RelayCommand<WebView> FacebookLoginCommand { get; }
        public RelayCommand HideFacebookLoginGridCommand { get; }

        public Visibility FacebookLoginVisibility { get; set; } = Visibility.Collapsed;

        public LoginViewModel()
        {
            Api = AppCore.Container.GetService<IInstaApi>();
            HideFacebookLoginGridCommand = new(HideFacebookLoginGrid);
            FacebookLoginCommand = new(LoginWithFacebook);
            LoginCommand = new(LoginAsync);
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            await Api.SendRequestsBeforeLoginAsync();
            await base.OnNavigatedToAsync(e);
        }

        async Task LoginAsync()
        {
            if (LoginCommand.IsRunning) return;
            IResult<InstaLoginResult> loginResult;
            try
            {
                IsLoading = true;
                Api.SetUser(UserIdentifier, Password);
                loginResult = await Api.LoginAsync();
                
                switch (loginResult.Value)
                {
                    case InstaLoginResult.Success:
                        var state = Api.GetStateDataAsString();
                        var loggedUser = Api.GetLoggedUser().LoggedInUser;
                        await ApplicationSettingsManager.Instance.AddOrUpdateUser(loggedUser.Pk, state, loggedUser.UserName);
                        var MainPage = AppCore.Container.GetService<IMainView>();
                        NavigationService.Navigate(MainPage);
                        await Api.SendRequestsAfterLoginAsync();
                        Api.Dispose();
                        break;

                    case InstaLoginResult.TwoFactorRequired:
                        var TwoFactorAuthView = AppCore.Container.GetService<ITwoFactorAuthView>();
                        NavigationService.Navigate(TwoFactorAuthView, Api);
                        break;

                    case InstaLoginResult.ChallengeRequired:
                        var ChallengeRequiredView = AppCore.Container.GetService<IChallengeRequiredView>();
                        NavigationService.Navigate(ChallengeRequiredView, Api);
                        break;

                    default:
                        FailToLogin(loginResult);
                        return;
                }
            }
            finally { IsLoading = false; }
        }

        void FailToLogin(IResult<InstaLoginResult> loginResult)
        {
            UIContext.Post(new SendOrPostCallback(async (e) =>
            {
                await MessageDialogHelper.ShowAsync(loginResult.Info.Message, LanguageManager.Instance.General.Error);
            }), null);
        }

        public void LoginWithFacebook(WebView webView)
        {
            FacebookLoginVisibility = Visibility.Visible;
            DeleteFacebookCookies();
            var destinationUri = InstaFbHelper.GetFacebookLoginUri();
            webView.Source = destinationUri;
            //webView.Navigate(facebookLoginUri);
        }

        public async void CompleteLoginWithFacebook(string html, Uri url)
        {
            var fbToken = InstaFbHelper.GetAccessToken(html);
            var cookies = GetUriCookies(url);
            var loginResult = await Api.LoginWithFacebookAsync(fbToken, cookies);
            switch (loginResult.Value)
            {
                case InstaLoginResult.Success:
                    var state = Api.GetStateDataAsString();
                    var loggedUser = Api.GetLoggedUser().LoggedInUser;
                    await ApplicationSettingsManager.Instance.AddOrUpdateUser(loggedUser.Pk, state, loggedUser.UserName);
                    var MainPage = AppCore.Container.GetService<IMainView>();
                    NavigationService.Navigate(MainPage);
                    await Api.SendRequestsAfterLoginAsync();
                    Api.Dispose();
                    break;

                case InstaLoginResult.TwoFactorRequired:
                    var TwoFactorAuthView = AppCore.Container.GetService<ITwoFactorAuthView>();
                    NavigationService.Navigate(TwoFactorAuthView, Api);
                    break;

                case InstaLoginResult.ChallengeRequired:
                    var ChallengeRequiredView = AppCore.Container.GetService<IChallengeRequiredView>();
                    NavigationService.Navigate(ChallengeRequiredView, Api);
                    break;

                default:
                    FailToLogin(loginResult);
                    return;
            }
            FacebookLoginVisibility = Visibility.Collapsed;
        }
        void HideFacebookLoginGrid()
        {
            FacebookLoginVisibility = Visibility.Collapsed;
        }

        private string GetUriCookies(Uri targetUri)
        {
            var httpBaseProtocolFilter = new HttpBaseProtocolFilter();
            var cookieManager = httpBaseProtocolFilter.CookieManager;
            var cookieCollection = cookieManager.GetCookies(targetUri);
            var sbCookies = new StringBuilder();
            foreach (var item in cookieCollection)
            {
                sbCookies.Append($"{item.Name}={item.Value}; ");
            }
            return sbCookies.ToString();
        }

        private void DeleteFacebookCookies()
        {
            HttpBaseProtocolFilter myFilter = new HttpBaseProtocolFilter();
            var cookieManager = myFilter.CookieManager;

            try
            {
                HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://facebook.com/"));
                foreach (HttpCookie cookie in myCookieJar)
                {
                    cookieManager.DeleteCookie(cookie);
                }
            }
            catch { }
            try
            {

                HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://www.facebook.com/"));
                foreach (HttpCookie cookie in myCookieJar)
                {
                    cookieManager.DeleteCookie(cookie);
                }
            }
            catch { }
            try
            {
                HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://m.facebook.com/"));
                foreach (HttpCookie cookie in myCookieJar)
                {
                    cookieManager.DeleteCookie(cookie);
                }
            }
            catch { }
            try
            {
                HttpCookieCollection myCookieJar = cookieManager.GetCookies(new Uri("https://instagram.com/"));
                foreach (HttpCookie cookie in myCookieJar)
                {
                    cookieManager.DeleteCookie(cookie);
                }
            }
            catch { }
        }
    }
}
