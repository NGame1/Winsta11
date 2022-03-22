using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WinstaNext.Models.Core
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
            var cul = new CultureInfo(lang);
            Thread.CurrentThread.CurrentCulture = cul;
            Thread.CurrentThread.CurrentUICulture = cul;
            context.Post(d, state);
        }
    }
}
