using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class Normalizer : Algorithm
    {
        public Signal InputSignal { get; set; }
        public float InputMinRange { get; set; }
        public float InputMaxRange { get; set; }
        public Signal OutputNormalizedSignal { get; set; }

        public override void Run()
        {
            float norm = 0;
            List<float> resamples = new List<float>();
            for (int i = 0; i < InputSignal.Samples.Count; i++)
            {
                norm = (InputMaxRange - InputMinRange) * ((InputSignal.Samples[i] - InputSignal.Samples.Min()) / (InputSignal.Samples.Max() - InputSignal.Samples.Min())) + InputMinRange;
                resamples.Add(norm);
            }
            OutputNormalizedSignal = new Signal(resamples, false);
        }
    }
}
