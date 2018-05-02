using System;
using System.Numerics;

namespace CourseworkAlgo1
{
    public class ProblemData
    {
        private double? _alpha;

        public int M { get; set; } = 2;
        public int N { get; set; } = 2;

        public (double begin, double end) Ksi1 { get; set; } = (-1, 1);
        public (double begin, double end) Ksi2 { get; set; } = (-1, 1);
        public (int ksi1, int ksi2) PartitionsAmount { get; set; } = (10, 10);

        public double C1 { get; set; } = 0.5;
        public double C2 { get; set; } = 0.75;

        public double Prec { get; set; } = 1e-6;

        public Func<Complex, Complex, Complex> P { get; set; } = (ksi1, ksi2) => 1;

        public double Alpha
        {
            get => _alpha ?? C1 * C2 / Math.Pow(2 * Math.PI, 2);
            set => _alpha = value;
        }

        public bool UseFunctionToFindLambda { get; set; } = true;
    }
}
