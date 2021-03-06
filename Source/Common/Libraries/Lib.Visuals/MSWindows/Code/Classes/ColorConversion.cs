﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace Public.Common.Lib.Visuals
{
    public static class ColorConversion
    {
        // Component parameters taken from MATLAB's rgb2gray function.
        public const double GrayRedComponent = 0.2989;
        public const double GrayGreenComponent = 0.5870;
        public const double GrayBlueComponent = 0.1140;


        #region Infrastructure

        /// <summary>
        /// Converts from one value to another
        /// </summary>
        /// <remarks>
        /// Inlining makes a significant (~20%) difference since this function is called often.
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T2 ConvertValue<T1, T2>(T1 input, IColorValueTypeInfo<T1> inputInfo, IColorValueTypeInfo<T2> outputInfo)
        {
            double intermediate = inputInfo.ToDouble(inputInfo.Subtract(input, inputInfo.Min)) / inputInfo.ToDouble(inputInfo.Range);

            T2 output = outputInfo.FromDouble(intermediate * outputInfo.ToDouble(outputInfo.Range));
            return output;
        }

        #endregion

        #region RGB

        /// <summary>
        /// If the color channel value type is byte, then the conversion is as simple as creating a new RGB color from the system color.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static RgbColor<byte> ColorToRgb(Color color)
        {
            RgbColor<byte> output = new RgbColor<byte>(color.R, color.G, color.B);
            return output;
        }

        /// <summary>
        /// Parallel operation for byte color channel type.
        /// </summary>
        public static void ColorsToRgbsParallel(IList<Color> colors, IList<RgbColor<byte>> rgbColors)
        {
            int numColors = colors.Count;
            Parallel.For(0, numColors, (x) => ColorConversion.ColorsToRgbsParallelMethod(x, colors, rgbColors));
        }

        private static void ColorsToRgbsParallelMethod(int iColor, IList<Color> colors, IList<RgbColor<byte>> rgbColors)
        {
            Color color = colors[iColor];

            RgbColor<byte> rgbColor = new RgbColor<byte>(color.R, color.G, color.B);
            rgbColors[iColor] = rgbColor;
        }

        /// <summary>
        /// Instance-by-instance generic color channel data type.
        /// </summary>
        public static RgbColor<T> ColorToRgb<T>(Color color, IColorValueTypeInfo<T> rgbValueInfo)
            where T : struct, IEquatable<T>
        {
            T red = ColorConversion.ConvertValue(color.R, ByteColorValueTypeInfo.Instance, rgbValueInfo);
            T green = ColorConversion.ConvertValue(color.G, ByteColorValueTypeInfo.Instance, rgbValueInfo);
            T blue = ColorConversion.ConvertValue(color.B, ByteColorValueTypeInfo.Instance, rgbValueInfo);

            RgbColor<T> output = new RgbColor<T>(red, green, blue);
            return output;
        }

        /// <summary>
        /// Single threaded operation on lists of data.
        /// </summary>
        public static void ColorsToRgbs<T>(IList<Color> colors, IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo)
            where T : struct, IEquatable<T>
        {
            int numColors = colors.Count;
            for (int iColor = 0; iColor < numColors; iColor++)
            {
                Color color = colors[iColor];

                T red = ColorConversion.ConvertValue(color.R, ByteColorValueTypeInfo.Instance, rgbValueInfo);
                T green = ColorConversion.ConvertValue(color.G, ByteColorValueTypeInfo.Instance, rgbValueInfo);
                T blue = ColorConversion.ConvertValue(color.B, ByteColorValueTypeInfo.Instance, rgbValueInfo);

                RgbColor<T> rgbColor = new RgbColor<T>(red, green, blue);
                rgbColors[iColor] = rgbColor;
            }
        }

        /// <summary>
        /// Parallel operation on lists of data (generally much (~33%) faster than the single threaded implementation).
        /// </summary>
        public static void ColorsToRgbsParallel<T>(IList<Color> colors, IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo)
            where T : struct, IEquatable<T>
        {
            int numColors = colors.Count;
            Parallel.For(0, numColors, (x) => ColorConversion.ColorsToRgbsParallelMethod(x, colors, rgbColors, rgbValueInfo));
        }

        private static void ColorsToRgbsParallelMethod<T>(int iColor, IList<Color> colors, IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo)
            where T : struct, IEquatable<T>
        {
            Color color = colors[iColor];

            T red = ColorConversion.ConvertValue(color.R, ByteColorValueTypeInfo.Instance, rgbValueInfo);
            T green = ColorConversion.ConvertValue(color.G, ByteColorValueTypeInfo.Instance, rgbValueInfo);
            T blue = ColorConversion.ConvertValue(color.B, ByteColorValueTypeInfo.Instance, rgbValueInfo);

            RgbColor<T> rgbColor = new RgbColor<T>(red, green, blue);
            rgbColors[iColor] = rgbColor;
        }

        /// <summary>
        /// If the color channel type is byte, the conversion is as simple as constructing a new color instance.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color RgbToColor(RgbColor<byte> rgbColor)
        {
            Color output = Color.FromArgb(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
            return output;
        }

        /// <summary>
        /// Parallel operation for byte color channel type.
        /// </summary>
        public static void RgbsToColorsParallel(IList<RgbColor<byte>> rgbColors, IList<Color> colors)
        {
            int numColors = colors.Count;
            Parallel.For(0, numColors, (x) => ColorConversion.RgbsToColorsParallelMethod(x, rgbColors, colors));
        }

        private static void RgbsToColorsParallelMethod(int iColor, IList<RgbColor<byte>> rgbColors, IList<Color> colors)
        {
            RgbColor<byte> rgbColor = rgbColors[iColor];

            Color color = Color.FromArgb(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
            colors[iColor] = color;
        }

        /// <summary>
        /// Instance-by-instance generic color channel data type.
        /// </summary>
        public static Color RgbToColor<T>(RgbColor<T> rgbColor, IColorValueTypeInfo<T> rgbValueInfo)
            where T : struct, IEquatable<T>
        {
            byte red = ColorConversion.ConvertValue(rgbColor.Red, rgbValueInfo, ByteColorValueTypeInfo.Instance);
            byte green = ColorConversion.ConvertValue(rgbColor.Green, rgbValueInfo, ByteColorValueTypeInfo.Instance);
            byte blue = ColorConversion.ConvertValue(rgbColor.Blue, rgbValueInfo, ByteColorValueTypeInfo.Instance);

            Color output = Color.FromArgb(red, green, blue);
            return output;
        }

        /// <summary>
        /// Single threaded operation on lists of data.
        /// </summary>
        public static void RgbsToColors<T>(IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo, IList<Color> colors)
            where T : struct, IEquatable<T>
        {
            int numColors = colors.Count;
            for (int iColor = 0; iColor < numColors; iColor++)
            {
                RgbColor<T> rgbColor = rgbColors[iColor];

                byte red = ColorConversion.ConvertValue(rgbColor.Red, rgbValueInfo, ByteColorValueTypeInfo.Instance);
                byte green = ColorConversion.ConvertValue(rgbColor.Green, rgbValueInfo, ByteColorValueTypeInfo.Instance);
                byte blue = ColorConversion.ConvertValue(rgbColor.Blue, rgbValueInfo, ByteColorValueTypeInfo.Instance);

                Color color = Color.FromArgb(red, green, blue);
                colors[iColor] = color;
            }
        }

        /// <summary>
        /// Parallel operation on lists of data (generally much (~33%) faster than the single threaded implementation).
        /// </summary>
        public static void RgbsToColorsParallel<T>(IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo, IList<Color> colors)
            where T : struct, IEquatable<T>
        {
            int numColors = colors.Count;
            Parallel.For(0, numColors, (x) => ColorConversion.RgbsToColorsParallelMethod(x, rgbColors, rgbValueInfo, colors));
        }

        private static void RgbsToColorsParallelMethod<T>(int iColor, IList<RgbColor<T>> rgbColors, IColorValueTypeInfo<T> rgbValueInfo, IList<Color> colors)
            where T : struct, IEquatable<T>
        {
            RgbColor<T> rgbColor = rgbColors[iColor];

            byte red = ColorConversion.ConvertValue(rgbColor.Red, rgbValueInfo, ByteColorValueTypeInfo.Instance);
            byte green = ColorConversion.ConvertValue(rgbColor.Green, rgbValueInfo, ByteColorValueTypeInfo.Instance);
            byte blue = ColorConversion.ConvertValue(rgbColor.Blue, rgbValueInfo, ByteColorValueTypeInfo.Instance);

            Color color = Color.FromArgb(red, green, blue);
            colors[iColor] = color;
        }

        #endregion

        //public static void ColorToRgb(Color color, out RgbColor rgbColor)
        //{
        //    double red = ColorConversion.LevelToValue(color.R);
        //    double green = ColorConversion.LevelToValue(color.G);
        //    double blue = ColorConversion.LevelToValue(color.B);

        //    rgbColor = new RgbColor(red, green, blue);
        //}

        ///// <summary>
        ///// Calculates the HSV color values for a set of RGB color values. Input values must be on the range [0, 1]. Output values are in the range [0, 1].
        ///// </summary>
        ///// <remarks>
        ///// Adapted from: http://www.rapidtables.com/convert/color/rgb-to-hsv.htm.
        ///// </remarks>
        //public static void RgbToHsv(double red, double green, double blue, out double hue, out double saturation, out double value)
        //{
        //    double colorMax = Math.Max(Math.Max(red, green), blue);
        //    double colorMin = Math.Min(Math.Min(red, green), blue);

        //    double delta = colorMax - colorMin;

        //    // Value is easy.
        //    value = colorMax;

        //    // Saturation is medium.
        //    if (0 == colorMax)
        //    {
        //        saturation = 0;
        //    }
        //    else
        //    {
        //        saturation = delta / colorMax;
        //    }

        //    // Hue is hard.
        //    double unNormalizedHue;
        //    if (0 == delta)
        //    {
        //        unNormalizedHue = 0;
        //    }
        //    else
        //    {
        //        if (red == colorMax)
        //        {
        //            unNormalizedHue = (green - blue) / delta;
        //            if (0 > unNormalizedHue)
        //            {
        //                unNormalizedHue += 6;
        //            }
        //        }
        //        else
        //        {
        //            if (green == colorMax)
        //            {
        //                unNormalizedHue = (blue - red) / delta + 2;
        //            }
        //            else
        //            {
        //                // Blue is the max color.
        //                unNormalizedHue = (blue - red) / delta + 4;
        //            }
        //        }
        //    }

        //    hue = unNormalizedHue / 6;
        //}

        //public static void RgbToHsv(RgbColor rgbColor, out double hue, out double saturation, out double value)
        //{
        //    ColorConversion.RgbToHsv(rgbColor.Red, rgbColor.Green, rgbColor.Blue, out hue, out saturation, out value);
        //}

        //public static void RgbToHsv(RgbColor rgbColor, out HsvColor hsvColor)
        //{
        //    hsvColor = new HsvColor(rgbColor);
        //}

        ///// <summary>
        ///// Calculates the RGB color values for a set of HSV color values. Input values must be on the range [0, 1]. Output values are in the range [0, 1].
        ///// </summary>
        ///// <remarks>
        ///// Adapted from: http://www.rapidtables.com/convert/color/hsv-to-rgb.htm.
        ///// </remarks>
        //public static void HsvToRgb(double hue, double saturation, double value, out double red, out double green, out double blue)
        //{
        //    double hexaHue = hue * 6;

        //    double delta = value * saturation;

        //    double redPrime = 0;
        //    double greenPrime = 0;
        //    double bluePrime = 0;

        //    double x = 0;
        //    if (hexaHue < 2)
        //    {
        //        x = delta * (1 - Math.Abs(hexaHue - 1));

        //        if (hexaHue < 1)
        //        {
        //            redPrime = delta;
        //            greenPrime = x;
        //        }
        //        else
        //        {
        //            redPrime = x;
        //            greenPrime = delta;
        //        }
        //    }
        //    else
        //    {
        //        if (hexaHue < 4)
        //        {
        //            x = delta * (1 - Math.Abs(hexaHue - 3));

        //            if (hexaHue < 3)
        //            {
        //                greenPrime = delta;
        //                bluePrime = x;
        //            }
        //            else
        //            {
        //                greenPrime = x;
        //                bluePrime = delta;
        //            }
        //        }
        //        else
        //        {
        //            x = delta * (1 - Math.Abs(hexaHue - 5));

        //            if (hexaHue < 5)
        //            {
        //                redPrime = x;
        //                bluePrime = delta;
        //            }
        //            else
        //            {
        //                redPrime = delta;
        //                bluePrime = x;
        //            }
        //        }
        //    }

        //    double m = value - delta;

        //    red = redPrime + m;
        //    green = greenPrime + m;
        //    blue = bluePrime + m;
        //}

        //public static void HsvToRgb(HsvColor hsvColor, out double red, out double green, out double blue)
        //{
        //    ColorConversion.HsvToRgb(hsvColor.Hue, hsvColor.Saturation, hsvColor.Value, out red, out green, out blue);
        //}

        //public static void HsvToRgb(HsvColor hsvColor, out RgbColor rgbColor)
        //{
        //    double red; double green; double blue;
        //    ColorConversion.HsvToRgb(hsvColor, out red, out green, out blue);

        //    rgbColor = new RgbColor(red, green, blue);
        //}

        ///// <summary>
        ///// Calculates a gray value for a set of RGB color values. Input values must be on the range [0, 1]. Output value is in the range [0, 1].
        ///// </summary>
        ///// <remarks>
        ///// Coefficients taken from MATLAB's rgb2gray function.
        ///// </remarks>
        //public static double RgbToGray(double red, double green, double blue)
        //{
        //    double output = ColorConversion.GrayRedComponent * red + ColorConversion.GrayGreenComponent * green + ColorConversion.GrayBlueComponent * blue;
        //    return output;
        //}

        //public static double RgbToGray(RgbColor rgbColor)
        //{
        //    double output = ColorConversion.RgbToGray(rgbColor.Red, rgbColor.Green, rgbColor.Blue);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates a gray value for a set of RGB color values. Output value is in the range [0, 1].
        ///// </summary>
        ///// <remarks>
        ///// Coefficients taken from MATLAB's rgb2gray function.
        ///// </remarks>
        //public static double RgbToGray(byte red, byte green, byte blue)
        //{
        //    double output = ColorConversion.RgbToGray((double)red, (double)green, (double)blue);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the level for a value in the range [0, 1] given a maximum value of the level.
        ///// </summary>
        ///// <param name="maxLevel">
        ///// The maximum level parameter is a double to suggest that the conversion of the maximum level (usually an integer) to a double should be done once, instead of every iteration loop.
        ///// </param>
        ///// <remarks>
        ///// Color values by default should be in the range [0, 1], but when reading or writing colors externally, generally bytes in the range [0, 255] are used.
        ///// This function allows easy conversion from color values to color levels.
        ///// </remarks>
        //public static int ValueToLevel(double value, double maxLevel)
        //{
        //    int output = (int)Math.Round(value * maxLevel);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the level for a value in the range [0, 1] assuming the level is on the range [0, 255];
        ///// </summary>
        //public static int ValueToLevel(double value)
        //{
        //    int output = ColorConversion.ValueToLevel(value, ColorConversion.ByteColorMaxValue);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the level for a value in the range [0, 1] given a maximum value of the level.
        ///// </summary>
        ///// <remarks>
        ///// Color values by default should be in the range [0, 1], but when reading or writing colors externally, generally bytes in the range [0, 255] are used.
        ///// This function allows easy conversion from color values to color levels.
        ///// </remarks>
        //public static byte ValueToLevelByte(double value, double maxLevel)
        //{
        //    byte output = (byte)Math.Round(value * maxLevel);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the level for a value in the range [0, 1] assuming the level is on the range [0, 255];
        ///// </summary>
        //public static byte ValueToLevelByte(double value)
        //{
        //    byte output = ColorConversion.ValueToLevelByte(value, ColorConversion.ByteColorMaxValue);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the color value for a level given the maximum possible value for the level.
        ///// </summary>
        ///// <remarks>
        ///// The color value is on the range [0, 1], while the level is generally on the range [0, 255]. But the max level can be specified.
        ///// </remarks>
        //public static double LevelToValue(int level, double maxLevel)
        //{
        //    double output = (double)level / maxLevel;
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the color value for a level assuming the level is on the range [0, 255].
        ///// </summary>
        //public static double LevelToValue(int level)
        //{
        //    double output = ColorConversion.LevelToValue(level, ColorConversion.ByteColorMaxValue);
        //    return output;
        //}

        ///// <summary>
        ///// Calculates the color value for a level given the maximum possible value for the level.
        ///// </summary>
        ///// <remarks>
        ///// The color value is a double on the range [0, 1], while the level is generally a byte on the range [0, 255].
        ///// </remarks>
        //public static double LevelToValue(byte level)
        //{
        //    double output = (double)level / ColorConversion.ByteColorMaxValue;
        //    return output;
        //}
    }
}
