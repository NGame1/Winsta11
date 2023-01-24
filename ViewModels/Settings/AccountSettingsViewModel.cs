using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Mvvm.Input;
using PropertyChanged;
using Resources;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml.Navigation;
using WinstaCore;

namespace ViewModels.Settings;

public class AccountSettingsViewModel : BaseViewModel
{
    [OnChangedMethod(nameof(OnIsPrivateChanged))]
    public bool IsPrivate { get; set; }

    public AsyncRelayCommand LogoutCommand { get; set; }

    public AccountSettingsViewModel()
    {
        LogoutCommand = new(LogoutAsync);
    }

    public async Task LogoutAsync()
    {
        var msg = new MessageDialog(LanguageManager.Instance.Messages.LogoutConfirmation);
        msg.Commands.Add(new UICommand(LanguageManager.Instance.General.Yes));
        msg.Commands.Add(new UICommand(LanguageManager.Instance.General.No));
        var msgresult = await msg.ShowAsync();
        if (msgresult is UICommand userChoice &&
            userChoice.Label == LanguageManager.Instance.General.No)
            return;

        using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();
        InstaUserShort user = AppCore.Container.GetService<InstaUserShort>();
        var res = await Api.LogoutAsync();
        if (res.Succeeded)
        {
            ApplicationSettingsManager.Instance.RemoveUser(user.Pk);
            MainPageViewModel.mainPageViewModel.RaiseForceLogoutEvent();
        }
        else
        {
            msg = new MessageDialog(
                string.Format(LanguageManager.Instance.Messages.ForceLogout, res.Info.Message));
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.Yes));
            msg.Commands.Add(new UICommand(LanguageManager.Instance.General.No));
            msgresult = await msg.ShowAsync();
            if (msgresult is UICommand userChoice2 &&
            userChoice2.Label == LanguageManager.Instance.General.No)
                return;
            ApplicationSettingsManager.Instance.RemoveUser(user.Pk);
            MainPageViewModel.mainPageViewModel.RaiseForceLogoutEvent();
        }
    }

    public override async Task OnNavigatedToAsync(NavigationEventArgs e)
    {
        try
        {
            using IInstaApi Api = AppCore.Container.GetService<IInstaApi>();

            var RequestEdit = await Api.AccountProcessor.GetRequestForEditProfileAsync();
            if (RequestEdit.Succeeded)
            {
                IsPrivate = RequestEdit.Value.IsPrivate;
            }

            await Task.Delay(100);
        }
        finally
        {
            IsLoading = false;
        }
    }

    async void OnIsPrivateChanged()
    {
        if (IsLoading) return;
        IsLoading = true;
        try
        {
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
            {
                IResult<InstaUserShort> result;
                if (IsPrivate)
                    result = await Api.AccountProcessor.SetAccountPrivateAsync();
                else result = await Api.AccountProcessor.SetAccountPublicAsync();
                if (result.Succeeded)
                {
                    IsPrivate = result.Value.IsPrivate;
                }
                else IsPrivate = !IsPrivate;
                await Task.Delay(100);
            }
        }
        finally
        {
            IsLoading = false;
        }
    }

}
