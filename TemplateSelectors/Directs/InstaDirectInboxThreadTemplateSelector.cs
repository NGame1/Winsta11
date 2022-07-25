using InstagramApiSharp.Classes.Models;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TemplateSelectors
{
    public class InstaDirectInboxThreadTemplateSelector : DataTemplateSelector
    {
        public DataTemplate GroupTemplate { get; set; }
        public DataTemplate PrivateTemplate { get; set; }
        
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if(item is InstaDirectInboxThread thread)
            {
                if (thread.IsGroup) return GroupTemplate;
                else return PrivateTemplate;
            }
            return base.SelectTemplateCore(item, container);
        }
    }
}
