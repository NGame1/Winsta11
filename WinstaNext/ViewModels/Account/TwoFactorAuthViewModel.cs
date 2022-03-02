using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Dialogs;
using WinstaNext.Views.Account;

namespace WinstaNext.ViewModels.Account
{
    internal class TwoFactorAuthViewModel : BaseViewModel
    {
        IInstaApi Api { get; set; }
        public RelayCommand VerifyCommand { get; set; }
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
            VerifyCommand = new RelayCommand(VerifyAsync);
            SendVerificationCodeSMSCommand = new AsyncRelayCommand(SendVerificationCodeSMS);
            SendVerificationNotificationCommand = new AsyncRelayCommand(SendVerificationNotification);
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

        private async Task SendVerificationNotification()
        {
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
                                    NavigationService.Navigate(typeof(MainPage));
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

        private async Task SendVerificationCodeSMS()
        {
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

        private async void VerifyAsync()
        {
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
                                        NavigationService.Navigate(typeof(MainPage));
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
                        NavigationService.Navigate(typeof(MainPage));
                        await Api.SendRequestsAfterLoginAsync();
                        Api.Dispose();
                        break;

                    case InstaLoginTwoFactorResult.ChallengeRequired:
                        NavigationService.Navigate(typeof(ChallengeRequiredView), Api);
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
