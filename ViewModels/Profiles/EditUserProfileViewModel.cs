using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaCore;
#nullable enable

namespace ViewModels.Profiles;

public class EditUserProfileViewModel : BaseViewModel
{
    public InstaUserEdit? User { get; set; } = null;

    public string? FullName { get; set; }
    public string? Username { get; set; }
    public string? Biography { get; set; }
    public string? ExternalUrl { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public InstaGenderType? Gender { get; set; }
    public AsyncRelayCommand SaveChangesCommand { get; set; }

    public EditUserProfileViewModel()
    {
        SaveChangesCommand = new(SaveChangesAsync);
    }

    async Task SaveChangesAsync()
    {
        try
        {
            IsLoading = true;
            await EditUserProfileAsync(FullName!, Biography!, ExternalUrl, Email!,
                                           User!.PhoneNumber, User!.Gender, Username!);
        }
        finally
        {
            IsLoading = false;
        }
    }

    public async Task EditUserProfileAsync(string name, string? biography, string? url, string email,
                                                string phone, InstaGenderType gender, string newUserName)
    {
        using var Api = AppCore.Container.GetService<IInstaApi>();
        var result = await Api.AccountProcessor.EditProfileAsync(name, biography, url, email, phone, gender, newUserName);
        if (!result.Succeeded)
            throw result.Info.Exception;
        NavigationService.GoBack();
    }

    public async Task<InstaUserEdit> GetUserProfileInfoAsync()
    {
        using var Api = AppCore.Container.GetService<IInstaApi>();
        var RequestEdit = await Api.AccountProcessor.GetRequestForEditProfileAsync();
        if (!RequestEdit.Succeeded)
        {
            NavigationService.GoBack();
            throw RequestEdit.Info.Exception;
        }
        return RequestEdit.Value;
    }

    public override async Task OnNavigatedToAsync(NavigationEventArgs e)
    {
        User = await GetUserProfileInfoAsync();
        Email = User?.Email;
        Gender = User?.Gender;
        Username = User?.Username;
        FullName = User?.FullName;
        Phone = User?.PhoneNumber;
        Biography = User?.Biography;
        ExternalUrl = User?.ExternalUrl;

        await base.OnNavigatedToAsync(e);
    }
}
