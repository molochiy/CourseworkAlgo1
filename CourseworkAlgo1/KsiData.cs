using System;

namespace CourseworkAlgo1
{
    public class KsiData
    {
        public double Begin { get; set; } = -1;
        public double End { get; set; } = 1;
        public double Step { get; set; } = 0.1;
        public int PartitionsAmount => (int)Math.Round((End - Begin) / Step) + 1;

        public int GetParitionForKsi(double ksi)
        {
            return (int)Math.Round((ksi - Begin) / Step);
        }

        public double GetKsiForPartition(int partition)
        {
            return Begin + partition * Step;
        }
    }
}