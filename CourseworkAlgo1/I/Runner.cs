using System;
using System.Numerics;

namespace CourseworkAlgo1.I
{
    public class Runner
    {
        public static void Run()
        {
            var problemDataVariants = new[]
            {
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 0.85,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2)
                }, "V1_1_1"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 1,
                    C2 = 1,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2)
                }, "V1_1_2"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 1.2,
                    C2 = 1.2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2)
                }, "V1_1_3"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 0.85,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1
                }, "V2_1_1"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 1,
                    C2 = 1,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1
                }, "V2_1_2"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 1.2,
                    C2 = 1.2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1
                }, "V2_1_3"),
                (new ProblemData
                {
                    N = 2,
                    M = 2,
                    C1 = 1,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Abs(Math.Sin(Math.PI * ksi2)), 2)
                }, "V3_1_1")
            };

            foreach (var problemDataVariant in problemDataVariants)
            {
                RunForProblemData(problemDataVariant);
            }
        }

        private static void RunForProblemData((ProblemData problemData, string variant) problemDataVariant)
        {
            try
            {
                Console.WriteLine($"I {problemDataVariant.variant} started.");
                var problemCalculator = new ProblemCalculator(problemDataVariant.problemData);

                var prevI = problemDataVariant.problemData.GetInitialI();
                //var prevI = InitDifferentI(problemData.N, problemData.M);
                Complex prevLambda = 0.001;

                CheckInputValues(prevI, prevLambda, problemCalculator);
                var runTime = DateTime.Now;
                int iteration = 0;
                var iterationsFileName =
                    $"{problemDataVariant.variant}\\Iterations_{runTime:yyyy-MM-dd_hh-mm-ss-fff}.txt";

                Logger.WriteIIterationToFile(problemDataVariant.problemData, problemCalculator, prevI, prevLambda,
                    iteration, iterationsFileName);

                var (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);

                Logger.WriteIIterationToFile(problemDataVariant.problemData, problemCalculator, nextI, nextLambda,
                    ++iteration, iterationsFileName);
                while (!IsSatisfyPrec(prevI, nextI, problemDataVariant.problemData.Prec) && iteration < 0.5e3)
                {
                    Console.WriteLine($"Iteration: {iteration}");

                    prevI = nextI;
                    prevLambda = nextLambda;

                    (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);

                    Logger.WriteIIterationToFile(problemDataVariant.problemData, problemCalculator, nextI, nextLambda,
                        ++iteration, iterationsFileName);
                }

                problemDataVariant.problemData.Ksi1.Begin = -2;
                problemDataVariant.problemData.Ksi2.Begin = -2;
                problemDataVariant.problemData.Ksi1.End = 2;
                problemDataVariant.problemData.Ksi2.End = 2;
                var f = problemCalculator.GetF(nextI, problemDataVariant.problemData.Ksi1,
                    problemDataVariant.problemData.Ksi2);
                ProblemCalculator.NormF(f);
                Logger.WriteResultsI(nextI, f, problemDataVariant.problemData, problemDataVariant.variant, runTime);

                Console.WriteLine($"I {problemDataVariant.variant} finished.");
            }
            catch
            {
                // ignored
            }
        }

        private static void CheckInputValues(Complex[][] I, Complex lambda, ProblemCalculator problemCalculator)
        {
            var functionalValue = problemCalculator.GetLagrangeFunctionValue(I, lambda);
            if (functionalValue.Real < 0)
            {
                throw new ArgumentException("Initial values is not valid!");
            }
        }

        private static bool IsSatisfyPrec(Complex[][] prevI, Complex[][] nextI, double prec)
        {
            for (var j = 0; j < prevI.Length; j++)
            {
                for (var k = 0; k < prevI[j].Length; k++)
                {
                    if (Math.Abs(prevI[j][k].Magnitude - nextI[j][k].Magnitude) > prec)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
