using System;
using Windows.Foundation;

namespace WinstaCore.Helpers
{
    public class AspectRatioHelper
    {
        public static int GetGreatestCommonDivisor(int Height, int Width)
        {
            return Width == 0 ? Height : GetGreatestCommonDivisor(Width, Height % Width);
        }

        public static Size FindNearestAspectRatio(uint ImageHeight, uint ImageWidth)
        {
            var res = 1000f;
            var resaspect = -1;
            float target_ratio = (float)ImageWidth / (float)ImageHeight;
            var _9_16Ratio = 9f / 16f;//0.5625
            var _5_7Ratio = 5f / 7f;//0.7141
            var _4_5Ratio = 4f / 5f;//0.8
            var _3_4Ratio = 3f / 4f;//75
            var _3_5Ratio = 3f / 5f;//0.6
            var _2_3Ratio = 2f / 3f;//0.66
            var _1_1Ratio = 1f / 1f;//1
            var tres = Math.Abs((target_ratio - _1_1Ratio));
            if (tres < res) { resaspect = 0; res = tres; }
            tres = Math.Abs((target_ratio - _2_3Ratio));
            if (tres < res) { resaspect = 1; res = tres; }
            tres = Math.Abs((target_ratio - _3_5Ratio));
            if (tres < res) { resaspect = 2; res = tres; }
            tres = Math.Abs((target_ratio - _3_4Ratio));
            if (tres < res) { resaspect = 3; res = tres; }
            tres = Math.Abs((target_ratio - _4_5Ratio));
            if (tres < res) { resaspect = 4; res = tres; }
            tres = Math.Abs((target_ratio - _5_7Ratio));
            if (tres < res) { resaspect = 5; res = tres; }
            tres = Math.Abs((target_ratio - _9_16Ratio));
            if (tres < res) { resaspect = 6; res = tres; }
            var orgwidthaspect = ImageWidth / GCD(ImageWidth, ImageHeight);
            var orgheightaspect = ImageHeight / GCD(ImageWidth, ImageHeight);
            var AspectWidth = Convert.ToInt32((ImageWidth / orgwidthaspect));
            var AspectHeight = Convert.ToInt32((ImageHeight / orgheightaspect));
            var Max = Convert.ToInt32(Math.Max(ImageHeight, ImageWidth));
            Size FinalSize;
            switch (resaspect)
            {
                case 0:
                    FinalSize = new Size(Max, Max);
                    break;
                case 1:
                    FinalSize = new Size((AspectWidth * 2), (AspectHeight * 3));
                    break;
                case 2:
                    FinalSize = new Size((AspectWidth * 3), (AspectHeight * 5));
                    break;
                case 3:
                    FinalSize = new Size((AspectWidth * 3), (AspectHeight * 4));
                    break;
                case 4:
                    FinalSize = new Size((AspectWidth * 4), (AspectHeight * 5));
                    break;
                case 5:
                    FinalSize = new Size((AspectWidth * 5), (AspectHeight * 7));
                    break;
                case 6:
                    FinalSize = new Size((AspectWidth * 9), (AspectHeight * 16));
                    break;
                default:
                    FinalSize = new Size(Max, Max);
                    break;
            }
            if (FinalSize.Width <= 480 || FinalSize.Height <= 480)
            {
                if (ImageWidth <= 480 || ImageHeight <= 480)
                {
                    var temp = CalculateSizeInBox(FinalSize.Width, FinalSize.Height, 680, 680);
                    if (temp.Width >= 480 && temp.Height >= 480)
                        return temp;
                    else
                    {
                        if (temp.Width < 480) temp.Width = 680;
                        if (temp.Height < 480) temp.Height = 680;
                        return temp;
                    }
                }
                else
                {
                    if (Max <= 1280)
                        return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, Max, Max);
                    else
                        return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, 1280, 1280);
                }
            }
            if (FinalSize.Width <= 1280 && FinalSize.Height <= 1280)
                return FinalSize;
            else return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, 1280, 1280);
            //return new Size((AspectWidth * Max), (AspectHeight * Max));
        }

        public static Size FindNearestAspectRatioIGTVStory(uint ImageHeight, uint ImageWidth)
        {
            float target_ratio = (float)ImageWidth / (float)ImageHeight;
            int resaspect = 6;
            var orgwidthaspect = ImageWidth / GCD(ImageWidth, ImageHeight);
            var orgheightaspect = ImageHeight / GCD(ImageWidth, ImageHeight);
            var AspectWidth = Convert.ToInt32((ImageWidth / orgwidthaspect));
            var AspectHeight = Convert.ToInt32((ImageHeight / orgheightaspect));
            var Max = Convert.ToInt32(Math.Max(ImageHeight, ImageWidth));
            Size FinalSize;
            switch (resaspect)
            {
                case 6:
                    FinalSize = new Size((AspectWidth * 9), (AspectHeight * 16));
                    break;
                default:
                    FinalSize = new Size(Max, Max);
                    break;
            }
            if (FinalSize.Width <= 480 || FinalSize.Height <= 480)
            {
                if (ImageWidth <= 480 || ImageHeight <= 480)
                {
                    var a = 480 / FinalSize.Width;
                    var b = 480 / FinalSize.Height;
                    var max = Math.Max(a, b);
                    FinalSize.Width = FinalSize.Width * max;
                    FinalSize.Height = FinalSize.Height * max;
                    return FinalSize;
                }
                else
                {
                    if (Max <= 1280)
                        return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, Max, Max);
                    else
                        return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, 1280, 1280);
                }
            }
            if (FinalSize.Width <= 1280 && FinalSize.Height <= 1280)
                return FinalSize;
            else return CalculateSizeInBox(FinalSize.Width, FinalSize.Height, 1280, 1280);
            //return new Size((AspectWidth * Max), (AspectHeight * Max));
        }

        static uint GCD(uint a, uint b)
        {
            uint Remainder;

            while (b != 0)
            {
                Remainder = a % b;
                a = b;
                b = Remainder;
            }

            return a;
        }
        static Size CalculateSizeInBox(double imageWidth, double imageHeight, double windowHeight, double windowWidth)
        {
            //calculate the ratio
            double dbl = (double)imageWidth / (double)imageHeight;
            //dbl = 1.102040816326531‬

            //set height of image to boxHeight and check if resulting width is less than boxWidth, 
            //else set width of image to boxWidth and calculate new height
            if ((int)((double)windowHeight * dbl) <= windowWidth)
            {
                return new Size(Convert.ToInt32(((double)windowHeight * dbl)), windowHeight);
            }
            else
            {
                return new Size(windowWidth, Convert.ToInt32(((double)windowWidth / dbl)));
            }
        }

    }
}
