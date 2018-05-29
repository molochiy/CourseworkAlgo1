using System;
using System.Numerics;

namespace CourseworkAlgo1.F
{
    public class Runner
    {
        public static void Run()
        {
            var problemDataVariants = new[]
            {
                /*(new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 0.85,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    AbsF = (ksi1, ksi2) => 1,
                    ArgF = (ksi1, ksi2) => 0,
                    IsLambdaConst = true
                }, "V1_1_1_res"),*//*
                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 1,
                    C2 = 1,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    AbsF = (ksi1, ksi2) => 1,
                    ArgF = (ksi1, ksi2) => 0
                }, "V1_1_2"),
                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 1.2,
                    C2 = 1.2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    AbsF = (ksi1, ksi2) => 1,
                    ArgF = (ksi1, ksi2) => 0
                }, "V1_1_3"),
                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 0.85,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1,
                    AbsF = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    ArgF = (ksi1, ksi2) => 0,
                    IsLambdaConst = true
                }, "V2_1_1"),*/

                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 2,
                    C2 = 2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1,
                    AbsF = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    ArgF = (ksi1, ksi2) => 0,
                    //IsLambdaConst = true
                }, "V2_1_2_res"),
                
                /*
                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 1.2,
                    C2 = 1.2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1,
                    AbsF = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    ArgF = (ksi1, ksi2) => 0
                }, "V2_1_3"),*/

                /*(new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 1,
                    C2 = 0.85,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Abs(Math.Sin(Math.PI * ksi2)), 2),
                    AbsF = (ksi1, ksi2) => 1,
                    ArgF = (ksi1, ksi2) => 0,
                    IsLambdaConst = true
                    //Ksi1 = new KsiData {Step = 0.01},
                    //Ksi2 = new KsiData {Step = 0.01}
                }, "V3_1_1_res")*/

                /*(new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 2,
                    C2 = 2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1,
                    AbsF = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    ArgF = (ksi1, ksi2) => 0
                }, "V2_1_1"),
                (new ProblemData
                {
                    N = 5,
                    M = 5,
                    C1 = 2,
                    C2 = 2,
                    Alpha = 0.1,
                    P = (ksi1, ksi2) => 1,
                    AbsF = (ksi1, ksi2) =>
                        Math.Pow(Math.Cos(Math.PI * ksi1 / 2), 2) * Math.Pow(Math.Cos(Math.PI * ksi2 / 2), 2),
                    ArgF = (ksi1, ksi2) => 0,
                    IsLambdaConst = true
                }, "V2_1_1")*/
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
                Console.WriteLine($"F {problemDataVariant.variant} started.");

                var problemCalculator = new ProblemCalculator(problemDataVariant.problemData);

                var runTime = DateTime.Now;
                int iteration = 0;
                var iterationsFileName =
                    $"{problemDataVariant.variant}\\Iterations_{runTime:yyyy-MM-dd_hh-mm-ss-fff}.txt";

                var prevF = problemDataVariant.problemData.GetInitialF();
                Complex prevLambda = 0;
                Logger.WriteFIterationToFile(problemDataVariant.problemData, problemCalculator, prevF, prevLambda, iteration,
                    iterationsFileName);

                var nextF = problemCalculator.CalculateNextF(prevF, prevLambda);
                //problemCalculator.NormF(nextF);
                var nextLambda = problemCalculator.CalculateNextLambda(nextF, prevLambda);
                Logger.WriteFIterationToFile(problemDataVariant.problemData, problemCalculator, nextF, nextLambda, ++iteration,
                    iterationsFileName);

                while (!IsSatisfyPrec(prevF, nextF, problemDataVariant.problemData.Prec) && iteration < 100)
                {
                    Console.WriteLine($"Iteration: {iteration}");

                    prevF = nextF;
                    prevLambda = nextLambda;

                    nextF = problemCalculator.CalculateNextF(prevF, prevLambda);
                    //problemCalculator.NormF(nextF);
                    nextLambda = problemCalculator.CalculateNextLambda(nextF, prevLambda);

                    Logger.WriteFIterationToFile(problemDataVariant.problemData, problemCalculator, nextF, nextLambda, ++iteration,
                        iterationsFileName);
                }

                var I = problemCalculator.CalculateI(nextF, nextLambda);

                problemDataVariant.problemData.Ksi1.Begin = -2;
                problemDataVariant.problemData.Ksi2.Begin = -2;
                problemDataVariant.problemData.Ksi1.End = 2;
                problemDataVariant.problemData.Ksi2.End = 2;
                var f = problemCalculator.GetF(I, problemDataVariant.problemData.Ksi1,
                    problemDataVariant.problemData.Ksi2);
                problemCalculator.NormF(f);
                Logger.WriteResultsF(I, f, problemDataVariant.problemData, problemDataVariant.variant, runTime);


                Console.WriteLine($"F {problemDataVariant.variant} finished.");
            }
            catch
            {
                // ignored
            }
        }

        private static bool IsSatisfyPrec(Complex[][] prevF, Complex[][] nextF, double prec)
        {
            for (var j = 0; j < prevF.Length; j++)
            {
                for (var k = 0; k < prevF[j].Length; k++)
                {
                    if (Math.Abs(prevF[j][k].Magnitude - nextF[j][k].Magnitude) > prec)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
    }
}
