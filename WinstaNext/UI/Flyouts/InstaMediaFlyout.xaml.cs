using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Networking.BackgroundTransfer;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.UI.Flyouts
{
    public sealed partial class InstaMediaFlyout : MenuFlyout
    {
        public static readonly DependencyProperty MediaProperty = DependencyProperty.Register(
          "Media",
          typeof(InstaMedia),
          typeof(InstaMediaFlyout),
          new PropertyMetadata(null));

        public InstaMedia Media
        {
            get { return (InstaMedia)GetValue(MediaProperty); }
            set { SetValue(MediaProperty, value); }
        }

        AsyncRelayCommand ArchiveCommand { get; set; }
        RelayCommand CopyUrlCommand { get; set; }
        RelayCommand CopyCaptionCommand { get; set; }
        RelayCommand DownloadContentCommand { get; set; }
        RelayCommand DeletePostCommand { get; set; }
        AsyncRelayCommand EnableCommentingCommand { get; set; }
        AsyncRelayCommand DisableCommentingCommand { get; set; }
        AsyncRelayCommand MutePostsCommand { get; set; }
        AsyncRelayCommand UnmutePostsCommand { get; set; }
        AsyncRelayCommand MuteStoriesCommand { get; set; }
        AsyncRelayCommand UnmuteStoriesCommand { get; set; }
        RelayCommand EditPostCommand { get; set; }

        public InstaMediaFlyout()
        {
            ArchiveCommand = new(ArchiveAsync);
            CopyUrlCommand = new(CopyUrl);
            CopyCaptionCommand = new(CopyCaption);
            DownloadContentCommand = new(DownloadContent);
            EnableCommentingCommand = new(EnableCommentingAsync);
            DisableCommentingCommand = new(DisableCommentingAsync);
            MutePostsCommand = new(MutePostsAsync);
            UnmutePostsCommand = new(UnmutePostsAsync);
            MuteStoriesCommand = new(MuteStoriesAsync);
            UnmuteStoriesCommand = new(UnmuteStoriesAsync);
            EditPostCommand = new(EditPost);
            this.InitializeComponent();
            Opening += InstaMediaFlyout_Opening;
        }

        async Task ArchiveAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.MediaProcessor.ArchiveMediaAsync(Media.InstaIdentifier);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        void CopyUrl()
        {
            var data = new DataPackage();
            data.SetText(Media.Url);
            //data.SetText("https://instagram.com/p/" + Media.Code);
            Clipboard.SetContent(data);
            Hide();
        }

        void CopyCaption()
        {
            var data = new DataPackage();
            data.SetText(Media.Caption.Text);
            Clipboard.SetContent(data);
            Hide();
        }

        void DownloadContent()
        {
            var bgdl = App.Container.GetService<BackgroundDownloader>();
            switch (Media.MediaType)
            {
                case InstaMediaType.Image:
                    DownloadImage(Media.Images[0].Uri);
                    break;
                case InstaMediaType.Video:
                    DownloadVideo(Media.Videos[0].Uri);
                    break;
                case InstaMediaType.Carousel:
                    for (int i = 0; i < Media.Carousel.Count; i++)
                    {
                        var ci = Media.Carousel.ElementAt(i);
                        if (ci.MediaType == InstaMediaType.Image)
                            DownloadImage(ci.Images[0].Uri);
                        else
                            DownloadVideo(ci.Videos[0].Uri);
                    }
                    break;
            }
        }

        async void DownloadImage(string imageUri)
        {
            var bgdl = App.Container.GetService<BackgroundDownloader>();
            var WinstaFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Winsta", CreationCollisionOption.OpenIfExists);
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.jpg", CreationCollisionOption.GenerateUniqueName);
            if (Uri.TryCreate(imageUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                bgdl.TransferGroup = BackgroundTransferGroup.CreateGroup(Media.InstaIdentifier);
                var dl = bgdl.CreateDownload(uri, desfile);
                dl.StartAsync().AsTask();
            }
        }

        async void DownloadVideo(string videoUri)
        {
            var bgdl = App.Container.GetService<BackgroundDownloader>();
            var WinstaFolder = await KnownFolders.PicturesLibrary.CreateFolderAsync("Winsta", CreationCollisionOption.OpenIfExists);
            var userDownloads = await WinstaFolder.CreateFolderAsync(Media.User.UserName, CreationCollisionOption.OpenIfExists);
            var desfile = await userDownloads.CreateFileAsync($"{Media.InstaIdentifier}.mp4", CreationCollisionOption.GenerateUniqueName);
            if (Uri.TryCreate(videoUri, UriKind.RelativeOrAbsolute, out var uri))
            {
                bgdl.TransferGroup = BackgroundTransferGroup.CreateGroup(Media.InstaIdentifier);
                var dl = bgdl.CreateDownload(uri, desfile);
                dl.StartAsync().AsTask();
            }
        }

        async Task EnableCommentingAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.CommentProcessor.EnableMediaCommentAsync(Media.InstaIdentifier);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        async Task DisableCommentingAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.CommentProcessor.DisableMediaCommentAsync(Media.InstaIdentifier);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        async Task MutePostsAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.MuteUserMediaAsync(Media.User.Pk, InstaMuteOption.Post);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        async Task UnmutePostsAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.UnMuteUserMediaAsync(Media.User.Pk, InstaMuteOption.Post);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        async Task MuteStoriesAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.MuteFriendStoryAsync(Media.User.Pk);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        async Task UnmuteStoriesAsync()
        {
            try
            {
                using (IInstaApi Api = App.Container.GetService<IInstaApi>())
                {
                    var result = await Api.UserProcessor.UnMuteFriendStoryAsync(Media.User.Pk);
                    if (!result.Succeeded) throw result.Info.Exception;
                }
            }
            finally { Hide(); }
        }

        void EditPost()
        {
            throw new NotImplementedException();
        }

        private void InstaMediaFlyout_Opening(object sender, object e)
        {
            if (Media != null)
            {
                Items.Clear();

                var Me = App.Container.GetService<InstaUserShort>();
                var font = (FontFamily)App.Current.Resources["FluentIcons"];
                var FluentSystemIconsRegular = (FontFamily)App.Current.Resources["FluentSystemIconsRegular"];

                if (!Media.User.IsPrivate)
                {
                    Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uE167",
                            FontFamily = font,
                            FontSize = 24
                        },
                        Text = "Copy URL",
                        Command = CopyUrlCommand
                    });
                    Items.Add(new MenuFlyoutSeparator());
                }

                Items.Add(new MenuFlyoutItem
                {
                    Icon = new FontIcon()
                    {
                        Glyph = "\uE118",
                        FontFamily = font,
                        FontSize = 24
                    },
                    Text = "Download content",
                    Command = DownloadContentCommand
                });
                Items.Add(new MenuFlyoutSeparator());

                if (Media.Caption != null && !string.IsNullOrEmpty(Media.Caption.Text))
                {
                    Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uE16F",
                            FontFamily = font,
                            FontSize = 24
                        },
                        Text = "Copy caption",
                        Command = CopyCaptionCommand
                    });
                    Items.Add(new MenuFlyoutSeparator());
                }
                if (Media.User.Pk == Me.Pk)
                {
                    Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uE1D3",
                            FontFamily = font,
                            FontSize = 24
                        },
                        Text = "Archive",
                        Command = ArchiveCommand
                    });
                    Items.Add(new MenuFlyoutItem()
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uE104",
                            FontFamily = font,
                            FontSize = 24
                        },
                        Text = "Edit Post",
                        Command = EditPostCommand
                    });
                    Items.Add(new MenuFlyoutItem()
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uE107",
                            FontFamily = font,
                            FontSize = 24
                        },
                        Text = "Delete Post",
                        Command = DeletePostCommand
                    });
                    Items.Add(new MenuFlyoutSeparator());
                    if (!Media.IsCommentsDisabled)
                        Items.Add(new MenuFlyoutItem
                        {
                            Icon = new FontIcon
                            {
                                Glyph = "\uF998",
                                FontFamily = FluentSystemIconsRegular,
                                FontSize = 24
                            },
                            Text = "Disable commenting",
                            Command = DisableCommentingCommand
                        });
                    else
                        Items.Add(new MenuFlyoutItem
                        {
                            Icon = new FontIcon
                            {
                                Glyph = "\uF97E",
                                FontFamily = FluentSystemIconsRegular,
                                FontSize = 24
                            },
                            Text = "Enable commenting",
                            Command = EnableCommentingCommand
                        });
                }
                else
                {
                    var mutingoptions = new MenuFlyoutSubItem()
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uFAA9",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = "Muting Options"
                    };
                    mutingoptions.Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uFAA9",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = "Mute Posts",
                        Command = MutePostsCommand
                    });
                    mutingoptions.Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\u0176",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = "Unmute Posts",
                        Command = UnmutePostsCommand
                    });
                    mutingoptions.Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uFAA9",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = "Mute Stories",
                        Command = MuteStoriesCommand
                    });
                    mutingoptions.Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\u0176",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = "Unmute Stories",
                        Command = UnmuteStoriesCommand
                    });
                    Items.Add(mutingoptions);
                }
            }
        }
    }
}
