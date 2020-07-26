using System;

namespace Bolao.Pinheiros.Utils
{
    public static class MathUtils
    {
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