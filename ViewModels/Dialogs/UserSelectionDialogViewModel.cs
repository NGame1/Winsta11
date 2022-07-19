using Core.Collections;
using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using WinstaCore;

namespace ViewModels.Dialogs
{
    [AddINotifyPropertyChangedInterface]
    public class UserSelectionDialogViewModel : BaseDialogViewModel
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
            using (IInstaApi Api = AppCore.Container.GetService<IInstaApi>())
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
