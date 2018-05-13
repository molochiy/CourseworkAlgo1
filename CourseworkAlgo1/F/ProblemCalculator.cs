using System;
using System.Linq;
using System.Numerics;

namespace CourseworkAlgo1.F
{
    public class ProblemCalculator
    {
        private readonly ProblemData _problemData;

        public ProblemCalculator(ProblemData problemData)
        {
            _problemData = problemData;
        }

        public Complex[][] CalculateNextF(Complex[][] f, Complex lambda)
        {
            var nextF = new Complex[_problemData.Ksi1.PartitionsAmount][];
            for (var i = 0; i < _problemData.Ksi1.PartitionsAmount; i++)
            {
                nextF[i] = new Complex[_problemData.Ksi2.PartitionsAmount];
                for (var j = 0; j < _problemData.Ksi2.PartitionsAmount; j++)
                {
                    var i1 = i;
                    var j1 = j;

                    Complex Func(double ksi1, double ksi2) =>
                        GetSubIntegralCommonFunc(f, lambda)(ksi1, ksi2) *
                        GetKFunction(_problemData.Ksi1.GetKsiForPartition(i1),
                            _problemData.Ksi2.GetKsiForPartition(j1))(ksi1, ksi2);

                    nextF[i][j] = IntegralCalculator.ComputeIntegralForProblem(Func, _problemData) / _problemData.Alpha;
                }
            }

            return nextF;
        }

        public void NormF(Complex[][] f)
        {
            /*var fNorm = IntegralCalculator.ComputeIntegralForProblem(
                (ksi1, ksi2) => Math.Pow(f[_problemData.Ksi1.GetParitionForKsi(ksi1)][_problemData.Ksi2.GetParitionForKsi(ksi2)].Magnitude, 2),
                _problemData);

            foreach (var fArray in f)
            {
                for (var j = 0; j < fArray.Length; j++)
                {
                    fArray[j] /= Complex.Sqrt(fNorm);
                }
            }*/

            var max = f.Max(ii => ii.Max(v => v.Magnitude));

            foreach (var arr in f)
            {
                for (var k = 0; k < arr.Length; k++)
                {
                    arr[k] = arr[k] / max;
                }
            }
        }

        public Complex CalculateNextLambda(Complex[][] f, Complex lambda)
        {
            return lambda;
            var grad = GetGFuncValue(f);
            var I = CalculateI(f, lambda);
            var functionValue = false ? CalculateFunctionValue(I) : CalculateFunctionValue(f, I);
            var lagrangeFunctionValue = functionValue + lambda * grad;
            // newton like
            return lambda - grad * lagrangeFunctionValue / Math.Pow(grad.Magnitude, 2);
            // from Lagrange functional
            // return lambda - (functionValue / grad + lambda);
        }

        public Complex CalculateFunctionValue(Complex[][] i)
        {
            var funcValue = IntegralCalculator.ComputeIntegralForProblem(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2);
                    return res * res;
                },
                _problemData);

            var sumI = i.Sum(iArr => iArr.Sum(iEl => Math.Pow(iEl.Magnitude, 2)));

