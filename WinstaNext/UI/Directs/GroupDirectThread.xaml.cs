﻿using InstagramApiSharp.Classes.Models;
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

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Directs
{
    public sealed partial class GroupDirectThread : UserControl
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
          nameof(DirectThread),
          typeof(InstaDirectInboxThread),
          typeof(GroupDirectThread),
          new PropertyMetadata(null));

        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set { SetValue(DirectThreadProperty, value); }
        }
        public GroupDirectThread()
        {
            this.InitializeComponent();
        }
    }
}
