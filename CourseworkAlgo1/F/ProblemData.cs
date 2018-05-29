using System;
using System.Numerics;

namespace CourseworkAlgo1.F
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

        public double Prec { get; set; } = 1e-4;

        public Func<double, double, Complex> P { get; set; } = (ksi1, ksi2) => 1;
        public Func<double, double, double> AbsF { get; set; } = (ksi1, ksi2) => 1;
        public Func<double, double, double> ArgF { get; set; } = (ksi1, ksi2) => 0;

        public double Alpha
        {
            get => _alpha ?? C1 * C2 / Math.Pow(2 * Math.PI, 2);
            set => _alpha = value;
        }

        public bool IsLambdaConst { get; set; }

        public Complex[][] GetInitialF()
        {
            var initF = new Complex[Ksi1.PartitionsAmount][];
            for (var i = 0; i < Ksi1.PartitionsAmount; i++)
            {
                initF[i] = new Complex [Ksi2.PartitionsAmount];
                for (var j = 0; j < Ksi2.PartitionsAmount; j++)
                {
                    var ksi1 = Ksi1.Step * i + Ksi1.Begin;
                    var ksi2 = Ksi2.Step * j + Ksi2.Begin;
                    var absF = AbsF(ksi1, ksi2);
                    var argF = ArgF(ksi1, ksi2);
                    initF[i][j] = new Complex(absF * Math.Cos(argF), absF * Math.Sin(argF));
                }
            }

            return initF;
        }
    }
}
