using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Microsoft.Toolkit.Uwp.UI.Extensions;
using Windows.Foundation;

namespace WinstaCore.Helpers.ExtensionMethods
{
    /// <summary>
    /// Smooth scroll the list to bring specified item into view
    /// </summary>
    public static partial class ListViewExtensions
    {
        /// <summary>
        /// Smooth scrolling the list to bring the specified index into view
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="index">The index to bring into view. Index can be negative.</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="scrollIfVisible">Set false to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Returns <see cref="Task"/> that completes after scrolling</returns>
        public static async Task SmoothScrollIntoViewWithIndexAsync(this ListViewBase listViewBase, int index, bool disableAnimation = false, bool scrollIfVisible = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            if (index > (listViewBase.Items.Count - 1))
            {
                index = listViewBase.Items.Count - 1;
            }

            if (index < -listViewBase.Items.Count)
            {
                index = -listViewBase.Items.Count;
            }

            index = (index < 0) ? (index + listViewBase.Items.Count) : index;

            bool isVirtualizing = default;
            double previousXOffset = default, previousYOffset = default;

            var scrollViewer = listViewBase.FindDescendant<ScrollViewer>();
            var selectorItem = listViewBase.ContainerFromIndex(index) as SelectorItem;

            // If selectorItem is null then the panel is virtualized.
            // So in order to get the container of the item we need to scroll to that item first and then use ContainerFromIndex
            if (selectorItem == null)
            {
                isVirtualizing = true;

                previousXOffset = scrollViewer.HorizontalOffset;
                previousYOffset = scrollViewer.VerticalOffset;

                var tcs = new TaskCompletionSource<object>();

                void ViewChanged(object _, ScrollViewerViewChangedEventArgs __) => tcs.TrySetResult(result: default);

                try
                {
                    scrollViewer.ViewChanged += ViewChanged;
                    listViewBase.ScrollIntoView(listViewBase.Items[index], ScrollIntoViewAlignment.Leading);
                    await tcs.Task;
                }
                finally
                {
                    scrollViewer.ViewChanged -= ViewChanged;
                }

                selectorItem = (SelectorItem)listViewBase.ContainerFromIndex(index);
            }

            var transform = selectorItem.TransformToVisual((UIElement)scrollViewer.Content);
            var position = transform.TransformPoint(new Point(0, 0));

            // Scrolling back to previous position
            if (isVirtualizing)
            {
                await scrollViewer.ChangeViewAsync(previousXOffset, previousYOffset, zoomFactor: null, disableAnimation: true);
            }

            var listViewBaseWidth = listViewBase.ActualWidth;
            var selectorItemWidth = selectorItem.ActualWidth;
            var listViewBaseHeight = listViewBase.ActualHeight;
            var selectorItemHeight = selectorItem.ActualHeight;

            previousXOffset = scrollViewer.HorizontalOffset;
            previousYOffset = scrollViewer.VerticalOffset;

            var minXPosition = position.X - listViewBaseWidth + selectorItemWidth;
            var minYPosition = position.Y - listViewBaseHeight + selectorItemHeight;

            var maxXPosition = position.X;
            var maxYPosition = position.Y;

            double finalXPosition, finalYPosition;

            // If the Item is in view and scrollIfVisible is false then we don't need to scroll
            if (!scrollIfVisible && (previousXOffset <= maxXPosition && previousXOffset >= minXPosition) && (previousYOffset <= maxYPosition && previousYOffset >= minYPosition))
            {
                finalXPosition = previousXOffset;
                finalYPosition = previousYOffset;
            }
            else
            {
                finalXPosition = previousXOffset + additionalHorizontalOffset;
                finalYPosition = maxYPosition + additionalVerticalOffset;
            }

            await scrollViewer.ChangeViewAsync(finalXPosition, finalYPosition, zoomFactor: null, disableAnimation);
        }

        /// <summary>
        /// Smooth scrolling the list to bring the specified data item into view
        /// </summary>
        /// <param name="listViewBase">List to scroll</param>
        /// <param name="item">The data item to bring into view</param>
        /// <param name="disableAnimation">Set true to disable animation</param>
        /// <param name="scrollIfVisibile">Set true to disable scrolling when the corresponding item is in view</param>
        /// <param name="additionalHorizontalOffset">Adds additional horizontal offset</param>
        /// <param name="additionalVerticalOffset">Adds additional vertical offset</param>
        /// <returns>Returns <see cref="Task"/> that completes after scrolling</returns>
        public static async Task SmoothScrollIntoViewWithItemAsync(this ListViewBase listViewBase, object item, bool disableAnimation = false, bool scrollIfVisibile = true, int additionalHorizontalOffset = 0, int additionalVerticalOffset = 0)
        {
            await SmoothScrollIntoViewWithIndexAsync(listViewBase, listViewBase.Items.IndexOf(item), disableAnimation, scrollIfVisibile, additionalHorizontalOffset, additionalVerticalOffset);
        }

        /// <summary>
        /// Changes the view of <see cref="ScrollViewer"/> asynchronous.
        /// </summary>
        /// <param name="scrollViewer">The scroll viewer.</param>
        /// <param name="horizontalOffset">The horizontal offset.</param>
        /// <param name="verticalOffset">The vertical offset.</param>
        /// <param name="zoomFactor">The zoom factor.</param>
        /// <param name="disableAnimation">if set to <c>true</c> disable animation.</param>
        private static async Task ChangeViewAsync(this ScrollViewer scrollViewer, double? horizontalOffset, double? verticalOffset, float? zoomFactor, bool disableAnimation)
        {
            if (horizontalOffset > scrollViewer.ScrollableWidth)
            {
                horizontalOffset = scrollViewer.ScrollableWidth;
            }
            else if (horizontalOffset < 0)
            {
                horizontalOffset = 0;
            }

            if (verticalOffset > scrollViewer.ScrollableHeight)
            {
                verticalOffset = scrollViewer.ScrollableHeight;
            }
            else if (verticalOffset < 0)
            {
                verticalOffset = 0;
            }

            // MUST check this and return immediately, otherwise this async task will never complete because ViewChanged event won't get triggered
            if (horizontalOffset == scrollViewer.HorizontalOffset && verticalOffset == scrollViewer.VerticalOffset)
            {
                return;
            }

            var tcs = new TaskCompletionSource<object>();

            void ViewChanged(object _, ScrollViewerViewChangedEventArgs e)
            {
                if (e.IsIntermediate)
                {
                    return;
                }

                tcs.TrySetResult(result: default);
            }

            try
            {
                scrollViewer.ViewChanged += ViewChanged;
                scrollViewer.ChangeView(horizontalOffset, verticalOffset, zoomFactor, disableAnimation);
                await tcs.Task;
            }
            finally
            {
                scrollViewer.ViewChanged -= ViewChanged;
            }
        }
    }
}