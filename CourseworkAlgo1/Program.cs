using System;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace CourseworkAlgo1
{
    class Program
    {
        private static void Main()
        {
            var problemData = new ProblemData { N = 2, M = 2, C1 = 2, C2 = 2, UseFunctionToFindLambda = true};

            StartV2(problemData);

            Console.ReadLine();
        }

        private static void StartV2(ProblemData problemData)
        {
            var prevI = InitI(problemData.N, problemData.M);
            //var prevI = InitDifferentI(problemData.N, problemData.M);
            Complex prevLambda = -0.001;

            var problemCalculator = new NewProblemCalculator(problemData);

            CheckInputValues(prevI, prevLambda, problemCalculator);
            var runTime = DateTime.Now;
            WriteIterationToFile(problemData, problemCalculator, prevI, prevLambda, 0, runTime);

            var (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);

            int counter = 1;
            WriteIterationToFile(problemData, problemCalculator, nextI, nextLambda, counter, runTime);
            while (!IsSatisfyPrec(prevI, nextI, problemData.Prec) && counter < 1e5)
            {
                Console.WriteLine($"Iteration: {counter}");
                prevI = nextI;
                prevLambda = nextLambda;

                (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);
                counter++;
                WriteIterationToFile(problemData, problemCalculator, nextI, nextLambda, counter, runTime);
            }

            var f = problemCalculator.GetFPower(nextI, problemData.Ksi1, problemData.Ksi2, problemData.PartitionsAmount);
            WriteResults(nextI, f, problemData, runTime);

            Console.WriteLine("Finished.");
        }

        private static void StartV1(ProblemData problemData)
        {
            var prevI = InitI(problemData.N, problemData.M);
            Complex prevLambda = -0.01;

            var problemCalculator = new ProblemCalculator(problemData);

            CheckInputValues(prevI, prevLambda, problemCalculator);

            var (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);

            // test sum = 696
            //NormI(new[]
            //{
            //    new Complex[] {1, 3, 5},
            //    new Complex[] {0, 2, 4},
            //    new Complex[] {11,22,6}
            //});

            /*int counter = 1;
            while (!IsSatisfyPrec(prevI, nextI) || counter < 10000)
            {
                Console.WriteLine($"Iteration: {counter}");
                prevI = nextI;
                prevLambda = nextLambda;

                (nextI, nextLambda) = problemCalculator.GetNextIAndLambda(prevI, prevLambda);
                counter++;

                Console.WriteLine();
            }*/

            //var f = problemCalculator.GetF(nextI, problemData.Ksi1, problemData.Ksi2, problemData.PartitionsAmount);
            //WriteResults(nextI, f, problemData);
        }

        private static Complex[][] InitI(int n, int m)
        {
            var I = new Complex[2 * n + 1][];
            //var oscillatorAmount = (2 * m + 1) * (2 * n + 1);
            for (var i = 0; i < 2 * n + 1; i++)
            {
                I[i] = Enumerable.Repeat<Complex>(1, 2 * m + 1).ToArray();
            }

            ProblemCalculator.NormI(I);

            return I;
        }

        private static Complex[][] InitDifferentI(int n, int m)
        {
            var I = new Complex[2 * n + 1][];
            //var oscillatorAmount = (2 * m + 1) * (2 * n + 1);
            for (var i = 0; i < 2 * n + 1; i++)
            {
                I[i] = new Complex[2 * m + 1];
                for (int j = 0; j < 2 * m + 1; j++)
                {
                    I[i][j] = i + j;
                }
            }

            ProblemCalculator.NormI(I);

            return I;
        }

        private static void CheckInputValues(Complex[][] I, Complex lambda, ProblemCalculator problemCalculator)
        {
            var functionalValue = problemCalculator.GetFunctionalValue(I, lambda);
            if (functionalValue.Real < 0)
            {
                throw new ArgumentException("Initial values is not valid!");
            }
        }

        private static void CheckInputValues(Complex[][] I, Complex lambda, NewProblemCalculator problemCalculator)
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
                    if (prevI[j][k].Magnitude - nextI[j][k].Magnitude > prec)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private static void WriteToFile(Complex[][] I, double c1, double c2)
        {
            var path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName).FullName;
            using (var writer = new StreamWriter($"{path}\\resultI{c1}_{c2}.txt"))
            {
                writer.WriteLine("[");
                foreach (var arr in I)
                {
                    var values = string.Join(", ", arr.Select(p => p.Real).ToArray());
                    writer.WriteLine($"  [{values}],");
                }
                writer.WriteLine("]");
                writer.Close();
            }
        }

        private static void WriteResults(Complex[][] I, Complex[][] F, ProblemData problemData, DateTime time)
        {
            WriteToFileNew(I, $"{problemData.C1}_{problemData.C2}\\resultI_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{j - problemData.N}, {k - problemData.M}, {value.Magnitude}");

            var ksi1Length = problemData.Ksi1.end - problemData.Ksi1.begin;
            var ksi1Step = ksi1Length / problemData.PartitionsAmount.ksi1;

            var ksi2Length = problemData.Ksi2.end - problemData.Ksi2.begin;
            var ksi2Step = ksi2Length / problemData.PartitionsAmount.ksi2;

            WriteToFileNew(F, $"{problemData.C1}_{problemData.C2}\\resultF_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{problemData.Ksi1.begin + j * ksi1Step}, {problemData.Ksi2.begin + k * ksi2Step}, {value.Magnitude}");
        }

        private static void WriteToFileNew(Complex[][] values, string fileName, Func<int, int, Complex, string> formatFunc)
        {
            var path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName).FullName;
            var file = new FileInfo($"{path}\\results\\{fileName}");
            file.Directory.Create();
            using (var writer = new StreamWriter(file.FullName))
            {
                for (var j = 0; j < values.Length; j++)
                {
                    for (var k = 0; k < values[j].Length; k++)
                    {
                        writer.WriteLine(formatFunc(j, k, values[j][k]));
                    }
                }
                writer.Close();
            }
        }

        private static void WriteIterationToFile(ProblemData problemData, NewProblemCalculator problemCalculator, Complex[][] values, Complex lambda, int iteration, DateTime time)
        {
            var path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName).FullName;
            var file = new FileInfo($"{path}\\results\\{problemData.C1}_{problemData.C2}\\Iterations_UseFunctionToFindLambda_{problemData.UseFunctionToFindLambda}_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt");
            file.Directory.Create();
            using (var writer = new StreamWriter(file.FullName, true))
            {
                writer.WriteLine($"Iteration #{iteration}");
                writer.WriteLine($"Sigma {problemCalculator.GetFunctionValue(values)}");
                writer.WriteLine($"Lambda {lambda}");
                writer.WriteLine($"G {problemCalculator.GetGFuncValue(values)}");
                writer.WriteLine($"Lagrange function {problemCalculator.GetLagrangeFunctionValue(values, lambda)}");
                for (var j = 0; j < values.Length; j++)
                {
                    for (var k = 0; k < values[j].Length; k++)
                    {
                        writer.WriteLine($"{j - problemData.N}, {k - problemData.M}, {values[j][k]} | Magnitude: {values[j][k].Magnitude}  Phase: {values[j][k].Phase}");
                    }
                }
                writer.WriteLine();
                writer.Close();
            }
        }
    }
}
