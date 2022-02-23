using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.UI.Controls
{
    internal class NoCastButtonMediaTransportControls : MediaTransportControls
    {
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            //Remove cast button
            AppBarButton CastButton = GetTemplateChild("CastButton") as AppBarButton;
            var MediaControlsCommandBar = GetTemplateChild("MediaControlsCommandBar") as CommandBar;
            MediaControlsCommandBar.PrimaryCommands.Remove(CastButton);

            //Set minimum width for overriden style
            this.Resources["MediaTransportControlsSliderWidth"] = 0;
            this.Resources["MediaTransportControlsMinWidth"] = 0;

            //finally override the style
            var style = App.Current.Resources["DefaultMediaTransportControlsStyle"];
            Style = (Style)style;
        }
    }
}
