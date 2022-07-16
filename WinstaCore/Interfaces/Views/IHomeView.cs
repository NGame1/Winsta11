using WinstaCore.Attributes;

namespace WinstaCore.Interfaces.Views
{
    public interface IHomeView : IView
    {
        RangePlayerAttribute Medias { get; }
    }
}
