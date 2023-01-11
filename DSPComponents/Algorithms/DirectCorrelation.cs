using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class DirectCorrelation : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public List<float> OutputNonNormalizedCorrelation { get; set; }
        public List<float> OutputNormalizedCorrelation { get; set; }

        public override void Run()
        {
            List<float> result = new List<float>();
            List<float> ns = new List<float>();
            float n, res, x, y;
            Signal signal = InputSignal1;
            if (InputSignal2 != null)
                signal = InputSignal2;

            for (int i = 0; i < InputSignal1.Samples.Count; i++)
            {
                res = 0;
                x = 0;
                y = 0;
                for (int j = 0; j < InputSignal1.Samples.Count; j++)
                {
                    x += (float)Math.Pow(InputSignal1.Samples[j], 2);
                    y += (float)Math.Pow(signal.Samples[j], 2);
                    if (signal.Periodic)
                        res += InputSignal1.Samples[j] * signal.Samples[(i + j) % signal.Samples.Count];
                    else
                    {
                        if (i + j < InputSignal1.Samples.Count)
                            res += InputSignal1.Samples[j] * signal.Samples[(i + j)];
                    }
                }
                res /= InputSignal1.Samples.Count;
                result.Add(res);
                n = x * y;
                n = (float)Math.Sqrt(n) / InputSignal1.Samples.Count;
                ns.Add(res / n);
            }
            OutputNonNormalizedCorrelation = result;
            OutputNormalizedCorrelation = ns;
        }
    }
}