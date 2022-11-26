

using Windows.Foundation;

namespace WinstaCore.Helpers
{
    public static class ControlSizeHelper
    {
        public static Size CalculateSizeInBox(double imageWidth, double imageHeight, double windowHeight, double windowWidth)
        {
            double dbl = imageWidth / imageHeight;
            if (windowHeight <= 0 && windowWidth <= 0)
                return new Size(imageHeight / dbl, (imageWidth / dbl));
            //calculate the ratio

            //set height of image to boxHeight and check if resulting width is less than boxWidth, 
            //else set width of image to boxWidth and calculate new height
            if (windowHeight * dbl <= windowWidth)
                return new Size((windowHeight * dbl), windowHeight);
            else
                return new Size(windowWidth, (windowWidth / dbl));
        }
    }
}
