using InstagramApiSharp.Classes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.Models.TemplateSelectors.Directs
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
