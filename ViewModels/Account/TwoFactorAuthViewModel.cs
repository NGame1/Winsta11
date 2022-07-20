using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
using WinstaCore.Helpers;
using WinstaCore.Interfaces.Views;
using WinstaCore.Interfaces.Views.Accounts;

namespace ViewModels.Account
{
    public class TwoFactorAuthViewModel : BaseViewModel
    {
        IInstaApi Api { get; set; }
        public AsyncRelayCommand VerifyCommand { get; set; }
        public AsyncRelayCommand SendVerificationCodeSMSCommand { get; set; }
        public AsyncRelayCommand SendVerificationNotificationCommand { get; set; }

        public bool TrustThisDevice { get; set; }
        public string VerificationCode { get; set; }

        public IEnumerable<InstaTwoFactorVerifyOptions> VerificationMethods { get => Enum.GetValues(typeof(InstaTwoFactorVerifyOptions)).Cast<InstaTwoFactorVerifyOptions>(); }

        [OnChangedMethod(nameof(OnVerificationMethodChanged))]
        public InstaTwoFactorVerifyOptions SelectedMethod { get; set; } = InstaTwoFactorVerifyOptions.SmsCode;

        public Visibility SendVerificationTextMessageVisibility { get; set; }
        public Visibility SendVerificationNotificationVisibility { get; set; }

        public override string PageHeader { get; protected set; }

        public TwoFactorAuthViewModel()
        {
            VerifyCommand = new(VerifyAsync);
            SendVerificationCodeSMSCommand = new(SendVerificationCodeSMSAsync);
            SendVerificationNotificationCommand = new(SendVerificationNotificationAsync);
            OnVerificationMethodChanged();
        }

