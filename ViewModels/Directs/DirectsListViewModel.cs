﻿using Core.Collections.IncrementalSources.Directs;
using InstagramApiSharp.Classes.Models;
using Microsoft.Toolkit.Uwp;
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace ViewModels.Directs
{
    public class DirectsListViewModel : BaseViewModel
    {
        IncrementalDirectInbox Instance { get; }
        public IncrementalLoadingCollection<IncrementalDirectInbox, InstaDirectInboxThread> Inbox { get; }

        public DirectsListViewModel()
        {
            Instance = new IncrementalDirectInbox();
            Inbox = new IncrementalLoadingCollection<IncrementalDirectInbox, InstaDirectInboxThread>
                (Instance, onStartLoading: LoadingStarted, onEndLoading: LoadEnded, onError: OnError);
        }

        private void OnError(Exception obj)
        {
            throw obj;
        }

        private void LoadEnded()
        {

        }

        private void LoadingStarted()
        {

        }

        public override async Task OnNavigatedToAsync(NavigationEventArgs e)
        {
            if (Inbox.Count == 0)
                await Inbox.LoadMoreItemsAsync(1);
            await base.OnNavigatedToAsync(e);
        }
    }
}
