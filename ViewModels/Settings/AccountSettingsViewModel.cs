using InstagramApiSharp.API;
using InstagramApiSharp.Classes;
using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;
using WinstaCore;

namespace ViewModels.Settings
{
    public class AccountSettingsViewModel : BaseViewModel
    {
        [OnChangedMethod(nameof(OnIsPrivateChanged))]
        public bool IsPrivate { get; set; }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            try
            {
                using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
                {
                    var RequestEdit = await Api.AccountProcessor.GetRequestForEditProfileAsync();
                    if (RequestEdit.Succeeded)
                    {
                        IsPrivate = RequestEdit.Value.IsPrivate;
                    }
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
}
