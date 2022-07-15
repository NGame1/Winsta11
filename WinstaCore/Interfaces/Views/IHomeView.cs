using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinstaCore.Attributes;

namespace WinstaCore.Interfaces.Views
{
    public interface IHomeView
    {
        RangePlayerAttribute Medias { get; }
    }
}
