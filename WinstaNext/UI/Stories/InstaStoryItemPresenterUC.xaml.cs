using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Stories
{
    //[AddINotifyPropertyChangedInterface]
    public sealed partial class InstaStoryItemPresenterUC : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty StoryProperty = DependencyProperty.Register(
             "Story",
             typeof(InstaStoryItem),
             typeof(InstaStoryItemPresenterUC),
             new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        [OnChangedMethod(nameof(OnStoryChanged))]
        public InstaStoryItem Story
        {
            get { return (InstaStoryItem)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        public bool LoadImage { get; set; } = false;
        public bool LoadMediaElement { get; set; } = false;

        public InstaStoryItemPresenterUC()
        {
            this.InitializeComponent();
        }

        void OnStoryChanged()
        {
            LoadMediaElement = LoadImage = false;
            if (Story.MediaType == InstaMediaType.Video)
                LoadMediaElement = true;
            else LoadImage = true;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadImage)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(LoadMediaElement)));
        }
    }
}
