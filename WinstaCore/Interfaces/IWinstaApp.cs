using InstagramApiSharp.API;
using InstagramApiSharp.Classes.Models;

namespace WinstaCore.Interfaces
{
    public interface IWinstaApp
    {
        string SetCurrentUserSession(string session);
        void SetMyUserInstance(InstaUserShort _user);
        IInstaApi CreateInstaAPIInstance(string session);
    }
}
