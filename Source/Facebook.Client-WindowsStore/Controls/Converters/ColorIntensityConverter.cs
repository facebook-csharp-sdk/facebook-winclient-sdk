using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Facebook.Client.Controls
{
    public class ColorIntensityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null) return null;
            if (parameter == null) return null;

            var brush = (SolidColorBrush)value;
            var rgbColorIn = brush.Color;
            var hlsColor = RgbToHls(rgbColorIn);

            var brightnessAdjustment = Double.Parse((parameter.ToString()));
            hlsColor.L *= brightnessAdjustment;

            var rgbColorOut = HlsToRgb(hlsColor);
            var brushOut = new SolidColorBrush();
            brushOut.Color = rgbColorOut;
            return brushOut;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }

           //#define  HLSMAX   RANGE /* H,L, and S vary over 0-HLSMAX */ 
           //#define  RGBMAX   255   
           //                        /* HLSMAX BEST IF DIVISIBLE BY 6 */ 
           //                        /* RGBMAX, HLSMAX must each fit in a byte. */ 

           ///* Hue is undefined if Saturation is 0 (grey-scale) */ 
           ///* This value determines where the Hue scrollbar is */ 
           ///* initially set for achromatic colors */ 
           //#define UNDEFINED (HLSMAX*2/3)

        void RGBtoHLS(Color lRGBColor)
        {
            const byte RgbMax = 255;    /* R, G, and B vary over 0-RGBMAX */ 

            /* calculate lightness */
            byte maximum = Math.Max(Math.Max(lRGBColor.R, lRGBColor.G), lRGBColor.B);
            byte minimum = Math.Min(Math.Min(lRGBColor.R, lRGBColor.G), lRGBColor.B);
            L = (((maximum + minimum) * HLSMAX) + RgbMax) / (2 * RgbMax);

            if (maximum == minimum)
            {           /* r=g=b --> achromatic case */
                S = 0;                     /* saturation */
                H = UNDEFINED;             /* hue */
            }
            else
            {                        /* chromatic case */
                /* saturation */
                if (L <= (HLSMAX / 2))
                    S = (((maximum - minimum) * HLSMAX) + ((maximum + minimum) / 2)) / (maximum + minimum);
                else
                    S = (((maximum - minimum) * HLSMAX) + ((2 * RgbMax - maximum - minimum) / 2))
                       / (2 * RgbMax - maximum - minimum);

                /* hue */
                var Rdelta = (((maximum - R) * (HLSMAX / 6)) + ((maximum - minimum) / 2)) / (maximum - minimum);
                var Gdelta = (((maximum - G) * (HLSMAX / 6)) + ((maximum - minimum) / 2)) / (maximum - minimum);
                var Bdelta = (((maximum - B) * (HLSMAX / 6)) + ((maximum - minimum) / 2)) / (maximum - minimum);

                if (R == maximum)
                    H = Bdelta - Gdelta;
                else if (G == maximum)
                    H = (HLSMAX / 3) + Rdelta - Bdelta;
                else /* B == cMax */
                    H = ((2 * HLSMAX) / 3) + Gdelta - Rdelta;

                if (H < 0)
                    H += HLSMAX;
                if (H > HLSMAX)
                    H -= HLSMAX;
            }
        }

        static HlsColor RgbToHls(Color rgbColor)
        {
            var hlsColor = new HlsColor();

            double r = (double)rgbColor.R / 255;
            var g = (double)rgbColor.G / 255;
            var b = (double)rgbColor.B / 255;
            var a = (double)rgbColor.A / 255;

            var min = Math.Min(r, Math.Min(g, b));
            var max = Math.Max(r, Math.Max(g, b));
            var delta = max - min;

            if (max == min)
            {
                hlsColor.H = 0;
                hlsColor.S = 0;
                hlsColor.L = max;
                return hlsColor;
            }

            hlsColor.L = (min + max) / 2;

            if (hlsColor.L < 0.5)
            {
                hlsColor.S = delta / (max + min);
            }
            else
            {
                hlsColor.S = delta / (2.0 - max - min);
            }

            if (r == max) hlsColor.H = (g - b) / delta;
            if (g == max) hlsColor.H = 2.0 + (b - r) / delta;
            if (b == max) hlsColor.H = 4.0 + (r - g) / delta;
            hlsColor.H *= 60;
            if (hlsColor.H < 0) hlsColor.H += 360;

            hlsColor.A = a;

            return hlsColor;
        }

        static Color HlsToRgb(HlsColor hlsColor)
        {
            var rgbColor = new Color();

            if (hlsColor.S == 0)
            {
                rgbColor.R = (byte)(hlsColor.L * 255);
                rgbColor.G = (byte)(hlsColor.L * 255);
                rgbColor.B = (byte)(hlsColor.L * 255);
                rgbColor.A = (byte)(hlsColor.A * 255);
                return rgbColor;
            }

            double t1;
            if (hlsColor.L < 0.5)
            {
                t1 = hlsColor.L * (1.0 + hlsColor.S);
            }
            else
            {
                t1 = hlsColor.L + hlsColor.S - (hlsColor.L * hlsColor.S);
            }

            var t2 = 2.0 * hlsColor.L - t1;

            var h = hlsColor.H / 360;

            var tR = h + (1.0 / 3.0);
            var r = SetColor(t1, t2, tR);

            var tG = h;
            var g = SetColor(t1, t2, tG);

            var tB = h - (1.0 / 3.0);
            var b = SetColor(t1, t2, tB);

            rgbColor.R = (byte)(r * 255);
            rgbColor.G = (byte)(g * 255);
            rgbColor.B = (byte)(b * 255);
            rgbColor.A = (byte)(hlsColor.A * 255);

            return rgbColor;
        }

        private static double SetColor(double t1, double t2, double t3)
        {
            if (t3 < 0) t3 += 1.0;
            if (t3 > 1) t3 -= 1.0;

            double color;
            if (6.0 * t3 < 1)
            {
                color = t2 + (t1 - t2) * 6.0 * t3;
            }
            else if (2.0 * t3 < 1)
            {
                color = t1;
            }
            else if (3.0 * t3 < 2)
            {
                color = t2 + (t1 - t2) * ((2.0 / 3.0) - t3) * 6.0;
            }
            else
            {
                color = t2;
            }

            return color;
        }

        private struct HlsColor
        {
            public double A;
            public double H;
            public double L;
            public double S;
        }
    }
}
