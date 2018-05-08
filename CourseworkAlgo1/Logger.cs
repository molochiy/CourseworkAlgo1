using System;
using System.IO;
using System.Numerics;
using System.Reflection;
using CourseworkAlgo1.I;

namespace CourseworkAlgo1
{
    public static class Logger
    {
        public static void WriteFIterationToFile(F.ProblemData problemData, Complex[][] values, Complex lambda, int iteration, string fileName)
        {
            var path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName).FullName;
            var file = new FileInfo($"{path}\\results\\F\\{problemData.C1}_{problemData.C2}\\{fileName}");
            file.Directory?.Create();

            using (var writer = new StreamWriter(file.FullName, true))
            {
                writer.WriteLine($"Iteration #{iteration}");
                writer.WriteLine($"Lambda {lambda}");
                for (var i = 0; i < values.Length; i++)
                {
                    for (var j = 0; j < values[i].Length; j++)
                    {
                        writer.WriteLine($"{problemData.Ksi1.GetKsiForPartition(i)}, {problemData.Ksi2.GetKsiForPartition(j)}, {values[i][j].Magnitude}");
                    }
                }
                writer.WriteLine();
                writer.Close();
            }
        }

        public static void WriteIIterationToFile(I.ProblemData problemData, ProblemCalculator problemCalculator, Complex[][] values, Complex lambda, int iteration, string fileName)
        {
            var path = Directory.GetParent(Directory.GetParent(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).FullName).FullName;
            var file = new FileInfo($"{path}\\results\\I\\{problemData.C1}_{problemData.C2}\\{fileName}");
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
                        writer.WriteLine($"{j - problemData.N}, {k - problemData.M}, {values[j][k].Magnitude}"/* | Magnitude: {values[j][k].Magnitude}  Phase: {values[j][k].Phase}"*/);
                    }
                }
                writer.WriteLine();
                writer.Close();
            }
        }

        public static void WriteResultsI(Complex[][] I, Complex[][] F, I.ProblemData problemData, string variant, DateTime time)
        {
            WriteToFile(I, $"I\\{problemData.C1}_{problemData.C2}\\{variant}\\resultI_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{j - problemData.N}, {k - problemData.M}, {value.Magnitude}");

            WriteToFile(F, $"I\\{problemData.C1}_{problemData.C2}\\{variant}\\resultF_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{problemData.Ksi1.GetKsiForPartition(j)}, {problemData.Ksi2.GetKsiForPartition(k)}, {value.Magnitude}");
        }

        public static void WriteResultsF(Complex[][] I, Complex[][] F, F.ProblemData problemData, string variant, DateTime time)
        {
            WriteToFile(I, $"F\\{problemData.C1}_{problemData.C2}\\{variant}\\resultI_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{j - problemData.N}, {k - problemData.M}, {value.Magnitude}");

            WriteToFile(F, $"F\\{problemData.C1}_{problemData.C2}\\{variant}\\resultF_{time:yyyy-MM-dd_hh-mm-ss-fff}.txt", (j, k, value) => $"{problemData.Ksi1.GetKsiForPartition(j)}, {problemData.Ksi2.GetKsiForPartition(k)}, {value.Magnitude}");
        }

        private static void WriteToFile(Complex[][] values, string fileName, Func<int, int, Complex, string> formatFunc)
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
    }
}
