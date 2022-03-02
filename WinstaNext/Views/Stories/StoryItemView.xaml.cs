using InstagramApiSharp.API;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class StoryItemView : UserControl
    {
        public static readonly DependencyProperty StoryItemProperty = DependencyProperty.Register(
          "StoryItem",
          typeof(WinstaStoryItem),
          typeof(StoryItemView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnStoryItemChanged))]
        public WinstaStoryItem StoryItem
        {
            get { return (WinstaStoryItem)GetValue(StoryItemProperty); }
            set { SetValue(StoryItemProperty, value); }
        }

        public StoryItemView()
        {
            this.InitializeComponent();
        }

        async void OnStoryItemChanged()
        {
            if(!StoryItem.ReelFeed.Items.Any())
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.StoryProcessor.GetUserStoryFeedAsync(StoryItem.ReelFeed.User.Pk);
                    if(!result.Succeeded)
                    {
                        
                    }
                }
            }
        }

    }
}
