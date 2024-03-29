﻿using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views.Medias;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Media
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SingleInstaMediaView : Page, ISingleInstaMediaView
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          nameof(Media),
          typeof(InstaMedia),
          typeof(SingleInstaMediaView),
          new PropertyMetadata(null));

        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        public SingleInstaMediaView()
        {
            this.InitializeComponent();
        }

        void GoBack(string para)
        {
            if (Frame.CanGoBack)
                Frame.GoBack();
            throw new ArgumentOutOfRangeException(para);
        }

        async void LoadMediaById(string mediaPk)
        {
            using (IInstaApi Api = App.Container.GetService<IInstaApi>())
            {
                var res = await Api.MediaProcessor.GetMediaByIdAsync(mediaPk.ToString());
                if (!res.Succeeded)
                    GoBack(nameof(mediaPk));
                Media = res.Value;
                return;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is long mediaid)
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    LoadMediaById($"{mediaid}");
                    return;
                }
            }
            if (e.Parameter is string url)
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var res = await Api.MediaProcessor.GetMediaIdFromUrlAsync(new(url));
                    if (!res.Succeeded)
                        GoBack(nameof(e.Parameter));
                    LoadMediaById(res.Value);
                    return;
                }
            }
            if (e.Parameter is not InstaMedia media)
            {
                GoBack(nameof(e.Parameter));
            }
            else Media = media;
            base.OnNavigatedTo(e);
        }
    }
}
