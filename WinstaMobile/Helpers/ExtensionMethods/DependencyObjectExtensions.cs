using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#nullable enable

namespace WinstaMobile.Helpers.ExtensionMethods
{
    public static class DependencyObjectExtensions
    {
        public static T? FindDescendant<T>(this DependencyObject element) where T : DependencyObject
        {
            T? val = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(element);
            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(element, i);
                T? val2 = child as T;
                if (val2 != null)
                {
                    val = val2;
                    break;
                }

                val = child.FindDescendant<T>();
                if (val != null)
                {
                    break;
                }
            }

            return val;
        }
    }
}
