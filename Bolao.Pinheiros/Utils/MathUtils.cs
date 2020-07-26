using System;

namespace Bolao.Pinheiros.Utils
{
    public static class MathUtils
    {
        public static double ToDecimalFormat(this double value)
        {
            return Math.Round(value, 2);
        }

        public static double CalcPercent(int value, int total)
        {
            return CalcPercent((double)value, (double)total);
        }

        public static double CalcPercent(double value, double total)
        {
            return Math.Round(value / total * 100, 2);
        }
    }
}