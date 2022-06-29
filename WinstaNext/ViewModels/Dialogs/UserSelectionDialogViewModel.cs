using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using WinstaNext.Core.Collections;

namespace WinstaNext.ViewModels.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    internal class UserSelectionDialogViewModel : BaseDialogViewModel
    {
        static ExtendedObservableCollection<InstaDirectInboxThread> StaticThreads { get; set; } = new();
        public ExtendedObservableCollection<InstaDirectInboxThread> Threads { get; set; } = new();

        public UserSelectionDialogViewModel()
        {
            if (StaticThreads.Count != 0)
                Threads.AddRange(StaticThreads);
            else LoadUsers();
        }

        async void LoadUsers()
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var result = await Api.MessagingProcessor.GetRankedRecipientsAsync();
                if (!result.Succeeded)
                {
                    HideDialogAction?.Invoke();
                    throw result.Info.Exception;
                }
                Threads.AddRange(result.Value.Items);
                StaticThreads = Threads;
            }
        }
    }
}
