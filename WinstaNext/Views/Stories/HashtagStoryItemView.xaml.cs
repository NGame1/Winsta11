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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Stories
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class HashtagStoryItemView : Page
    {
        public static readonly DependencyProperty HashtagStoryItemProperty = DependencyProperty.Register(
          "HashtagStoryItem",
          typeof(HashtagStoryItemView),
          typeof(HashtagStoryItemView),
          new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnHashtagStoryItemChanged))]
        public InstaHashtagStory HashtagStoryItem
        {
            get { return (InstaHashtagStory)GetValue(HashtagStoryItemProperty); }
            set { SetValue(HashtagStoryItemProperty, value); }
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
