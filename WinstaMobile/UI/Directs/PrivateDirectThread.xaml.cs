﻿using InstagramApiSharp.Classes.Models;
using Mapster;
using Microsoft.Extensions.DependencyInjection;
using PropertyChanged;
using System.ComponentModel;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaMobile.UI.Directs
{
    public sealed partial class PrivateDirectThread : UserControl, INotifyPropertyChanged
    {
        public static readonly DependencyProperty DirectThreadProperty = DependencyProperty.Register(
               nameof(DirectThread),
               typeof(InstaDirectInboxThread),
               typeof(PrivateDirectThread),
               new PropertyMetadata(null));

        public event PropertyChangedEventHandler PropertyChanged;

        [OnChangedMethod(nameof(OnThreadChanged))]
        public InstaDirectInboxThread DirectThread
        {
            get { return (InstaDirectInboxThread)GetValue(DirectThreadProperty); }
            set { SetValue(DirectThreadProperty, value); }
        }

        public PrivateDirectThread()
        {
            this.InitializeComponent();
        }
        
        void OnThreadChanged()
        {
            if (!DirectThread.IsGroup && (DirectThread.Users == null || !DirectThread.Users.Any()))
            {
                DirectThread.Users.Add(App.Container.GetService<InstaUserShort>().Adapt<InstaUserShortFriendship>());
            }
        }

    }
}
