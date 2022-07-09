using System;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace WinstaNext.Core.Dialogs
{
    public static class MessageDialogHelper
    {
        public static async void Show(string content, string title = "")
        {
            try
            {
                var msg = new MessageDialog(content, title);
                await msg.ShowAsync();
            }
            catch
            {
                await Task.Yield();
                Show(content, title);
            }
        }

        public static async Task ShowAsync(string content, string title = "")
        {
            try
            {
                var msg = new MessageDialog(content, title);
                await msg.ShowAsync();
            }
            catch (Exception)
            {
                await Task.Yield();
                await ShowAsync(content, title);
            }
        }
    }
}
