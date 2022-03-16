using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinstaNext.Core.Collections.IncrementalSources.Activities;

namespace WinstaNext.ViewModels.Activities
{
    internal class ActivitiesViewModel : BaseViewModel
    {
        public IncrementalLoadingCollection<IncrementalUserActivities, InstaRecentActivityFeed> Activities { get; }

        public override string PageHeader { get; protected set; } = LanguageManager.Instance.Instagram.Activities;

        public ActivitiesViewModel()
        {
            Activities = new();
        }
    }
}
