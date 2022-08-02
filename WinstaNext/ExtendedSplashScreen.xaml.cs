using Microsoft.Toolkit.Uwp.Helpers;
using Resources;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Theme;
using WinstaNext.Views.Account;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtendedSplashScreen : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        LaunchActivatedEventArgs launchActivatedEventArgs;
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;

        public ExtendedSplashScreen(SplashScreen splashScreen, LaunchActivatedEventArgs e, bool loadState)
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(ExtendedSplash_OnResize);
            launchActivatedEventArgs = e;
            splash = splashScreen;
            if (splash != null)
            {
                // Register an event handler to be executed when the splash screen has been dismissed.
                splash.Dismissed += new TypedEventHandler<SplashScreen, Object>(DismissedEventHandler);

                // Retrieve the window coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();

                // If applicable, include a method for positioning a progress control.
                PositionRing();
            }

            // Create a Frame to act as the navigation context
            rootFrame = new Frame();
            this.Loaded += ExtendedSplashScreen_Loaded;
            //ExtendedSplash_OnResize(null, null);

            GetBetaVersionAvailability();
        }

        async void GetBetaVersionAvailability()
        {
            try
            {
                var http = new HttpClient();
                var str = await http.GetStringAsync(new Uri("http://worldtimeapi.org/api/timezone/Etc/UTC", UriKind.RelativeOrAbsolute));
                var json = Newtonsoft.Json.Linq.JObject.Parse(str);
                var datetime = json.Value<DateTime>("datetime");
                var end = new DateTime(2022, 09, 30);
                var rdays = end.Subtract(datetime).Days;
                if (rdays < 0)
                    App.Current.Exit();
            }
            catch
            {
                App.Current.Exit();
            }
        }

        void ExtendedSplash_OnResize(Object sender, WindowSizeChangedEventArgs e)
        {
            // Safely update the extended splash screen image coordinates. This function will be fired in response to snapping, unsnapping, rotation, etc...
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }

        private async void ExtendedSplashScreen_Loaded(object sender, RoutedEventArgs e)
        {
            InitializeUI(rootFrame);

            await Task.Delay(100);

            RegisterQuickReplyBgTask();

            if (launchActivatedEventArgs.PrelaunchActivated == false)
            {
                TryEnablePrelaunch();
                Window.Current.Content = rootFrame;
                ((App)App.Current).SetContainer();
                NavigateRootFrame(rootFrame, launchActivatedEventArgs.Arguments);

                // Ensure the current window is active
                Window.Current.Activate();
            }

            ApplicationViewScaling.TrySetDisableLayoutScaling(false);
        }

        #region Extended Splash screen
        void PositionImage()
        {
            extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            extendedSplashImage.Height = splashImageRect.Height;
            extendedSplashImage.Width = splashImageRect.Width;
        }

        void PositionRing()
        {
            splashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (splashProgressRing.Width * 0.5));
            splashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        void DismissedEventHandler(SplashScreen sender, object e)
        {
            dismissed = true;

            // Complete app setup operations here...
        }

        #endregion

        void InitializeUI(Frame rootFrame)
        {
            if (LanguageManager.Instance.General.IsRightToLeft)
                rootFrame.FlowDirection = FlowDirection.RightToLeft;

            XboxBetterExperience();

            ManageTheme(rootFrame);
        }

        private void TryEnablePrelaunch()
        {
            CoreApplication.EnablePrelaunch(true);
        }

        private async void NavigateRootFrame(Frame rootFrame, object args)
        {
            if (ApplicationSettingsManager.Instance.GetShowLoginScreen())
            {
                rootFrame.Navigate(typeof(LoginView), args);
                //rootFrame.Navigate(typeof(TwoFactorAuthView), args);
                return;
            }
            else
            {
                var lastpk = ApplicationSettingsManager.Instance.GetLastLoggedUser();
                //var firstPk = ApplicationSettingsManager.Instance.GetUsersList().FirstOrDefault().Key;
                var session = await ApplicationSettingsManager.Instance.GetUserSession(lastpk);
                ((App)App.Current).SetCurrentUserSession(session);
            }
            if (rootFrame.Content == null)
            {
                rootFrame.Navigate(typeof(MainPage), args);
            }
        }

        void XboxBetterExperience()
        {
            if (SystemInformation.Instance.DeviceFamily != "Windows.Xbox") return;

            //Disable Scaling screen
            //bool result = ApplicationViewScaling.TrySetDisableLayoutScaling(true);

            //Set screen bounds to the edges of the TV
            ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);
        }

        void ManageTheme(FrameworkElement element)
        {
            var theme = ApplicationSettingsManager.Instance.GetTheme();
            if (theme == AppTheme.Dark)
                element.RequestedTheme = ElementTheme.Dark;
            else if (theme == AppTheme.Light)
                element.RequestedTheme = ElementTheme.Light;
        }

        string ENTRY_POINT = "WinstaBackgroundTask.NotifyQuickReplyTask";
        public async void RegisterQuickReplyBgTask()
        {
            try
            {
                if (BackgroundTaskRegistration.AllTasks.Any(i => i.Value.Name.Equals(ENTRY_POINT)))
                {
                    return;
                }

                BackgroundAccessStatus status = await BackgroundExecutionManager.RequestAccessAsync();
                BackgroundTaskBuilder builder = new BackgroundTaskBuilder
                {
                    Name = ENTRY_POINT,
                    TaskEntryPoint = ENTRY_POINT
                };

                builder.SetTrigger(new ToastNotificationActionTrigger());

                builder.Register();
            }
            catch { }
        }
    }
}
