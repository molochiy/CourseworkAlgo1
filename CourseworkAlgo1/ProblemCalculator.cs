using System;
using System.Linq;
using System.Numerics;

namespace CourseworkAlgo1
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

        public Complex[][] GetF(Complex[][] i, (double begin, double end) ksi1,
            (double begin, double end) ksi2, (int ksi1, int ksi2) partitionsAmount)
        {
            var fFunc = GetFFunction(i);

            var ksi1Length = ksi1.end - ksi1.begin;
            var ksi1Step = ksi1Length / partitionsAmount.ksi1;

            var ksi2Length = ksi2.end - ksi2.begin;
            var ksi2Step = ksi2Length / partitionsAmount.ksi2;

            var f = new Complex[partitionsAmount.ksi1 + 1][];
            for (var j = 0; j < partitionsAmount.ksi1 + 1; j++)
            {
                f[j] = new Complex[partitionsAmount.ksi2 + 1];
                for (var k = 0; k < partitionsAmount.ksi2 + 1; k++)
                {
                    f[j][k] = fFunc(ksi1.begin + j * ksi1Step,
                        ksi2.begin + k * ksi2Step);
                }
            }

            return f;
        }

        public Complex GetFunctionalValue(Complex[][] i, Complex lambda)
        {
            var funcValue = IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2);
                    return res * res + lambda * res;
                },
                (-1, 1),
                (-1, 1));

            var sumI = i.Sum(iArr => iArr.Sum(iEl => Math.Pow(iEl.Magnitude, 2)));

            return funcValue + sumI;
        }

        // IproblemData.Nm = integral{2 * A(ksi1, ksi2) + lambda * B(ksi1, ksi2)} dksi1 dksi2
        public Complex[][] CalculateI(Complex[][] i, Complex lambda)
        {
            var nextI = GetZeroI();

            var f = GetFFunction(i);

            for (var n = -_problemData.N; n <= _problemData.N; n++)
            {
                for (var m = -_problemData.M; m <= _problemData.M; m++)
                {
                    var aFunc = GetAFunction(f, n, m);
                    var bFunc = GetBFunction(f, n, m);

                    nextI[n + _problemData.N][m + _problemData.M] = IntegralCalculator.ComputeIntegral(
                                                (ksi1, ksi2) => 2 * aFunc(ksi1, ksi2) + lambda * bFunc(ksi1, ksi2),
                                                (-1, 1),
                                                (-1, 1))
                                            / _problemData.Alpha;

                    //Console.WriteLine($"I[{n + _problemData.N}][{m + _problemData.M}] = {nextI[n + _problemData.N][m + _problemData.M]}");
                }
            }

            return nextI;
        }

        // nextLambda = prevLambda + gamma * grad(prevI, prevLambda)
        public Complex CalculateLambda(Complex[][] prevI, Complex[][] nextI, Complex prevLambda)
        {
            var grad = GetСonditionFuncValue(prevI);
            // var nextLambda = prevLambda - GetGammaFromFunc(nextI, prevLambda, grad) * grad;
            var nextLambda = prevLambda - GetFunctionalValue(prevI, prevLambda) / Math.Pow(grad.Magnitude, 2) * grad;

            return nextLambda;
        }

        public static void NormI(Complex[][] i)
        {
            var sum = i.Sum(ii => ii.Sum(v => Math.Pow(v.Magnitude, 2)));
            sum = Math.Sqrt(sum);

            foreach (var arr in i)
            {
                for (var k = 0; k < arr.Length; k++)
                {
                    arr[k] = arr[k] / sum;
                }
            }
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

        // A(ksi1, ksi2)=[P(ksi1, ksi2) - |f(ksi1, ksi2)|^2] * B(ksi1, ksi2)
        private Func<double, double, Complex> GetAFunction(Func<double, double, Complex> f, int n, int m)
        {
            return (ksi1, ksi2) => (_problemData.P(ksi1, ksi2) - Math.Pow(f(ksi1, ksi2).Magnitude, 2))
                                   * GetBFunction(f, n, m)(ksi1, ksi2);
        }

        // B(ksi1, ksi2) = e^-i*(c1*n*ksi1+c2*m*ksi2) * f(ksi1, ksi2)
        private Func<double, double, Complex> GetBFunction(Func<double, double, Complex> f, int n, int m)
        {
            return (ksi1, ksi2) =>
            {
                var angle = _problemData.C1 * n * ksi1 + _problemData.C2 * m * ksi2;
                return new Complex(Math.Cos(-angle), Math.Sin(-angle))
                       * f(ksi1, ksi2);
            };
        }

        private Complex GetСonditionFuncValue(Complex[][] i)
        {
            return IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) => _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(i)(ksi1, ksi2).Magnitude, 2),
                (-1, 1),
                (-1, 1));
        }

        private Complex GetGammaFromFunc(Complex[][] nextI, Complex prevLambda, Complex grad)
        {
            var condFuncValueForNextI = GetСonditionFuncValue(nextI);
            var funcValue = IntegralCalculator.ComputeIntegral(
                (ksi1, ksi2) =>
                {
                    var res = _problemData.P(ksi1, ksi2) - Math.Pow(GetFFunction(nextI)(ksi1, ksi2).Magnitude, 2);
                    return res * res;
                },
                (-1, 1),
                (-1, 1));

            var sumI = nextI.Sum(iArr => iArr.Sum(i => Math.Pow(i.Magnitude, 2)));

            return funcValue / condFuncValueForNextI + _problemData.Alpha * sumI / condFuncValueForNextI + prevLambda / grad;
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
