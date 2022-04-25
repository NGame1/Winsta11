using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using InstagramApiSharp.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using WinstaNext.Helpers.DownloadUploadHelper;

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
            DownloadHelper.Download(Media);
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
                        Text = LanguageManager.Instance.Instagram.CopyURL,
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
                    Text = LanguageManager.Instance.General.Download,
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
                        Text = LanguageManager.Instance.Instagram.CopyCaption,
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
                        Text = LanguageManager.Instance.Instagram.Archive,
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
                        Text = LanguageManager.Instance.Instagram.EditPost,
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
                        Text = LanguageManager.Instance.Instagram.DeletePost,
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
                            Text = LanguageManager.Instance.Instagram.DisableCommenting,
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
                            Text = LanguageManager.Instance.Instagram.EnableCommenting,
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
                        Text = LanguageManager.Instance.Instagram.MutingOptions
                    };
                    mutingoptions.Items.Add(new MenuFlyoutItem
                    {
                        Icon = new FontIcon()
                        {
                            Glyph = "\uFAA9",
                            FontFamily = FluentSystemIconsRegular,
                            FontSize = 24
                        },
                        Text = LanguageManager.Instance.Instagram.MutePosts,
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
                        Text = LanguageManager.Instance.Instagram.UnmutePosts,
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
                        Text = LanguageManager.Instance.Instagram.MuteStories,
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
                        Text = LanguageManager.Instance.Instagram.UnmuteStories,
                        Command = UnmuteStoriesCommand
                    });
                    Items.Add(mutingoptions);
                }
            }
        }
    }
}
