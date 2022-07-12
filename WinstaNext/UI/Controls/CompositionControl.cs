using PropertyChanged;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.UI.Controls
{
    /// <summary>
    /// A version of the ContentControl that works with the ContentTemplateSelector.
    /// </summary>
    [AddINotifyPropertyChangedInterface]
    public class CompositionControl : ContentControl
    {
        public static readonly DependencyProperty BindingContextProperty = DependencyProperty.Register(
             nameof(BindingContext),
             typeof(object),
             typeof(CompositionControl),
             new PropertyMetadata(null));

        [OnChangedMethod(nameof(OnBindingContextChanged))]
        public object BindingContext
        {
            get { return GetValue(BindingContextProperty); }
            set { SetValue(BindingContextProperty, value); }
        }

        void OnBindingContextChanged()
        {
            if (this.ContentTemplateSelector is DataTemplateSelector dataTemplateSelector)
            {
                this.ContentTemplate = dataTemplateSelector.SelectTemplate(BindingContext, null);
            }

            base.OnApplyTemplate();
        }

        /// <summary>
        /// Invoked when the value of the Content property changes. 
        /// </summary>
        /// <param name="oldContent">The old value of the Content property.</param>
        /// <param name="newContent">The new value of the Content property.</param>
        //protected override void OnContentChanged(object oldContent, object newContent)
        //{
        //    // There is a bug in the standard content control that trashes the value passed into the SelectTemplateCore method.  This is a
        //    // work-around that allows the same basic structure and can hopefully be replaced when the bug is fixed.  Basically take the new content
        //    // and figure out what template should be used with it based on the structure of the template selector.
        //    DataTemplateSelector dataTemplateSelector = this.ContentTemplateSelector as DataTemplateSelector;
        //    if (dataTemplateSelector != null)
        //    {
        //        this.ContentTemplate = dataTemplateSelector.SelectTemplate(newContent, null);
        //    }
        //
        //    // Allow the base class to handle the rest of the call.
        //    base.OnContentChanged(oldContent, newContent);
        //}
    }
}