            return funcValue + _problemData.Alpha * sumI;
        }

        public Complex CalculateFunctionValue(Complex[][] f, Complex[][] i)
        {
            var funcValue = IntegralCalculator.ComputeIntegralForProblem(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(f[_problemData.Ksi1.GetParitionForKsi(ksi1)][_problemData.Ksi2.GetParitionForKsi(ksi2)].Magnitude, 2);
                    return res * res;
                },
                _problemData);

            var sumI = i.Sum(iArr => iArr.Sum(iEl => Math.Pow(iEl.Magnitude, 2)));

            return funcValue + _problemData.Alpha * sumI;
        }

        public Complex[][] GetF(Complex[][] i, KsiData ksi1, KsiData ksi2)
        {
            var fFunc = GetFFunction(i);

            var f = new Complex[ksi1.PartitionsAmount][];
            for (var j = 0; j < ksi1.PartitionsAmount; j++)
            {
                f[j] = new Complex[ksi2.PartitionsAmount];
                for (var k = 0; k < ksi2.PartitionsAmount; k++)
                {
                    var value = fFunc(ksi1.GetKsiForPartition(j), ksi2.GetKsiForPartition(k));
                    f[j][k] = value.Magnitude;
                }
            }

            return f;
        }

        // F(ksi1, ksi2) = Sum_nSum_m[Inm*e^i*(c1*n*ksi1+c2*m*ksi2)]
        private Func<double, double, Complex> GetFFunction(Complex[][] i)
        {
            return (ksi1, ksi2) =>
            {
                var result = new Complex();
                for (var n = -_problemData.N; n <= _problemData.N; n++)
                {
                    for (var m = -_problemData.M; m <= _problemData.M; m++)
                    {
                        var angle = _problemData.C1 * n * ksi1 + _problemData.C2 * m * ksi2;
                        result += i[n + _problemData.N][m + _problemData.M] * new Complex(Math.Cos(angle), Math.Sin(angle));
                    }

                }

                return result;
            };
        }

        public Complex GetGFuncValue(Complex[][] f)
        {
            return IntegralCalculator.ComputeIntegralForProblem(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(f[_problemData.Ksi1.GetParitionForKsi(ksi1)][_problemData.Ksi2.GetParitionForKsi(ksi2)].Magnitude, 2);
                    return res; //.Magnitude;
                },
                _problemData);
        }

        public Complex GetSigmaWitoutIFuncValue(Complex[][] f)
        {
            return IntegralCalculator.ComputeIntegralForProblem(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(f[_problemData.Ksi1.GetParitionForKsi(ksi1)][_problemData.Ksi2.GetParitionForKsi(ksi2)].Magnitude, 2);
                    return res * res;;
                },
                _problemData);
        }

        private Func<double, double, Complex> GetKFunction(double inKsi1, double inKsi2)
        {
            return (ksi1, ksi2) =>
            {
                var result = new Complex();
                for (var n = -_problemData.N; n <= _problemData.N; n++)
                {
                    for (var m = -_problemData.M; m <= _problemData.M; m++)
                    {
                        var angle = _problemData.C1 * n * (inKsi1 - ksi1) + _problemData.C2 * m * (inKsi2 - ksi2);
                        result += new Complex(Math.Cos(angle), Math.Sin(angle));
                    }

                }

                return result;
            };
        }

        public Complex[][] CalculateI(Complex[][] f, Complex lambda)
        {
            var I = new Complex[2 * _problemData.N + 1][];
            for (var i = 0; i < 2 * _problemData.N + 1; i++)
            {
                I[i] = new Complex[2 * _problemData.M + 1];
            }

            for (var n = -_problemData.N; n <= _problemData.N; n++)
            {
                for (var m = -_problemData.M; m <= _problemData.M; m++)
                {
                    var n1 = n;
                    var m1 = m;

                    Complex Func(double ksi1, double ksi2)
                    {
                        var angle = -(_problemData.C1 * n1 * ksi1 + _problemData.C2 * m1 * ksi2);
                        return GetSubIntegralCommonFunc(f, lambda)(ksi1, ksi2) * new Complex(Math.Cos(angle), Math.Sin(angle));
                    }

                    I[n + _problemData.N][m + _problemData.M] += IntegralCalculator.ComputeIntegralForProblem(Func, _problemData);
                }
            }

            return I;
        }

        private Func<double, double, Complex> GetSubIntegralCommonFunc(Complex[][] f, Complex lambda)
        {
            return (double ksi1, double ksi2) =>
            {
                var indexKsi1 = _problemData.Ksi1.GetParitionForKsi(ksi1);
                var indexKsi2 = _problemData.Ksi2.GetParitionForKsi(ksi2);
                return (2 * (_problemData.P(ksi1, ksi2) - Math.Pow(f[indexKsi1][indexKsi2].Magnitude, 2)) + lambda) *
                       f[indexKsi1][indexKsi2];
            };
        }
    }
}
