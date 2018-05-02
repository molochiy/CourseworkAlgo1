using System;
using System.Numerics;

namespace CourseworkAlgo1
{
    public static class IntegralCalculator
    {
        public static int InnerVarIntervalPartitionsAmount = 10;
        public static int OuterVarIntervalPartitionsAmount = 10;

        public static Complex ComputeIntegral(Func<double, double, Complex> function, (double begin, double end) innerVarClosedInterval, (double begin, double end) outerVarClosedInterval)
        {
            var innerVarStep = (innerVarClosedInterval.end - innerVarClosedInterval.begin) /
                               (2 * InnerVarIntervalPartitionsAmount);

            var outerVarStep = (outerVarClosedInterval.end - outerVarClosedInterval.begin) /
                               (2 * OuterVarIntervalPartitionsAmount);

            var sum = Complex.Zero;

            for (var i = 0; i < InnerVarIntervalPartitionsAmount; i++)
            {
                for (var j = 0; j < OuterVarIntervalPartitionsAmount; j++)
                {
                    sum += function(2 * i * innerVarStep + innerVarClosedInterval.begin, 2 * j * outerVarStep + outerVarClosedInterval.begin)
                           + 4 * function((2 * i + 1) * innerVarStep + innerVarClosedInterval.begin, 2 * j * outerVarStep + outerVarClosedInterval.begin)
                           + function((2 * i + 2) * innerVarStep + innerVarClosedInterval.begin, 2 * j * outerVarStep + outerVarClosedInterval.begin)
                           + 4 * function(2 * i * innerVarStep + innerVarClosedInterval.begin, (2 * j + 1) * outerVarStep + outerVarClosedInterval.begin)
                           + 16 * function((2 * i + 1) * innerVarStep + innerVarClosedInterval.begin, (2 * j + 1) * outerVarStep + outerVarClosedInterval.begin)
                           + 4 * function((2 * i + 2) * innerVarStep + innerVarClosedInterval.begin, (2 * j + 1) * outerVarStep + outerVarClosedInterval.begin)
                           + function(2 * i * innerVarStep + innerVarClosedInterval.begin, (2 * j + 2) * outerVarStep + outerVarClosedInterval.begin)
                           + 4 * function((2 * i + 1) * innerVarStep + innerVarClosedInterval.begin, (2 * j + 2) * outerVarStep + outerVarClosedInterval.begin)
                           + function((2 * i + 2) * innerVarStep + innerVarClosedInterval.begin, (2 * j + 2) * outerVarStep + outerVarClosedInterval.begin);
                }
            }

            return innerVarStep * outerVarStep * sum / 9;
        }
    }
}
