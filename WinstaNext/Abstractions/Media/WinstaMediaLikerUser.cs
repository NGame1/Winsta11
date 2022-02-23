using InstagramApiSharp.Classes.Models;
using System.Linq;

namespace WinstaNext.Abstractions.Media
{
    public class WinstaMediaLikerUser
    {
        public InstaUser User { get; }
        public InstaFriendshipShortStatus FriendshipStatus { get; }

        public WinstaMediaLikerUser(InstaUserShort baseObject, InstaFriendshipShortStatus status)
        {
            User = new(baseObject);
            FriendshipStatus = status;
        }
    }
}