        void OnVerificationMethodChanged()
        {
            SendVerificationTextMessageVisibility = Visibility.Collapsed;
            SendVerificationNotificationVisibility = Visibility.Collapsed;

            switch (SelectedMethod)
            {
                case InstaTwoFactorVerifyOptions.SmsCode:
                    SendVerificationTextMessageVisibility = Visibility.Visible;
                    break;
                case InstaTwoFactorVerifyOptions.RecoveryCode:
                    break;
                case InstaTwoFactorVerifyOptions.AuthenticationApp:
                    break;
                case InstaTwoFactorVerifyOptions.Notification:
                    SendVerificationNotificationVisibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        async Task SendVerificationNotificationAsync()
        {
            if (SendVerificationNotificationCommand.IsRunning) return;
            try
            {
                IsLoading = true;
                var trustedNotification = await Api.Check2FATrustedNotificationAsync();
                if (trustedNotification.Succeeded)
                {
                    var reviewStatus = trustedNotification.Value.ReviewStatus;

                    switch (reviewStatus)
                    {
                        case Insta2FANotificationReviewStatus.Unchanged:

                            return;

                        case Insta2FANotificationReviewStatus.Approved:
                            {
                                var code = "";
                                var trustedDevice = TrustThisDevice;

                                var twoFactorOption = InstaTwoFactorVerifyOptions.Notification;

                                var twoFactorLogin = await Api
                                    .TwoFactorLoginAsync(code,
                                    trustThisDevice: trustedDevice,
                                    twoFactorVerifyOptions: twoFactorOption);

                                if (twoFactorLogin.Succeeded)
                                {
                                    var state = Api.GetStateDataAsString();
                                    var loggedUser = Api.GetLoggedUser().LoggedInUser;
                                    await ApplicationSettingsManager.Instance.AddOrUpdateUser(loggedUser.Pk, state, loggedUser.UserName);
                                    var MainPage = AppCore.Container.GetService<IMainView>();
                                    NavigationService.Navigate(MainPage);
                                    await Api.SendRequestsAfterLoginAsync();
                                    Api.Dispose();
                                }
                                else
                                {
                                    FailToAuthenticate(twoFactorLogin);
                                    NavigationService.GoBack();
                                }
                            }
                            return;
                        case Insta2FANotificationReviewStatus.Denied:
                            UIContext.Post(new SendOrPostCallback(async (e) =>
                            {
                                await MessageDialogHelper.ShowAsync(trustedNotification.Info.Message, LanguageManager.Instance.General.Error);
                                NavigationService.GoBack();
                            }), null);
                            return;
                    }
                }
            }
            finally { IsLoading = false; }
        }

        async Task SendVerificationCodeSMSAsync()
        {
            if (SendVerificationCodeSMSCommand.IsRunning) return;
            try
            {
                IsLoading = true;
                var result = await Api.SendTwoFactorLoginSMSAsync();
                if (!result.Succeeded)
                {
                    await MessageDialogHelper.ShowAsync(result.Info.Message);
                    return;
                }

            }
            finally { IsLoading = false; }
        }

        public override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Api = (IInstaApi)e.Parameter;
        }

        private async Task VerifyAsync()
        {
            if (VerifyCommand.IsRunning) return;
            try
            {
                IsLoading = true;
                if (SelectedMethod == InstaTwoFactorVerifyOptions.Notification)
                {
                    var trustedNotification = await Api.Check2FATrustedNotificationAsync();
                    if (trustedNotification.Succeeded)
                    {
                        var reviewStatus = trustedNotification.Value.ReviewStatus;

                        switch (reviewStatus)
                        {
                            case Insta2FANotificationReviewStatus.Unchanged:
                                {

                                }
                                return;

                            case Insta2FANotificationReviewStatus.Approved:
                                {
                                    var code = "";
                                    var trustedDevice = TrustThisDevice;

                                    var twoFactorOption = InstaTwoFactorVerifyOptions.Notification;

                                    var twoFactorLogin = await Api
                                        .TwoFactorLoginAsync(code,
                                        trustThisDevice: trustedDevice,
                                        twoFactorVerifyOptions: twoFactorOption);

                                    if (twoFactorLogin.Succeeded)
                                    {
                                        var state = Api.GetStateDataAsString();
                                        var loggedUser = Api.GetLoggedUser().LoggedInUser;
                                        await ApplicationSettingsManager.Instance.AddOrUpdateUser(loggedUser.Pk, state, loggedUser.UserName);
                                        var MainPage = AppCore.Container.GetService<IMainView>();
                                        NavigationService.Navigate(MainPage);
                                        await Api.SendRequestsAfterLoginAsync();
                                        Api.Dispose();
                                    }
                                    else
                                    {
                                        FailToAuthenticate(twoFactorLogin);
                                        NavigationService.GoBack();
                                    }
                                }
                                return;
                            case Insta2FANotificationReviewStatus.Denied:
                                {
                                    UIContext.Post(new SendOrPostCallback(async (e) =>
                                    {
                                        await MessageDialogHelper.ShowAsync(trustedNotification.Info.Message, LanguageManager.Instance.General.Error);
                                        NavigationService.GoBack();
                                    }), null);
                                }
                                return;
                        }
                    }
                }

                IResult<InstaLoginTwoFactorResult> result;
                result = await Api.TwoFactorLoginAsync(VerificationCode, TrustThisDevice, SelectedMethod);
                if (!result.Succeeded)
                {
                    await MessageDialogHelper.ShowAsync(result.Info.Message);
                    return;
                }
                switch (result.Value)
                {
                    case InstaLoginTwoFactorResult.Success:
                        var state = Api.GetStateDataAsString();
                        var loggedUser = Api.GetLoggedUser().LoggedInUser;
                        await ApplicationSettingsManager.Instance.AddOrUpdateUser(loggedUser.Pk, state, loggedUser.UserName);
                        var MainPage = AppCore.Container.GetService<IMainView>();
                        NavigationService.Navigate(MainPage);
                        await Api.SendRequestsAfterLoginAsync();
                        Api.Dispose();
                        break;

                    case InstaLoginTwoFactorResult.ChallengeRequired:
                        var ChallengeRequiredView = AppCore.Container.GetService<IChallengeRequiredView>();
                        NavigationService.Navigate(ChallengeRequiredView, Api);
                        break;

                    default:
                        FailToAuthenticate(result);
                        break;
                }
            }
            finally { IsLoading = false; }
        }

        void FailToAuthenticate(IResult<InstaLoginTwoFactorResult> loginResult)
        {
            UIContext.Post(new SendOrPostCallback(async (e) =>
            {
                await MessageDialogHelper.ShowAsync(loginResult.Info.Message, LanguageManager.Instance.General.Error);
            }), null);
        }
    }
}
