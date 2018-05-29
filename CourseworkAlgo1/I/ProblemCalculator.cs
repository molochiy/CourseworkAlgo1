using System;
using System.Linq;
using System.Numerics;

namespace CourseworkAlgo1.I
{
    public class ProblemCalculator
    {
        private readonly ProblemData _problemData;

        public ProblemCalculator(ProblemData problemData)
        {
            _problemData = problemData;
        }

        public (Complex[][], Complex) GetNextIAndLambda(Complex[][] i, Complex lambda)
        {
            var nextI = CalculateI(i, lambda);
            NormI(nextI);
            var nextLambda = CalculateLambda(i, nextI, lambda);

            return (nextI, nextLambda);
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

        public Complex[][] GetFPower(Complex[][] i, KsiData ksi1, KsiData ksi2)
        {
            var fFunc = GetFFunction(i);

            var f = new Complex[ksi1.PartitionsAmount][];
            for (var j = 0; j < ksi1.PartitionsAmount; j++)
            {
                f[j] = new Complex[ksi2.PartitionsAmount];
                for (var k = 0; k < ksi2.PartitionsAmount; k++)
                {
                    var value = fFunc(ksi1.GetKsiForPartition(j), ksi2.GetKsiForPartition(k));
                    f[j][k] = Math.Pow(value.Magnitude, 2);
                }
            }

            return f;
        }

        public Complex GetLagrangeFunctionValue(Complex[][] i, Complex lambda)
        {
            var funcValue = IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2);
                    return res * res;
                },
                (-1, 1),
                (-1, 1));

            var sumI = i.Sum(iArr => iArr.Sum(iEl => Math.Pow(iEl.Magnitude, 2)));

            var gValue = GetGFuncValue(i);

            return funcValue + _problemData.Alpha * sumI + lambda * gValue;
        }

        public Complex GetFunctionValue(Complex[][] i)
        {
            var funcValue = IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2);
                    return res * res;
                },
                (-1, 1),
                (-1, 1));

            var sumI = i.Sum(iArr => iArr.Sum(iEl => Math.Pow(iEl.Magnitude, 2)));

            return funcValue + _problemData.Alpha * sumI;
        }

        // Inm = integral{2 * A(ksi1, ksi2) + lambda * B(ksi1, ksi2)} dksi1 dksi2
        public Complex[][] CalculateI(Complex[][] i, Complex lambda)
        {
            var nextI = GetZeroI();

            for (var n = -_problemData.N; n <= _problemData.N; n++)
            {
                for (var m = -_problemData.M; m <= _problemData.M; m++)
                {
                            nextI[n + _problemData.N][m + _problemData.M] += IntegralCalculator.ComputeIntegral(
                                    GetIntegralFunction(i, lambda, n, m),
                                    (-1, 1),
                                    (-1, 1)
                                );

                    nextI[n + _problemData.N][m + _problemData.M] /= _problemData.Alpha;
                    // Console.WriteLine($"I[{n + _problemData.N}][{m + _problemData.M}] = {nextI[n + _problemData.N][m + _problemData.M]}");
                }
            }

            return nextI;
        }

        // nextLambda = prevLambda + gamma * grad(prevI, prevLambda)
        public Complex CalculateLambda(Complex[][] prevI, Complex[][] nextI, Complex prevLambda)
        {
            return prevLambda;
            return _problemData.UseFunctionToFindLambda
                    ? GetNextLambdaFromFunc(nextI, prevLambda)
                  :  GetNextLambdaByNewtonLikeMethod(prevI, prevLambda);
        }

        public Func<double, double, Complex> GetIntegralFunction(Complex[][] i, Complex lambda, int n, int m)
        {
            var f = GetFFunction(i);

            Complex EFunc(double ksi1, double ksi2)
            {
                var angle = _problemData.C1 * n * ksi1 + _problemData.C2 * m * ksi2;
                return new Complex(Math.Cos(-angle), Math.Sin(-angle));
            }

            return (ksi1, ksi2) => (2 * (_problemData.P(ksi1, ksi2) - Math.Pow(f(ksi1, ksi2).Magnitude, 2)) + lambda)
                                   * EFunc(ksi1, ksi2) * f(ksi1, ksi2);
        }

        public static void NormI(Complex[][] i)
        {
            var sum = i.Sum(ii => ii.Sum(v => Math.Pow(v.Magnitude, 2)));
            sum = Math.Sqrt(sum);

            // var sum = i.Max(ii => ii.Max(v => v.Magnitude));

            foreach (var arr in i)
            {
                for (var k = 0; k < arr.Length; k++)
                {
                    arr[k] = arr[k] / sum;
                }
            }
        }

        public static void NormF(Complex[][] f)
        {
            var max = f.Max(ii => ii.Max(v => v.Magnitude));

            foreach (var arr in f)
            {
                for (var k = 0; k < arr.Length; k++)
                {
                    arr[k] = arr[k] / max;
                }
            }
        }

        // F(ksi1, ksi2) = Sum_nSum_m[Inm*e^f*(c1*n*ksi1+c2*m*ksi2)]
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

        public Complex GetGFuncValue(Complex[][] i)
        {
            return IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2);
                    return res; //.Magnitude;
                },
                (-1, 1),
                (-1, 1));
        }

        // change return to prevLambda +- to change gradient method
        private Complex GetNextLambdaFromFunc(Complex[][] nextI, Complex prevLambda)
        {
            var gFuncValue = GetGFuncValue(nextI);
            var funcValue = IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(nextI)(ksi1, ksi2).Magnitude, 2);
                    return res * res;
                },
                (-1, 1),
                (-1, 1));

            var sumI = nextI.Sum(iArr => iArr.Sum(i => Math.Pow(i.Magnitude, 2)));

            return prevLambda - ((funcValue + _problemData.Alpha * sumI) / gFuncValue + prevLambda);
        }

        private Complex GetNextLambdaByNewtonLikeMethod(Complex[][] prevI, Complex prevLambda)
        {
            var grad = GetGFuncValue(prevI);
            return prevLambda - grad * GetLagrangeFunctionValue(prevI, prevLambda) / Math.Pow(grad.Magnitude, 2);
        }

        private Complex[][] GetZeroI()
        {
            var I = new Complex[2 * _problemData.N + 1][];
            for (var i = 0; i < 2 * _problemData.N + 1; i++)
            {
                I[i] = new Complex[2 * _problemData.M + 1];
            }

            return I;
        }
    }
}
