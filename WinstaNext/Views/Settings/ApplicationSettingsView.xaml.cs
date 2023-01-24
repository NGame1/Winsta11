using Resources;
using WinstaCore.Interfaces.Views.Settings;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Settings;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class ApplicationSettingsView : BasePage, IApplicationSettingsView
{
    public override string PageHeader { get; protected set; } = LanguageManager.Instance.Settings.ApplicationSettings;

    public ApplicationSettingsView()
    {
        this.InitializeComponent();
    }

}
