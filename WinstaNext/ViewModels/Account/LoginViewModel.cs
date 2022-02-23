using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Xaml.Interactions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Core.Preview;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Dialogs;
using WinstaNext.Views.Account;

namespace WinstaNext.ViewModels.Account
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
            Api = App.Container.GetService<IInstaApi>();
            LoginCommand = new AsyncRelayCommand(LoginAsync);
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            await Api.SendRequestsBeforeLoginAsync();
            await base.OnNavigatedToAsync(e);
        }

        async Task LoginAsync()
        {
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
                        NavigationService.Navigate(typeof(MainPage));
                        break;

                    case InstaLoginResult.TwoFactorRequired:
                        NavigationService.Navigate(typeof(TwoFactorAuthView), Api);
                        break;

                    case InstaLoginResult.ChallengeRequired:
                        NavigationService.Navigate(typeof(ChallengeRequiredView), Api);
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
    }
}
