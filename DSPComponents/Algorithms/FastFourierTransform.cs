using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastFourierTransform : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public int InputSamplingFrequency { get; set; }
        public Signal OutputFreqDomainSignal { get; set; }

        public override void Run()
        {
            OutputFreqDomainSignal = new Signal(new List<float>(), false);
            OutputFreqDomainSignal.Frequencies = new List<float>();
            OutputFreqDomainSignal.FrequenciesAmplitudes = new List<float>();
            OutputFreqDomainSignal.FrequenciesPhaseShifts = new List<float>();
            int N = InputTimeDomainSignal.Samples.Count;
            for (int i = 0; i < N; i++)
            {
                Complex sumSig = new Complex(0, 0);
                for (int j = 0; j < N; j++)
                {
                    float e = (float)(-2 * Math.PI * i * j / N);
                    Complex c = new Complex(Math.Cos(e), Math.Sin(e));
                    sumSig += InputTimeDomainSignal.Samples[j] * c;
                }
                OutputFreqDomainSignal.Frequencies.Add(i * InputSamplingFrequency / N);
                OutputFreqDomainSignal.FrequenciesAmplitudes.Add((float)Math.Sqrt(sumSig.Real * sumSig.Real + sumSig.Imaginary * sumSig.Imaginary));
                OutputFreqDomainSignal.FrequenciesPhaseShifts.Add((float)Math.Atan2(sumSig.Imaginary, sumSig.Real));
            }
        }
    }
}
