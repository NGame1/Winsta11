using InstagramApiSharp.Classes.Models;
using System.Linq;

namespace Abstractions.Stories
{
    public class WinstaHighlightFeed : InstaHighlightFeed
    {
        public bool IsLoading = false;
        public WinstaHighlightFeed(InstaHighlightFeed baseObject)
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
