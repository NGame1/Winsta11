using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WinstaNext.Core.Dialogs
{
    public static class MessageDialogHelper
    {
        public static async void Show(string content, string title = "")
        {
            var msg = new MessageDialog(content, title);
            await msg.ShowAsync();
        }

        public static async Task ShowAsync(string content, string title = "")
        {
            var msg = new MessageDialog(content, title);
            await msg.ShowAsync();
        }
    }
}
