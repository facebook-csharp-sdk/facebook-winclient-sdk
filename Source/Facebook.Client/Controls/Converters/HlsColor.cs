namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using Windows.UI;
#endif
#if WP8
    using System;
    using System.Windows.Media;
#endif

    /// <summary>
    /// Describes a color in terms of hue, lightness, and saturation (HLS).
    /// </summary>
    internal struct HlsColor
    {
        public double A;
        public double H;
        public double L;
        public double S;

        /// <summary>
        /// Converts a color described in terms of alpha, red, green and blue channels (RGB) to 
        /// its equivalent HLS Color.
        /// </summary>
        /// <param name="rgbColor">The RGB color to convert.</param>
        /// <returns>An equivalent HLS Color.</returns>
        public static HlsColor FromRgb(Color rgbColor)
        {
            var hlsColor = new HlsColor();

            var red = rgbColor.R / 255.0;
            var green = rgbColor.G / 255.0;
            var blue = rgbColor.B / 255.0;
            var alpha = rgbColor.A / 255.0;

            var minVal = Math.Min(red, Math.Min(green, blue));
            var maxVal = Math.Max(red, Math.Max(green, blue));
            var delta = maxVal - minVal;

            if (maxVal == minVal)
            {
                hlsColor.H = 0.0;
                hlsColor.S = 0.0;
                hlsColor.L = maxVal;
                return hlsColor;
            }

            hlsColor.L = (minVal + maxVal) / 2.0;
            hlsColor.S = delta / ((hlsColor.L < 0.5) ? (maxVal + minVal) : (2.0 - maxVal - minVal));

            if (red == maxVal) hlsColor.H = (green - blue) / delta;
            if (green == maxVal) hlsColor.H = 2.0 + ((blue - red) / delta);
            if (blue == maxVal) hlsColor.H = 4.0 + ((red - green) / delta);

            hlsColor.H *= 60;
            if (hlsColor.H < 0) hlsColor.H += 360;

            hlsColor.A = alpha;

            return hlsColor;
        }

        /// <summary>
        /// Returns an equivalent color described in terms of alpha, red, green and blue channels (RGB).
        /// </summary>
        /// <returns>An equivalent Color (RGB).</returns>
        public Color ToRgb()
        {
            var rgbColor = new Color();

            if (this.S == 0)
            {
                rgbColor.R = (byte)(this.L * 255);
                rgbColor.G = (byte)(this.L * 255);
                rgbColor.B = (byte)(this.L * 255);
                rgbColor.A = (byte)(this.A * 255);
                return rgbColor;
            }

            double t1;
            if (this.L < 0.5)
            {
                t1 = this.L * (1.0 + this.S);
            }
            else
            {
                t1 = this.L + this.S - (this.L * this.S);
            }

            var t2 = (2.0 * this.L) - t1;

            var h = this.H / 360;

            var tR = h + (1.0 / 3.0);
            var r = TransformColor(t1, t2, tR);

            var tG = h;
            var g = TransformColor(t1, t2, tG);

            var tB = h - (1.0 / 3.0);
            var b = TransformColor(t1, t2, tB);

            rgbColor.R = (byte)(r * 255);
            rgbColor.G = (byte)(g * 255);
            rgbColor.B = (byte)(b * 255);
            rgbColor.A = (byte)(this.A * 255);

            return rgbColor;
        }

        private static double TransformColor(double t1, double t2, double t3)
        {
            if (t3 < 0) t3 += 1.0;
            if (t3 > 1) t3 -= 1.0;

            double color;
            if (6.0 * t3 < 1)
            {
                color = t2 + ((t1 - t2) * 6.0 * t3);
            }
            else if (2.0 * t3 < 1)
            {
                color = t1;
            }
            else if (3.0 * t3 < 2)
            {
                color = t2 + ((t1 - t2) * ((2.0 / 3.0) - t3) * 6.0);
            }
            else
            {
                color = t2;
            }

            return color;
        }
    }
}
