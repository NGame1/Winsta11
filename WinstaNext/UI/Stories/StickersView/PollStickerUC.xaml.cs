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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.UI.Stories.StickersView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public sealed partial class PollStickerUC : UserControl
    {
        public static readonly DependencyProperty PollProperty = DependencyProperty.Register(
                "Poll",
                typeof(InstaStoryPollItem),
                typeof(PollStickerUC),
                new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnPollChanged))]
        public InstaStoryPollItem Poll
        {
            get { return (InstaStoryPollItem)GetValue(PollProperty); }
            set { SetValue(PollProperty, value); }
        }

        public PollStickerUC()
        {
            this.InitializeComponent();
        }

        void OnPollChanged()
        {
            if (string.IsNullOrEmpty(Poll.PollSticker.Question))
                titleRow.Height = new(0);
        }
    }
}
