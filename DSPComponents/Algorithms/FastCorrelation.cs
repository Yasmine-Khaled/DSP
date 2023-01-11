using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FastCorrelation : Algorithm
    {
        public Signal InputSignal1 { get; set; }
        public Signal InputSignal2 { get; set; }
        public List<float> OutputNonNormalizedCorrelation { get; set; }
        public List<float> OutputNormalizedCorrelation { get; set; }

        public override void Run()
        {
            //throw new NotImplementedException();
            OutputNonNormalizedCorrelation = new List<float>();
            OutputNormalizedCorrelation = new List<float>();
            Signal signal2 = InputSignal1;
            if (InputSignal2 != null)
                signal2 = InputSignal2;
            float term;
            float x = 0;
            float y = 0;
            for (int j = 0; j < InputSignal1.Samples.Count; j++)
            {
                x += (float)Math.Pow(InputSignal1.Samples[j], 2);
                y += (float)Math.Pow(signal2.Samples[j], 2);
            }
            term = x * y;
            term = (float)Math.Sqrt(term) / InputSignal1.Samples.Count;

            List<double> r1, c1, r2, c2;
            List<Complex> cn1 = new List<Complex>();
            List<Complex> cn2 = new List<Complex>();
            for (int k = 0; k < InputSignal1.Samples.Count; k++)
            {
                r1 = new List<double>();
                c1 = new List<double>();
                r2 = new List<double>();
                c2 = new List<double>();
                for (int n = 0; n < InputSignal1.Samples.Count; n++)
                {
                    r1.Add(InputSignal1.Samples[n] * Math.Cos((double)2 * Math.PI * k * n / InputSignal1.Samples.Count));
                    c1.Add(InputSignal1.Samples[n] * Math.Sin((double)-2 * Math.PI * k * n / InputSignal1.Samples.Count));
                }
                for (int n = 0; n < InputSignal1.Samples.Count; n++)
                {
                    r2.Add(signal2.Samples[n] * Math.Cos((double)2 * Math.PI * k * n / signal2.Samples.Count));
                    c2.Add(signal2.Samples[n] * Math.Sin((double)-2 * Math.PI * k * n / signal2.Samples.Count));
                }
                Complex cc1 = new Complex(r1.Sum(), -1 * c1.Sum());
                Complex cc2 = new Complex(r2.Sum(), c2.Sum());
                cn1.Add(cc1);
                cn2.Add(cc2);
            }

            List<Complex> product = new List<Complex>();
            for (int n = 0; n < InputSignal1.Samples.Count; n++)
                product.Add(Complex.Multiply(cn1[n], cn2[n]));
            Signal s = new Signal(new List<float>(), new bool());
            s.FrequenciesAmplitudes = new List<float>();
            s.FrequenciesPhaseShifts = new List<float>();
            s.Frequencies = new List<float>();
            for (int n = 0; n < InputSignal1.Samples.Count; n++)
            {
                s.FrequenciesAmplitudes.Add((float)product[n].Magnitude);
                s.FrequenciesPhaseShifts.Add((float)product[n].Phase);
                s.Frequencies.Add(0);
            }

            InverseDiscreteFourierTransform d = new InverseDiscreteFourierTransform();
            d.InputFreqDomainSignal = s;
            d.Run();
            for (int n = 0; n < InputSignal1.Samples.Count; n++)
            {
                OutputNonNormalizedCorrelation.Add(d.OutputTimeDomainSignal.Samples[n] / InputSignal1.Samples.Count);
                OutputNormalizedCorrelation.Add((d.OutputTimeDomainSignal.Samples[n] / InputSignal1.Samples.Count) / term);
            }
        }
    }
}