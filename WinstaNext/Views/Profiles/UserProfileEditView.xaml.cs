﻿using Resources;
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
using WinstaCore.Interfaces.Views.Profiles;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace WinstaNext.Views.Profiles
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserProfileEditView : BasePage, IUserProfileEditView
    {
        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.EditUser;

        public UserProfileEditView()
        {
            this.InitializeComponent();
        }

    }
}
