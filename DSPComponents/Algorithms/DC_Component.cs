using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class DC_Component : Algorithm
    {
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }

        public override void Run()
        {
            List<float> outPut = new List<float>();
            float mean = 0.0f;
            float summation = 0.0f;
            for (int i = 0; i < InputSignal.Samples.Count(); i++)
            {
                summation += InputSignal.Samples[i];
            }
            mean = summation / InputSignal.Samples.Count();
            for (int i = 0; i < InputSignal.Samples.Count(); i++)
            {
                outPut.Add(InputSignal.Samples[i] - mean);
            }

            OutputSignal = new Signal(outPut, false);
        }
    }
}