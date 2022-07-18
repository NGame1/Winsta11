using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using Microsoft.Toolkit.Mvvm.Input;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Interfaces.Views;
using WinstaCore.Interfaces.Views.Accounts;

namespace ViewModels.Account
{
    public class LoginViewModel : BaseViewModel
    {
        public override string PageHeader { get; protected set; }
        IInstaApi Api { get; }
        public string UserIdentifier { get; set; }
        public string Password { get; set; }
        public AsyncRelayCommand LoginCommand { get; }
        public LoginViewModel()
        {
            Api = AppCore.Container.GetService<IInstaApi>();
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
                //await MessageDialogHelper.ShowAsync(loginResult.Info.Message, LanguageManager.Instance.General.Error);
            }), null);
        }
    }
}
