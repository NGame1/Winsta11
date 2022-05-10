using Microsoft.QueryStringDotNET;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using Windows.Data.Xml.Dom;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Notifications;

namespace WinstaNext.Helpers
{
    public class NotifyHelper
    {
        public static async void CreateNotifyEmpty(string subject, string content, string image)
        {
            XmlDocument ToastNotifyXML = new XmlDocument();
            var toastfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Templates/ToastNotification.xml", UriKind.RelativeOrAbsolute));
            var Text = await FileIO.ReadTextAsync(toastfile, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            Text = Text.Replace("SongName", subject);
            Text = Text.Replace("Artist", content);
            Text = Text.Replace("ThumbImage.png", "");
            //Text = Text.Replace("TrackID", item.id);
            Text = Text.Replace("&", "&amp;");
            ToastNotifyXML.LoadXml(Text);
            ToastNotification notification = new ToastNotification(ToastNotifyXML);
            await Task.Delay(1000);
            ToastNotificationManager.CreateToastNotifier().Show(notification);
        }

        public static async Task<ToastNotification> CreateNotifyEmptyAsync(string subject, string content, string image)
        {
            XmlDocument ToastNotifyXML = new XmlDocument();
            var toastfile = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Templates/ToastNotification.xml", UriKind.RelativeOrAbsolute));
            var Text = await FileIO.ReadTextAsync(toastfile, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            Text = Text.Replace("SongName", subject);
            Text = Text.Replace("Artist", content);
            Text = Text.Replace("ThumbImage.png", image);
            //Text = Text.Replace("TrackID", item.id);
            Text = Text.Replace("&", "&amp;");
            ToastNotifyXML.LoadXml(Text);
            return new(ToastNotifyXML);
        }

        public static void CreateNotifyEmpty2(string subject, string content)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = subject
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                }
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }


        public static void CreateNotifyLaunchAction(string subject, string content, string launchAction)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = subject
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                },
                Launch = "notifyAction=" + launchAction
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
        public static void CreateNotifyAction(string subject, string content, string img)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveText()
                            {
                                Text = subject
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = img,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        }
                    }
                },
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void CreateNotifyButtonAction(string subject, string content, string img, string action)
        {
            var toastContent = new ToastContent()
            {
                Visual = new ToastVisual()
                {
                    BindingGeneric = new ToastBindingGeneric()
                    {
                        Children =
                        {
                            new AdaptiveImage()
                            {
                                Source = img
                            },
                            new AdaptiveText()
                            {
                                Text = subject
                            },
                            new AdaptiveText()
                            {
                                Text = content
                            }
                        }
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Play", new QueryString()
                        {
                            { "action", "play" },
                            { "url", action }

                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Foreground
                        },
                        new ToastButton("Download", new QueryString()
                        {
                            { "action", "download" },
                            { "url", action }

                        }.ToString())
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = "actionFromNotify=" + action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
    }
}
