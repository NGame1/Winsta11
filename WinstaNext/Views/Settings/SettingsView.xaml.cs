// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

using Resources;
using WinstaCore.Interfaces.Views.Settings;

namespace WinstaNext.Views.Settings
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsView : BasePage, ISettingsView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.General.Settings;

        public SettingsView()
        {
            this.InitializeComponent();
        }
    }
}
