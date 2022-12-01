using PropertyChanged;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using WinstaCore.Interfaces.Views.Medias.Upload;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace WinstaNext.Views.Media.Upload
{
    [AddINotifyPropertyChangedInterface]
    public sealed partial class VideoMediaRangeSlider : UserControl, IVideoMediaRangeSlider
    {
        public double Minimum
        {
            get { return (double)GetValue(MinimumProperty); }
            set { SetValue(MinimumProperty, value); }
        }

        public double Maximum
        {
            get { return (double)GetValue(MaximumProperty); }
            set { SetValue(MaximumProperty, value); }
        }

        public double RangeMin
        {
            get { return (double)GetValue(RangeMinProperty); }
            set { SetValue(RangeMinProperty, value); }
        }

        public double RangeMax
        {
            get { return (double)GetValue(RangeMaxProperty); }
            set { SetValue(RangeMaxProperty, value); }
        }

        public double RangeNow
        {
            get { return (double)GetValue(RangeNowProperty); }
            set { SetValue(RangeNowProperty, value); }
        }

        public MediaElement MediaElement
        {
            get { return (MediaElement)GetValue(MediaElementProperty); }
            set { SetValue(MediaElementProperty, value); }
        }

        public TimeSpan Position
        {
            get => MediaElement.Position;
            set { MediaElement.Position = value; }
        }

        public static readonly DependencyProperty MinimumProperty = DependencyProperty.Register("Minimum", typeof(double), typeof(VideoMediaRangeSlider), new PropertyMetadata(0.0));

        public static readonly DependencyProperty MaximumProperty = DependencyProperty.Register("Maximum", typeof(double), typeof(VideoMediaRangeSlider), new PropertyMetadata(1.0));

        public static readonly DependencyProperty RangeMinProperty = DependencyProperty.Register("RangeMin", typeof(double), typeof(VideoMediaRangeSlider), new PropertyMetadata(0.0, OnRangeMinPropertyChanged));

        public static readonly DependencyProperty RangeMaxProperty = DependencyProperty.Register("RangeMax", typeof(double), typeof(VideoMediaRangeSlider), new PropertyMetadata(1.0, OnRangeMaxPropertyChanged));

        public static readonly DependencyProperty RangeNowProperty = DependencyProperty.Register("RangeNow", typeof(double), typeof(VideoMediaRangeSlider), new PropertyMetadata(0.0, OnRangeNowPropertyChanged));

        public static readonly DependencyProperty MediaElementProperty = DependencyProperty.Register("MediaElement", typeof(MediaElement), typeof(VideoMediaRangeSlider), new PropertyMetadata(null));

        public event EventHandler MinimumValueChanged;
        public event EventHandler MaximumValueChanged;

        ThreadPoolTimer Timer { get; }
        public VideoMediaRangeSlider()
        {
            this.InitializeComponent();
            Timer = ThreadPoolTimer.CreatePeriodicTimer(Timer_Tick, TimeSpan.FromMilliseconds(100));
        }

        private async void Timer_Tick(ThreadPoolTimer timer)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                if (MediaElement == null) return;
                RangeNow = MediaElement.Position.TotalMilliseconds;
                if (RangeNow > RangeMax)
                {
                    RangeNow = RangeMin;
                    MediaElement.Position = TimeSpan.FromMilliseconds(RangeNow);
                }
            });
        }

        public void AddImageToSlider(ImageSource src)
        {
            _imageStack.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            var img = new Image()
            {
                Width = 85,
                Source = src,
                Stretch = Stretch.UniformToFill,
                VerticalAlignment = VerticalAlignment.Center
            };
            Grid.SetColumn(img, _imageStack.ColumnDefinitions.Count - 1);
            _imageStack.Children.Add(img);
        }

        static void OnRangeMinPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not VideoMediaRangeSlider slider) return;
            var newValue = (double)e.NewValue;

            if (newValue < slider.Minimum)
            {
                slider.RangeMin = slider.Minimum;
            }
            else if (newValue > slider.Maximum)
            {
                slider.RangeMin = slider.Maximum;
            }
            else
            {
                slider.RangeMin = newValue;
            }

            if (slider.RangeMin > slider.RangeMax)
            {
                slider.RangeMax = slider.RangeMin;
            }
            if (slider.RangeMin > slider.RangeNow)
            {
                slider.RangeNow = slider.RangeMin;
                slider.MediaElement.Position = TimeSpan.FromMilliseconds(slider.RangeNow);
            }

            slider.UpdateMinThumb(slider.RangeMin);
        }

        static void OnRangeMaxPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not VideoMediaRangeSlider slider) return;
            var newValue = (double)e.NewValue;

            if (newValue < slider.Minimum)
            {
                slider.RangeMax = slider.Minimum;
            }
            else if (newValue > slider.Maximum)
            {
                slider.RangeMax = slider.Maximum;
            }
            else
            {
                slider.RangeMax = newValue;
            }

            if (slider.RangeMax < slider.RangeMin)
            {
                slider.RangeMin = slider.RangeMax;
            }
            if (slider.RangeMax < slider.RangeNow)
            {
                slider.RangeNow = slider.RangeMin;
                slider.MediaElement.Position = TimeSpan.FromMilliseconds(slider.RangeNow);
            }

            slider.UpdateMaxThumb(slider.RangeMax);
        }

        static void OnRangeNowPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not VideoMediaRangeSlider slider) return;
            var newValue = (double)e.NewValue;
            if (newValue < slider.Minimum)
            {
                slider.RangeNow = slider.Minimum;
            }
            else if (newValue > slider.Maximum)
            {
                slider.RangeNow = slider.Maximum;
            }
            else
            {
                slider.RangeNow = newValue;
            }

            slider.UpdateNowThumb(slider.RangeNow);
        }

        public void UpdateMinThumb(double min, bool update = false)
        {
            if (ContainerCanvas != null)
            {
                if (update || !MinThumb.IsDragging)
                {
                    var relativeLeft = ((min - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;

                    Canvas.SetLeft(MinThumb, relativeLeft);
                    Canvas.SetLeft(ActiveRectangle, relativeLeft);

                    ActiveRectangle.Width = (RangeMax - min) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
                }
            }
        }

        public void UpdateMaxThumb(double max, bool update = false)
        {
            if (ContainerCanvas != null)
            {
                if (update || !MaxThumb.IsDragging)
                {
                    var relativeRight = (max - Minimum) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;

                    Canvas.SetLeft(MaxThumb, relativeRight);

                    ActiveRectangle.Width = (max - RangeMin) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
                }
            }
        }

        public void UpdateNowThumb(double now, bool update = false)
        {
            if (ContainerCanvas != null)
            {
                if (update || !NowThumb.IsDragging)
                {
                    var relativeLeft = ((now - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;

                    Canvas.SetLeft(NowThumb, relativeLeft);
                    //Canvas.SetLeft(ActiveRectangle, RangeMin);

                    //ActiveRectangle.Width = (RangeMax - RangeMin) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
                }
            }
        }

        void ContainerCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var relativeLeft = ((RangeMin - Minimum) / (Maximum - Minimum)) * ContainerCanvas.ActualWidth;
            var relativeRight = (RangeMax - Minimum) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;

            Canvas.SetLeft(MinThumb, relativeLeft);
            Canvas.SetLeft(ActiveRectangle, relativeLeft);
            Canvas.SetLeft(MaxThumb, relativeRight);

            ActiveRectangle.Width = (RangeMax - RangeMin) / (Maximum - Minimum) * ContainerCanvas.ActualWidth;
        }

        void MinThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var min = DragThumb(MinThumb, 0, Canvas.GetLeft(MaxThumb), e.HorizontalChange);
            UpdateMinThumb(min, true);
            RangeMin = Math.Round(min);
        }

        void MaxThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var max = DragThumb(MaxThumb, Canvas.GetLeft(MinThumb), ContainerCanvas.ActualWidth, e.HorizontalChange);
            UpdateMaxThumb(max, true);
            RangeMax = Math.Round(max);
        }

        private void NowThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            var now = DragThumb(NowThumb, RangeMin, RangeMax, e.HorizontalChange);
            UpdateNowThumb(now, true);
            RangeNow = Math.Round(now);
            MediaElement.Position = TimeSpan.FromMilliseconds(RangeNow);
        }

        double DragThumb(Thumb thumb, double min, double max, double offset)
        {
            var currentPos = Canvas.GetLeft(thumb);
            var nextPos = currentPos + offset;

            nextPos = Math.Max(min, nextPos);
            nextPos = Math.Min(max, nextPos);

            return (Minimum + (nextPos / ContainerCanvas.ActualWidth) * (Maximum - Minimum));
        }

        void MinThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UpdateMinThumb(RangeMin);
            Canvas.SetZIndex(MinThumb, 10);
            Canvas.SetZIndex(MaxThumb, 0);
            this.MinimumValueChanged?.Invoke(this, EventArgs.Empty);
        }

        void MaxThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UpdateMaxThumb(RangeMax);
            Canvas.SetZIndex(MinThumb, 0);
            Canvas.SetZIndex(MaxThumb, 10);
            this.MaximumValueChanged?.Invoke(this, EventArgs.Empty);
        }

        private void NowThumb_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            UpdateNowThumb(RangeNow);
            Canvas.SetZIndex(MinThumb, 0);
            Canvas.SetZIndex(MaxThumb, 10);
            MediaElement.Position = TimeSpan.FromMilliseconds(RangeNow);
            MediaElement.Play();
        }

        private void NowThumb_DragStarted(object sender, DragStartedEventArgs e)
        {
            MediaElement.Pause();
        }

        
    }
}

