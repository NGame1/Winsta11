using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System.Linq;
using System.Reflection;
#nullable enable

namespace Abstractions.Stories
{
    [AddINotifyPropertyChangedInterface]
    public class WinstaReelFeed : InstaReelFeed
    {
        public bool IsLoading = false;

        public WinstaReelFeed(InstaReelFeed baseObject)
        {
            var properties = baseObject.GetType().GetProperties();

            properties.ToList().ForEach(property =>
            {
                //Check whether that property is present in derived class
                var isPresent = this.GetType().GetProperty(property.Name);
                if (isPresent != null && property.CanWrite)
                {
                    //If present get the value and map it
                    var value = baseObject.GetType().GetProperty(property.Name).GetValue(baseObject, null);
                    this.GetType().GetProperty(property.Name).SetValue(this, value, null);
                }
            });
        }
    }
}
