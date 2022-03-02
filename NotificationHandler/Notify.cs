using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace NotificationHandler
{
    public static class Notify
    {
        public static void SendMessageNotify(string name, string message, string image, string action, string heroImage = null)
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
                                Text = name ?? "",
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = message ?? ""
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "reply"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", action)
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Icons/send.png",
                            TextBoxId = "textBox"
                        }
                    }
                },
                Launch = action
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());

            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendMessageWithoutTextNotify(string name, string message, string image, string action, string heroImage = null)
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
                                Text = name,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Open", action)
                        {
                            ActivationType = ToastActivationType.Background,
                        },
                        new ToastButton("Dismiss", "dismiss=True")
                        {
                            ActivationType = ToastActivationType.Background,
                        }
                    }
                },
                Launch = action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendLikeNotify(string name, string message, string image, string action, string heroImage = null)
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
                                Text = name,
                                HintMaxLines = 1
                            },
                            new AdaptiveText()
                            {
                                Text =message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Launch = action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendPrivateFollowRequestNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Accept", action + "&action=acceptFriendshipRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Decline", action + "&action=declineFriendshipRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Open profile", action + "&action=openProfile")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendLiveBroadcastNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Open live", action)
                        {
                            ActivationType = ToastActivationType.Foreground
                        },
                        new ToastButton("Dismiss", "dismiss=True")
                        {
                            ActivationType = ToastActivationType.Background,
                        }
                    }
                },
                Launch = action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendDirectPendingRequestNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Accept", action + "&action=acceptDirectRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Delete", action + "&action=deleteDirectRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Block", action + "&action=blockDirectRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Open", action + "&action=openPendingThread")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = action
            };

            // Create the toast notification
            var toastNotif = new ToastNotification(toastContent.GetXml());

            // And send the notification
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
        public static void SendNewPostNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Send a comment"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", action + "&action=commentMedia")
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Icons/send.png",
                            TextBoxId = "textBox"
                        },
                        new ToastButton("Like", action + "&action=likeMedia")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Open", action + "&action=openMedia")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = action
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
        public static void SendCommentNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Reply to comment"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", action + "&action=commentMedia")
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Icons/send.png",
                            TextBoxId = "textBox"
                        },
                        new ToastButton("Like", action + "&action=likeComment")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Open", action + "&action=openComment")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = action
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }
        public static void SendNewIgtvNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image,
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Inputs =
                    {
                        new ToastTextBox("textBox")
                        {
                            PlaceholderContent = "Send a comment"
                        }
                    },
                    Buttons =
                    {
                        new ToastButton("Send", action + "&action=commentMedia")
                        {
                            ActivationType = ToastActivationType.Background,
                            ImageUri = "Assets/Icons/send.png",
                            TextBoxId = "textBox"
                        },
                        new ToastButton("Like", action + "&action=likeMedia")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        new ToastButton("Open", action + "&action=openMedia")
                        {
                            ActivationType = ToastActivationType.Foreground
                        }
                    }
                },
                Launch = action
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

        public static void SendNewLoginRequestNotify(string message, string image, string action, string heroImage = null)
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
                                Text = message
                            }
                        },
                        AppLogoOverride = new ToastGenericAppLogo()
                        {
                            Source = image ?? "",
                            HintCrop = ToastGenericAppLogoCrop.Circle
                        },
                        HeroImage = heroImage != null ? new ToastGenericHeroImage { Source = heroImage } : null
                    }
                },
                Actions = new ToastActionsCustom()
                {
                    Buttons =
                    {
                        new ToastButton("Approve", action + "&action=approveLoginRequest")
                        {
                            ActivationType = ToastActivationType.Background,
                        },
                        new ToastButton("Deny", action + "&action=denyLoginRequest")
                        {
                            ActivationType = ToastActivationType.Background
                        },
                        //new ToastButton("Open", action + "&action=openLoginRequest")
                        //{
                        //    ActivationType = ToastActivationType.Foreground
                        //}
                    }
                },
                Launch = action
            };

            var toastNotif = new ToastNotification(toastContent.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(toastNotif);
        }

    }
}
