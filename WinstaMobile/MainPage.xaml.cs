﻿using System;
using System.Linq;
using Windows.Security.Credentials.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using WinstaCore.Enums;
using WinstaCore;
using WinstaCore.Interfaces.Views;
using WinstaMobile.Views;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WinstaMobile;

/// <summary>
/// An empty page that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainPage : BasePage, IMainView
{
    private const string CompactOverlayStateName = "CompactOverlay";
    private const string NavigationViewExpandedStateName = "NavigationViewExpanded";
    private const string NavigationViewCompactStateName = "NavigationViewCompact";

    public override string PageHeader { get; protected set; }

    public MainPage()
    {
        this.InitializeComponent();
        NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.LeftCompact;
        Window.Current.SetTitleBar(AppTitleBar);
        Loaded += MainPage_Loaded;
        SizeChanged += MainPage_SizeChanged;
        App.Current.LeavingBackground += Current_LeavingBackground;
    }

    async void Current_LeavingBackground(object sender, Windows.ApplicationModel.LeavingBackgroundEventArgs e)
    {
        var content = Window.Current.Content;
        Window.Current.Content = null;

        await AppLock();
        Window.Current.Content = content;
    }

    async void MainPage_Loaded(object sender, RoutedEventArgs e)
    {
        this.Loaded -= MainPage_Loaded;

        await AppLock();

        ViewModel.NavigationService.SetNavigationFrame(ContentFrame);

        VisualStateManager.GoToState(this, NavigationViewExpandedStateName, useTransitions: true);

        UpdateVisualState();

        SearchBox.Focus(FocusState.Keyboard);

        // Workaround for a bug where opening the window in compact display mode will misalign the content layout.
        NavigationView.PaneDisplayMode = Microsoft.UI.Xaml.Controls.NavigationViewPaneDisplayMode.Auto;

        ViewModel.SelectedMenuItem = ViewModel.MenuItems.FirstOrDefault();
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

    private void SearchBoxKeyboardAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        SearchBox.Focus(FocusState.Keyboard);
    }

    private void NavigationView_BackRequested(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewBackRequestedEventArgs args)
    {
        if (ViewModel.NavigationService.CanGoBack)
            ViewModel.NavigationService.GoBack();
    }

    private void EscapeAccelerator_Invoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        if (ViewModel.NavigationService.CanGoBack)
            ViewModel.NavigationService.GoBack();
        args.Handled = true;
    }

    private void ContentFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
    {
        if (e.Content is BasePage page)
        {
            NavigationView.Header = page.PageHeader;
            NavigationView.AlwaysShowHeader = !string.IsNullOrEmpty(page.PageHeader);
        }
    }

    private void NavigationView_ItemInvoked(Microsoft.UI.Xaml.Controls.NavigationView sender, Microsoft.UI.Xaml.Controls.NavigationViewItemInvokedEventArgs args)
    {
        ViewModel.SelectedMenuItemChanged();
    }

    async Task AppLock()
    {
        if (ApplicationSettingsManager.Instance.GetAppLockEnabled())
        {
            if (ApplicationSettingsManager.Instance.GetAppLockMode() == ApplicationLockMode.WindowsHello)
            {
                if (await UserConsentVerifier.CheckAvailabilityAsync() == UserConsentVerifierAvailability.Available)
                {
                    if (await UserConsentVerifier.RequestVerificationAsync("Device owner verification") == UserConsentVerificationResult.Verified)
                    {

                    }
                    else App.Current.Exit();
                }
            }
            else
            {
                //await new Dialogs.AppPasscodeVerificationDialog().ShowAsync();
            }
        }
    }

}
