using System;
using System.Linq;
using System.Numerics;

namespace CourseworkAlgo1.I
{
    public class ProblemData
    {
        private double? _alpha;

        public int M { get; set; } = 2;
        public int N { get; set; } = 2;

        public KsiData Ksi1 { get; set; } = new KsiData();
        public KsiData Ksi2 { get; set; } = new KsiData();

        public double C1 { get; set; } = 0.5;
        public double C2 { get; set; } = 0.75;

        public double Prec { get; set; } = 1e-6;

        public Func<double, double, Complex> P { get; set; } = (ksi1, ksi2) => 1;

        public double Alpha
        {
            get => _alpha ?? C1 * C2 / Math.Pow(2 * Math.PI, 2);
            set => _alpha = value;
        }

        public bool UseFunctionToFindLambda { get; set; } = false;

        public Complex[][] GetInitialI()
        {
            var I = new Complex[2 * N + 1][];
            //var oscillatorAmount = (2 * m + 1) * (2 * n + 1);
            for (var i = 0; i < 2 * M + 1; i++)
            {
                I[i] = Enumerable.Repeat<Complex>(1, 2 * M + 1).ToArray();
            }

            ProblemCalculator.NormI(I);

            return I;
        }
    }
}
