using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace WinstaNext.Core.Behaviors
{
    internal class SynchronizeVerticalOffsetBehavior : Behavior<ScrollViewer>
    {
        public static ScrollViewer GetSource(DependencyObject obj)
        {
            return (ScrollViewer)obj.GetValue(SourceProperty);
        }

        public static void SetSource(DependencyObject obj, ScrollViewer value)
        {
            obj.SetValue(SourceProperty, value);
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
                "Source",
                typeof(object),
                typeof(SynchronizeVerticalOffsetBehavior),
                new PropertyMetadata(null, SourceChangedCallBack));

        private static void SourceChangedCallBack(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SynchronizeVerticalOffsetBehavior SynchronizeVerticalOffsetBehavior = d as SynchronizeVerticalOffsetBehavior;
            var source = GetSource(d);
            if (SynchronizeVerticalOffsetBehavior != null && source != null)
            {
                try
                {
                    source.ViewChanged -= SynchronizeVerticalOffsetBehavior.SourceScrollViewer_ViewChanged;
                }
                catch { }
                source.ViewChanged += SynchronizeVerticalOffsetBehavior.SourceScrollViewer_ViewChanged;
                //var oldSourceScrollViewer = e.OldValue as ScrollViewer;
                //var newSourceScrollViewer = e.NewValue as ScrollViewer;
                //if (oldSourceScrollViewer != null)
                //{
                //    oldSourceScrollViewer.ViewChanged -= synchronizeHorizontalOffsetBehavior.SourceScrollViewer_ViewChanged;
                //}
                //if (newSourceScrollViewer != null)
                //{
                //    newSourceScrollViewer.ViewChanged += synchronizeHorizontalOffsetBehavior.SourceScrollViewer_ViewChanged;
                //    synchronizeHorizontalOffsetBehavior.UpdateTargetViewAccordingToSource(newSourceScrollViewer);
                //}
            }
        }

        double verticaloffset = 0;
        private void SourceScrollViewer_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            ScrollViewer sourceScrollViewer = sender as ScrollViewer;
            this.UpdateTargetViewAccordingToSource(sourceScrollViewer);
        }

        private void UpdateTargetViewAccordingToSource(ScrollViewer sourceScrollViewer)
        {
            if (sourceScrollViewer != null)
            {
                if (this.AssociatedObject != null)
                {
                    var offsetdelta = sourceScrollViewer.VerticalOffset - verticaloffset;
                    verticaloffset = sourceScrollViewer.VerticalOffset;

                    if (this.AssociatedObject.VerticalOffset <= this.AssociatedObject.ScrollableHeight
                        && sourceScrollViewer.VerticalOffset <= this.AssociatedObject.ScrollableHeight)
                        this.AssociatedObject.ChangeView(null, AssociatedObject.VerticalOffset + offsetdelta, null, true);

                    else if(offsetdelta < 0 && sourceScrollViewer.VerticalOffset == 0 && AssociatedObject.VerticalOffset >0)
                        this.AssociatedObject.ChangeView(null, 0, null);

                    //if (sourceScrollViewer.VerticalOffset <= this.AssociatedObject.ScrollableHeight
                    //    && offsetdelta < 0)
                    //    this.AssociatedObject.ChangeView(null, AssociatedObject.VerticalOffset + offsetdelta, null);

                }
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            var source = GetSource(this.AssociatedObject);
            this.UpdateTargetViewAccordingToSource(source);
        }
    }
}
