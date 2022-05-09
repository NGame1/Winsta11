using InstagramApiSharp.Classes.Models;
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
    public sealed partial class HashtagStoryItemView : Page
    {

        public static readonly DependencyProperty StoryRootProperty = DependencyProperty.Register(
          "StoryRoot",
          typeof(WinstaStoryItem),
          typeof(HashtagStoryItemView),
          new PropertyMetadata(null));

        public WinstaStoryItem StoryRoot
        {
            get { return (WinstaStoryItem)GetValue(StoryRootProperty); }
            set { SetValue(StoryRootProperty, value); }
        }

        [OnChangedMethod(nameof(OnHashtagStoryItemChanged))]
        public InstaHashtagStory HashtagStoryItem
        {
            get => StoryRoot.HashtagStory;
        }

        public HashtagStoryItemView()
        {
            this.InitializeComponent();
        }

        void OnHashtagStoryItemChanged()
        {

        }
    }
}
