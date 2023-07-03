using Abstractions.Navigation;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Uwp.UI;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using WinstaCore;
using WinstaCore.Attributes;
using WinstaCore.Services;
using WinstaCore.Interfaces.Views.Medias;
using WinstaCore.Enums;
using Windows.UI.Xaml.Markup;
#nullable enable

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.Controls
{
    internal class MediaTileTemplateSelector : DataTemplateSelector
    {
        public DataTemplate StaggeredTileTemplate { get; set; } = new DataTemplate();
        public DataTemplate ListTileTemplate { get; set; } = new DataTemplate();

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (ApplicationSettingsManager.Instance.GetAppUiMode() == WinstaCore.Enums.ApplicationUserInterfaceModel.Staggered)
                return StaggeredTileTemplate;
            return ListTileTemplate;
        }

        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            return this.SelectTemplateCore(item);
        }
    }

    public partial class WinstaListView : UserControl
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
               nameof(Medias),
               typeof(RangePlayerAttribute),
               typeof(WinstaListView),
               new PropertyMetadata(null));

        public RangePlayerAttribute? Medias
        {
            get { return (RangePlayerAttribute)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        public AsyncRelayCommand LoadMoreCommand { get; }

        internal ScrollViewer? Scroll { get; private set; }

        private ApplicationUserInterfaceModel ApplicationUiMode { get; set; }
        private ItemsWrapGrid? WrapGrid { get; set; }

        public WinstaListView()
        {
            this.InitializeComponent();
            LoadMoreCommand = new(LoadMoreAsync);
        }

        async Task LoadMoreMediaAsync()
        {
            if(Medias == null) { await Task.Delay(1000);return; }
            await Medias?.LoadMoreItemsAsync(1);
        }

        async Task LoadMoreAsync()
        {
            await LoadMoreMediaAsync();
        }

        bool NeedsLoadMore()
        {
            try
            {
                if (Scroll == null) return false;
                return Scroll.ViewportHeight > Scroll.ScrollableHeight;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async void FeedPostsList_Loaded(object sender, RoutedEventArgs e)
        {
            ApplicationUiMode = ApplicationSettingsManager.Instance.GetAppUiMode();
            if (ApplicationUiMode == ApplicationUserInterfaceModel.List)
            {
                if (FeedPostsList.ItemsPanelRoot is ItemsWrapGrid) return;
                FeedPostsList.ItemsPanel = null;
                string Xaml =
                    @"<ItemsPanelTemplate xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
                            <ItemsWrapGrid SizeChanged=""FeedPostsList_SizeChanged"" 
                                           Loaded=""ItemsWrapGrid_Loaded""
                                           HorizontalAlignment=""Center""
                                           MaximumRowsOrColumns=""10""
                                           Orientation=""Horizontal""/>
                    </ItemsPanelTemplate>";
                FeedPostsList.ItemsPanel = XamlReader.Load(Xaml) as ItemsPanelTemplate;
                FeedPostsList.ApplyTemplate();
                FeedPostsList.UpdateLayout();
                var IWG = (FeedPostsList.ItemsPanelRoot as ItemsWrapGrid);
                IWG!.Loaded += ItemsWrapGrid_Loaded;
                IWG.SizeChanged += FeedPostsList_SizeChanged;
                FeedPostsList_SizeChanged(null, null);
                IWG.UpdateLayout();
                FeedPostsList.UpdateLayout();
            }

            Scroll = FeedPostsList.FindDescendantOrSelf<ScrollViewer>();
            if (Scroll == null) return;
            Scroll.ViewChanged += Scroll_ViewChanged;

            while (NeedsLoadMore())
            {
                await LoadMoreAsync();
                // a little Wait to load thumbnails
                await Task.Delay(1000);
            }
        }

        private void FeedPostsList_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (e.ClickedItem is not InstaMedia media) return;
            var NavigationService = App.Container.GetRequiredService<NavigationService>();
            var mediaView = App.Container.GetRequiredService<IIncrementalInstaMediaView>();
            NavigationService.Navigate(mediaView, new IncrementalMediaViewParameter(Medias, media));
        }

        async void Scroll_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            Scroll ??= (ScrollViewer)sender;
            if (Scroll.VerticalOffset >= Scroll.ScrollableHeight - 400
                && !LoadMoreCommand.IsRunning)
                await LoadMoreCommand.ExecuteAsync(null);
        }

        private void FeedPostsList_SizeChanged(object? sender, SizeChangedEventArgs? e)
        {
            if (WrapGrid == null) return;
            if (!ApplicationSettingsManager.Instance.GetForceThreeColumns())
            {
                WrapGrid.ItemHeight = WrapGrid.ItemWidth = 185;
                WrapGrid.SizeChanged -= FeedPostsList_SizeChanged;
                FeedPostsList.SizeChanged -= FeedPostsList_SizeChanged;
                return;
            }
            if (e != null)
            {
                var width = e.NewSize.Width / 3f;
                WrapGrid.ItemHeight = WrapGrid.ItemWidth = width;
            }
            else
            {
                if (double.IsNaN(FeedPostsList.ActualWidth)) return;
                var width = FeedPostsList.ActualWidth / 3f;
                WrapGrid.ItemHeight = WrapGrid.ItemWidth = width;
            }
        }

        private void ItemsWrapGrid_Loaded(object sender, RoutedEventArgs e)
        {
            WrapGrid = (ItemsWrapGrid)sender;
        }
    }
}
