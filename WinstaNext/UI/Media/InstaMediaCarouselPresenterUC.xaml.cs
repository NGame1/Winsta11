using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaNext.Helpers;
using WinstaNext.Services;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Media
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class InstaMediaCarouselPresenterUC : UserControl
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          "Media",
          typeof(InstaMedia),
          typeof(InstaMediaCarouselPresenterUC),
          new PropertyMetadata(null));

        public static readonly DependencyProperty FlipHeightProperty = DependencyProperty.Register(
          "FlipHeight",
          typeof(double),
          typeof(InstaMediaCarouselPresenterUC),
          new PropertyMetadata(null));

        public static readonly DependencyProperty FlipWidthProperty = DependencyProperty.Register(
          "FlipWidth",
          typeof(double),
          typeof(InstaMediaCarouselPresenterUC),
          new PropertyMetadata(null));

        public double FlipHeight
        {
            get { return (double)GetValue(FlipHeightProperty); }
            set { SetValue(FlipHeightProperty, value); }
        }

        public double FlipWidth
        {
            get { return (double)GetValue(FlipWidthProperty); }
            set { SetValue(FlipWidthProperty, value); }
        }

        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        NavigationService NavigationService { get; }

        public InstaMediaCarouselPresenterUC()
        {
            NavigationService = App.Container.GetService<NavigationService>();
            this.InitializeComponent();
        }
        
        private void Gallery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SetFlipViewSize();
        }

        public void SetFlipViewSize()
        {
            if (Gallery.SelectedItem == null) return;
            var parentelement = (NavigationService.Content as FrameworkElement);
            if (Gallery.SelectedItem is InstaCarouselItem item)
            {
                var minwidth = parentelement.ActualWidth < this.ActualWidth ?
                parentelement.ActualWidth : this.ActualWidth;
                var s = ControlSizeHelper.CalculateSizeInBox(item.Width, item.Height,
                    parentelement.ActualHeight - 150, minwidth);
                FlipHeight = s.Height;
                FlipWidth = s.Width;
            }
        }

        private async void InstaMediaVideoPresenterUC_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (Dispatcher.HasThreadAccess)
                SlideAndPlay();
            else await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, SlideAndPlay);
        }

        void SlideAndPlay()
        {
            var currentIndex = Gallery.SelectedIndex;
            if (Gallery.Items.Count - 1 == currentIndex) return;
            currentIndex++;
            Gallery.SelectedIndex = currentIndex;
            var fvi = (FlipViewItem)Gallery.ContainerFromIndex(currentIndex);
            if (fvi.ContentTemplateRoot is InstaMediaVideoPresenterUC videoPresenter)
            {
                videoPresenter.mediaPlayer.Play();
            }
        }

    }
}
