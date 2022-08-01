using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views;
using WinstaCore;
using WinstaCore.Interfaces.Views.Accounts;

namespace ViewModels.Account
{
    public class ChallengeRequiredViewModel : BaseViewModel
    {
        public IInstaApi Api { get; set; }

        public AsyncRelayCommand SubmitPhoneCommand { get; set; }
        public bool SubmitPhoneRequired { get; set; } = false;
        public string PhoneNumber { get; set; }

        public bool UnknownChaalenge { get; set; } = false;
        public Uri ChallengeUrl { get; set; }

        public bool NormalChallenge { get; set; } = false;
        public bool PhoneAuthenticationVisible { get; set; } = false;
        public bool PhoneAuthChecked { get; set; } = false;
        public bool EmailAuthChecked { get; set; } = false;
        public bool EmailAuthenticationVisible { get; set; } = false;
        public string PhoneAuthNumber { get; set; } = string.Empty;
        public string EmailAuthAddress { get; set; } = string.Empty;
        public AsyncRelayCommand SendVerificationCodeCommand { get; set; }

        public bool VerifyStep { get; set; } = false;
        public string VerificationCode { get; set; } = string.Empty;
        public AsyncRelayCommand VerifyCommand { get; set; }

        public ChallengeRequiredViewModel()
        {
            SubmitPhoneCommand = new(SubmitPhoneAsync);
            SendVerificationCodeCommand = new(SendVerificationCodeAsync);
            VerifyCommand = new(VerifyAsync);
        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            Api = (IInstaApi)e.Parameter;
            var challenge = await Api.GetChallengeRequireVerifyMethodAsync();
            if (challenge.Succeeded)
            {
                if (challenge.Value.SubmitPhoneRequired)
                {
                    SubmitPhoneRequired = true;
                }
                else
                {
                    if (challenge.Value.FlowRenderType == InstaChallengeFlowRenderType.Unknown)
                    {
                        var url = Api.ChallengeLoginInfo.Url;
                        ChallengeUrl = new(url);
                        UnknownChaalenge = true;
                    }
                    else
                    {
                        if (challenge.Value.StepData != null)
                        {
                            NormalChallenge = true;

                            if (!string.IsNullOrEmpty(challenge.Value.StepData.PhoneNumber))
                            {
                                PhoneAuthenticationVisible = true;
                                PhoneAuthChecked = true;
                                PhoneAuthNumber = challenge.Value.StepData.PhoneNumber;

                            }
                            if (!string.IsNullOrEmpty(challenge.Value.StepData.Email))
                            {
                                EmailAuthenticationVisible = true;
                                if (!PhoneAuthChecked) EmailAuthChecked = true;
                                EmailAuthAddress = challenge.Value.StepData.Email;
                            }
                            if (challenge.Value.IsUnvettedDelta || challenge.Value.StepData.IsNewDeltaUI())
                            {
                                await Api.GetDeltaChallengeAsync();
                            }
                        }
                    }
                }
            }
        }

        public async Task SubmitPhoneAsync()
        {
            if (string.IsNullOrEmpty(PhoneNumber) ||
                     string.IsNullOrWhiteSpace(PhoneNumber))
            {
                throw new Exception("Please type a valid phone number(with country code).\r\ni.e: +989123456789");
            }

            var phoneNumber = PhoneNumber;
            if (!phoneNumber.StartsWith("+"))
                phoneNumber = $"+{phoneNumber}";
            IsLoading = true;
            try
            {
                var submitPhone = await Api.SubmitPhoneNumberForChallengeRequireAsync(phoneNumber);
                if (submitPhone.Succeeded)
                {
                    SubmitPhoneRequired = false;
                    NormalChallenge = true;
                }
                else
                    throw submitPhone.Info.Exception;
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SendVerificationCodeAsync()
        {
            IsLoading = true;
            try
            {
                if (EmailAuthChecked)
                {
                    if (IsDeltaChallenge())
                    {
                        var email = await Api.SetDeltaChallengeChoiceAsync(InstaDeltaChallengeChoice.Email);
                        if (!email.Succeeded)
                            throw new Exception(email.Info.Message);

                    }
                    else
                    {
                        // send verification code to email
                        var email = await Api.RequestVerifyCodeToEmailForChallengeRequireAsync();
                        if (!email.Succeeded)
                            throw new Exception(email.Info.Message);
                    }
                }
                else if (PhoneAuthChecked)
                {
                    if (IsDeltaChallenge())
                    {
                        var phoneNumber = await Api.SetDeltaChallengeChoiceAsync(InstaDeltaChallengeChoice.Phone);
                        if (!phoneNumber.Succeeded)
                            throw new Exception(phoneNumber.Info.Message);
                    }
                    else
                    {
                        // send verification code to phone number
                        var phoneNumber = await Api.RequestVerifyCodeToSMSForChallengeRequireAsync();
                        if (!phoneNumber.Succeeded)
                            throw new Exception(phoneNumber.Info.Message);
                    }
                }
            }
            finally
            {
                IsLoading = false;
                NormalChallenge = false;
                VerifyStep = true;
            }
        }

        public async Task VerifyAsync()
        {
            VerificationCode = VerificationCode.Trim();
            VerificationCode = VerificationCode.Replace(" ", "");
            var regex = new Regex(@"^-*[0-9,\.]+$");
            if (!regex.IsMatch(VerificationCode))
            {
                throw new Exception("Verification code is a 6 digits number");
            }
            IsLoading = true;
            try
            {
                // Note: calling VerifyCodeForChallengeRequireAsync function, 
                // if user has two factor enabled, will wait 15 seconds and it will try to
                // call LoginAsync.

                var verifyLogin = await Api.VerifyCodeForChallengeRequireAsync(VerificationCode);
                if (verifyLogin.Succeeded)
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
                    if (verifyLogin.Value == InstaLoginResult.TwoFactorRequired)
                    {
                        var TwoFactorAuthView = AppCore.Container.GetService<ITwoFactorAuthView>();
                        NavigationService.Navigate(TwoFactorAuthView);
                    }
                    else throw new Exception(verifyLogin.Info.Message);
                }

            }
            catch (Exception) { throw; }
            finally { IsLoading = false; }
        }

        private bool IsDeltaChallenge() => Api.ChallengeVerifyMethod?.IsUnvettedDelta ?? false ||
                    (Api.ChallengeVerifyMethod?.StepData != null && Api.ChallengeVerifyMethod.StepData.IsNewDeltaUI());

    }
}