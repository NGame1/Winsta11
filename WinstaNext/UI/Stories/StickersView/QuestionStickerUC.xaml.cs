using InstagramApiSharp.Classes.Models;
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

namespace WinstaNext.UI.Stories.StickersView
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class QuestionStickerUC : UserControl
    {
        public static readonly DependencyProperty QuestionProperty = DependencyProperty.Register(
                nameof(Question),
                typeof(InstaStoryQuestionItem),
                typeof(QuestionStickerUC),
                new PropertyMetadata(null));

        public InstaStoryQuestionItem Question
        {
            get { return (InstaStoryQuestionItem)GetValue(QuestionProperty); }
            set { SetValue(QuestionProperty, value); }
        }

        public QuestionStickerUC()
        {
            this.InitializeComponent();
        }
    }
}
