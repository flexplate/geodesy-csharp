using System;

namespace geodesy
{
    public static class DoubleExtensions
    {
        public static double ToDegrees(this double radians)
        {
            return radians * (180 / Math.PI);
        }

        public static double ToRadians(this double degrees)
        {
            return (degrees * Math.PI) / 180;
        }

        public static string ToFormattedString(this double input, int minLength, int decimals)
        {
            string Zeros = "0";
            while (Zeros.Length < minLength) { Zeros += "0"; }
            string Precision = "";
            while (Precision.Length < decimals) { Precision += "0"; }
            string Format = Zeros + "." + Precision;
            return input.ToString(Format);
        }
    }
}
