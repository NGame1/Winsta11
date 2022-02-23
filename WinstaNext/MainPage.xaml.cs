using WinstaNext.Core.Messages;
using WinstaNext.Core.Navigation;
using Microsoft.Toolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Core;
using WinstaNext.Views;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinstaNext
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : BasePage, 
        IRecipient<ChangePageHeaderMessage>,
        IRecipient<NavigateToPageMessage>
    {
        private const string CompactOverlayStateName = "CompactOverlay";
        private const string NavigationViewExpandedStateName = "NavigationViewExpanded";
        private const string NavigationViewCompactStateName = "NavigationViewCompact";

        public MainPage()
        {
            this.InitializeComponent();
            WeakReferenceMessenger.Default.RegisterAll(this);
            NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Left;
            Window.Current.SetTitleBar(AppTitleBar);
            Loaded += MainPage_Loaded;
            SizeChanged += MainPage_SizeChanged;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            this.Loaded -= MainPage_Loaded;

            ViewModel.NavigationService.SetNavigationFrame(ContentFrame);

            VisualStateManager.GoToState(this, NavigationViewExpandedStateName, useTransitions: true);

            UpdateVisualState();

            SearchBox.Focus(FocusState.Keyboard);

            // Workaround for a bug where opening the window in compact display mode will misalign the content layout.
            NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;

            ViewModel.SelectedMenuItem = ViewModel.MenuItems.FirstOrDefault();

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateVisualState();
        }

        private void NavigationView_DisplayModeChanged(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewDisplayModeChangedEventArgs args)
        {
            ViewModel.NavigationViewDisplayMode = NavigationView.DisplayMode;
            UpdateVisualState();
        }

        private void NavigationView_PaneClosing(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewPaneClosingEventArgs args)
        {
            ViewModel.IsNavigationViewPaneOpened = false;
            ViewModel.NavigationViewDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Compact;
            UpdateVisualState();
        }

        private void NavigationView_PaneOpening(Microsoft.UI.Xaml.Controls.NavigationView sender, object args)
        {
            ViewModel.IsNavigationViewPaneOpened = true;
            ViewModel.NavigationViewDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewDisplayMode.Expanded;
            UpdateVisualState();
        }

        private void NavigationView_Loaded(object sender, RoutedEventArgs e)
        {
            ViewModel.IsNavigationViewPaneOpened = NavigationView.IsPaneOpen;
            UpdateVisualState();
        }

        private void UpdateVisualState()
        {
            var view = ApplicationView.GetForCurrentView();
            bool isCompactOverlayMode = view.ViewMode == ApplicationViewMode.CompactOverlay;

            if (isCompactOverlayMode)
            {
                VisualStateManager.GoToState(this, CompactOverlayStateName, useTransitions: true);
            }
            else
            {
                switch ((NavigationViewDisplayMode)ViewModel.NavigationViewDisplayMode)
                {
                    case NavigationViewDisplayMode.Compact:
                        VisualStateManager.GoToState(this, NavigationViewCompactStateName, useTransitions: true);
                        break;

                    case NavigationViewDisplayMode.Expanded:
                        VisualStateManager.GoToState(this, NavigationViewExpandedStateName, useTransitions: true);
                        break;
                }
            }
        }

        public void Receive(NavigateToPageMessage message)
        {
            ContentFrame.Navigate(
                message.View,
                new NavigationParameter(message.Parameter),
                new EntranceNavigationTransitionInfo());
        }

        private void SearchBoxKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
        {
            SearchBox.Focus(FocusState.Keyboard);
        }

        public void Receive(ChangePageHeaderMessage message)
        {
            NavigationView.Header = message.Title;
            NavigationView.AlwaysShowHeader = message.ShowHeader;
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            if (ViewModel.NavigationService.CanGoBack)
            {
                ViewModel.NavigationService.GoBack();
                e.Handled = true;
            }
        }

        private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
        {
            if (ViewModel.NavigationService.CanGoBack)
                ViewModel.NavigationService.GoBack();
        }
    }
}
