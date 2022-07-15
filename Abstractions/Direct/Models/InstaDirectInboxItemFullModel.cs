using InstagramApiSharp.Classes.Models;
using PropertyChanged;
using System.Linq;
#nullable enable

namespace Abstractions.Direct.Models
{
    [AddINotifyPropertyChangedInterface]
    public class InstaDirectInboxItemFullModel : InstaDirectInboxItem
    {
        public InstaUserShort? User { get; }
        public InstaDirectInboxItemFullModel(InstaDirectInboxItem baseObject, InstaUserShort? user)
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

            User = user;

        }
    }
}
