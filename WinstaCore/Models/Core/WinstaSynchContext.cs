﻿using System.Globalization;
using System.Threading;

namespace WinstaCore.Models.Core
{
    public class WinstaSynchronizationContext : SynchronizationContext
    {
        SynchronizationContext context;
        public WinstaSynchronizationContext(SynchronizationContext ctx)
        {
            context = ctx;
        }

        public override void Post(SendOrPostCallback d, object state)
        {
            var lang = ApplicationSettingsManager.Instance.GetLanguage();
#if !WINDOWS_UWP15063
            var cul = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = cul;
            Thread.CurrentThread.CurrentUICulture = cul;
#endif
            context.Post(d, state);
        }
    }
}
