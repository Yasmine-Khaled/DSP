using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.IO;
namespace DSPAlgorithms.Algorithms
{
    public class DCT : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            List<float> outsig = new List<float>();
            float count = InputSignal.Samples.Count;


            for (int u = 0; u < count; u++)
            {
                float sum = 0;
                for (int k = 0; k < count; k++)
                {
                    sum += InputSignal.Samples[k] * (float)Math.Cos((((2 * k) - 1) * ((2 * u) - 1) * Math.PI) / (4 * count));
                }


                outsig.Add(((float)Math.Sqrt(2 / count) * sum));
            }
            OutputSignal = new Signal(outsig, false);

        }
    }
}