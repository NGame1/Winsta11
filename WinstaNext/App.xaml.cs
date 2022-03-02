using InstagramApiSharp.API;
using InstagramApiSharp.API.Builder;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Uwp.Helpers;
using NodaTime.TimeZones;
using Syncfusion.Licensing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Media.Playback;
using Windows.System.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Core.Dialogs;
using WinstaNext.Services;

namespace WinstaNext
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        public static IServiceProvider Container { get; private set; }

        public App()
        {
            this.InitializeComponent();
            ApplicationViewScaling.TrySetDisableLayoutScaling(true);
            SyncfusionLicenseProvider.RegisterLicense("NTYxODMyQDMxMzkyZTM0MmUzMGlhcE44YWwwUi9yNGt2aUNYSjZUVmgxQWM1Qk5lSGsrZWM3NTZiS0FiamM9;NTYxODMzQDMxMzkyZTM0MmUzMEtKMmYyYitybjZpdkNELzhGay80QU1Ja1RPT2YrR0l4NFEvbVoweC9FK0U9;NTYxODM0QDMxMzkyZTM0MmUzMEJGKzBmWFBQb3U5ZklzMU1CUWpra1FNc1ZhYU5tZDl4cVZYbXZwOFY1eHc9;NTYxODM1QDMxMzkyZTM0MmUzMGZvQWY5N0VLcUVVdEtuSVE2VE1kVU9CMSsrR3BBcEJpU0Q0bVpnMERkSGs9;NTYxODM2QDMxMzkyZTM0MmUzMEFZdTlUOUlSckxuWDRhNEZsdWNGYzBaZXIrelgzV0t3a2UvVVp4R3dxUlk9;NTYxODM3QDMxMzkyZTM0MmUzMGtlcW1HRUNxSUs5d3RqT3ZzZkc0Mm4zaVZQMVEvczc4akNTWUI3ekQrNGc9;NTYxODM4QDMxMzkyZTM0MmUzMENLeTZHUHlaMGhYSUFEWmhCdjBWU1hqeU9Ua05CN0hqaCtDVy9RWkhPams9;NTYxODM5QDMxMzkyZTM0MmUzMFNXUStjOXY0cGJuUWRHWTFZWURJbUl2K1RUWCtuMmxxQTg2bzVqQXlUNms9;NTYxODQwQDMxMzkyZTM0MmUzMEI1aTVmeXZTVGJnc2V1STJmQWFUZ0lqRXlYbDZPREJRTjZwRGI2MnoxVnM9;NTYxODQxQDMxMzkyZTM0MmUzMFZsQlVqZVRMQjRBb1Z6cEtxcXM2NTEzU3hBeFdUVkJ0dnFyRTB3SkdzSDA9;NTYxODQyQDMxMzkyZTM0MmUzMFdLSmVnSEw2QS84cWZ2dFhzNUZZSlAzQmxCSWdBZWQ2SDRPa0dHY3lXVDg9");
            this.Suspending += OnSuspending;
            
            this.UnhandledException += App_UnhandledException;
            TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
            //Enable reveal focus for visual elements
            this.FocusVisualKind = FocusVisualKind.Reveal;

            //Removes mouse pointer on XBOX
            if (SystemInformation.Instance.DeviceFamily == "Windows.Xbox")
                this.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            e.SetObserved();
            var stack = Environment.StackTrace;
            MessageDialogHelper.Show(e.Exception.Message);
        }

        public void SetContainer()
        {
            Container = ConfigureDependencyInjection();
        }

        private void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var stack = Environment.StackTrace;
            MessageDialogHelper.Show(e.Message);
        }

        IServiceProvider ConfigureDependencyInjection()
        {
            var serviceCollection = new ServiceCollection();

            serviceCollection.AddTransient<IInstaApi>(CreateInstaAPIInstance);
            serviceCollection.AddTransient<InstaUserShort>(CreateMyUserInstance);
            serviceCollection.AddTransient<NavigationService>(CreateNavigationService);
            serviceCollection.AddSingleton<DisplayRequest>(CreateDisplayRequestInstance);

            serviceCollection.AddSingleton<MediaPlayer>(CreateMediaPlayerInstance);

            return serviceCollection.BuildServiceProvider();
        }

        DisplayRequest CreateDisplayRequestInstance(IServiceProvider arg)
        {
            return new DisplayRequest();
        }

        private MediaPlayer CreateMediaPlayerInstance(IServiceProvider arg)
        {
            return new MediaPlayer()
            {
                AudioCategory = MediaPlayerAudioCategory.Communications,
                AutoPlay = false
            };
        }

        Dictionary<Window, NavigationService> Navigations { get; } = new();

        private NavigationService CreateNavigationService(IServiceProvider arg)
        {
            if (!Navigations.ContainsKey(Window.Current))
            {
                var CurrentWindow = Window.Current;
                var currentFrame = CurrentWindow.Content as Frame;
                var _navigationService = new NavigationService(currentFrame);
                Navigations.Add(CurrentWindow, _navigationService);
                return _navigationService;
            }
            return Navigations[Window.Current];
        }

        InstaUserShort _myUser = null;
        public void SetMyUserInstance(InstaUserShort _user)
        {
            _myUser = _user;
        }

        private InstaUserShort CreateMyUserInstance(IServiceProvider arg)
        {
            if (_myUser != null) return _myUser;
            using (var Api = arg.GetService<IInstaApi>())
            {
                return Api.GetLoggedUser().LoggedInUser;
            }
        }

        string _session = "";
        public string SetCurrentUserSession(string session) => _session = session;
        private IInstaApi CreateInstaAPIInstance(IServiceProvider arg)
        {
            var api = InstaApiBuilder
                .CreateBuilder()
                .Build();
            var local = TimeZoneInfo.Local;
            api.TimezoneOffset = Convert.ToInt32(local.BaseUtcOffset.TotalSeconds);
            var tzd = TzdbDateTimeZoneSource.Default.WindowsToTzdbIds.FirstOrDefault(x => x.Key == local.StandardName);
            api.SetTimezone(tzd.Value);
            if (!string.IsNullOrEmpty(_session))
            {
                api.LoadStateDataFromString(_session);
            }
            return api;
        }

        internal IInstaApi CreateInstaAPIInstance(string session)
        {
            var api = InstaApiBuilder
                .CreateBuilder()
                .Build();
            var local = TimeZoneInfo.Local;
            api.TimezoneOffset = Convert.ToInt32(local.BaseUtcOffset.TotalSeconds);
            var tzd = TzdbDateTimeZoneSource.Default.WindowsToTzdbIds.FirstOrDefault(x => x.Key == local.StandardName);
            api.SetTimezone(tzd.Value);
            api.LoadStateDataFromString(session);
            return api;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            if (e.PreviousExecutionState != ApplicationExecutionState.Running)
            {
                bool loadState = (e.PreviousExecutionState == ApplicationExecutionState.Terminated);
                ExtendedSplashScreen extendedSplash = new(e.SplashScreen, e, loadState);
                Window.Current.Content = extendedSplash;
            }
            Window.Current.Activate();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {

            base.OnActivated(args);
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
