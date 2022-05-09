using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Abstractions.Stories;
using WinstaNext.Services;
using WinstaNext.Views.Profiles;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    public sealed partial class InstaReelFeedPresenterUC : UserControl
    {
        public static readonly DependencyProperty ReelFeedProperty = DependencyProperty.Register(
             "ReelFeed",
             typeof(WinstaReelFeed),
             typeof(InstaReelFeedPresenterUC),
             new PropertyMetadata(null));

        public WinstaReelFeed ReelFeed
        {
            get { return (WinstaReelFeed)GetValue(ReelFeedProperty); }
            set { SetValue(ReelFeedProperty, value); }
        }

        public RelayCommand<object> NavigateToUserProfileCommand { get; set; }

        public InstaReelFeedPresenterUC()
        {
            NavigateToUserProfileCommand = new(NavigateToUserProfile);
            this.InitializeComponent();
        }

        void NavigateToUserProfile(object obj)
        {
            var NavigationService = App.Container.GetService<NavigationService>();
            NavigationService.Navigate(typeof(UserProfileView), obj);
        }

    }
}
