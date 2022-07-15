namespace WinstaCore.Theme
{
    /// <summary>
    /// Specifies a UI theme that should be used for individual UIElement parts of an app UI.
    /// </summary>
    public enum AppTheme
    {
        /// <summary>
        /// Use the Application.RequestedTheme value for the element. This is the default.
        /// </summary>
        Default = 0,

        /// <summary>
        /// Use the **Light** default theme.
        /// </summary>
        Light = 1,

        /// <summary>
        /// Use the **Dark** default theme.
        /// </summary>
        Dark = 2,
    }
}
