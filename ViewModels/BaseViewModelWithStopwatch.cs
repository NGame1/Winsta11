using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Navigation;

namespace ViewModels;

public class BaseViewModelWithStopwatch : BaseViewModel
{
    protected Stopwatch StopTimer { get; set; }

    public override void OnNavigatedTo(NavigationEventArgs e)
    {
        StopTimer = Stopwatch.StartNew();
        base.OnNavigatedTo(e);
    }

    public override void OnNavigatingFrom(NavigatingCancelEventArgs e)
    {
        StopTimer.Stop();
        StopTimer = null;
        base.OnNavigatingFrom(e);
    }

    public async Task<bool> EnsureTimeElapsed(int Milliseconds = 500)
    {
        StopTimer.Restart();
        await Task.Delay(Milliseconds);
        if (StopTimer.ElapsedMilliseconds < Milliseconds) return false;
        return true;
    }
}
