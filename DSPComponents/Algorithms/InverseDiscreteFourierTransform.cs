using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class InverseDiscreteFourierTransform : Algorithm
    {
        public Signal InputFreqDomainSignal { get; set; }
        public Signal OutputTimeDomainSignal { get; set; }
        public override void Run()
        {
            //throw new NotImplementedException();

            OutputTimeDomainSignal = new Signal(new List<float>(), new bool());
            List<double> r;
            List<double> c;
            for (int k = 0; k < InputFreqDomainSignal.Frequencies.Count; k++)
            {
                r = new List<double>();
                c = new List<double>();
                Complex c1 = new Complex();
                for (int n = 0; n < InputFreqDomainSignal.Frequencies.Count; n++)
                {
                    r.Add(InputFreqDomainSignal.FrequenciesAmplitudes[n] * Math.Cos((double)InputFreqDomainSignal.FrequenciesPhaseShifts[n]));
                    c.Add(InputFreqDomainSignal.FrequenciesAmplitudes[n] * Math.Sin((double)InputFreqDomainSignal.FrequenciesPhaseShifts[n]));
                    c1 += new Complex(r[n], c[n]) * Complex.Exp(Complex.ImaginaryOne * 2 * n * k * Math.PI / InputFreqDomainSignal.Frequencies.Count);
                }
                OutputTimeDomainSignal.Samples.Add((float)(c1.Real) / InputFreqDomainSignal.Frequencies.Count);
            }
        }
    }
}
