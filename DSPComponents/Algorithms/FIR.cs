using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSPAlgorithms.DataStructures;

namespace DSPAlgorithms.Algorithms
{
    public class FIR : Algorithm
    {
        public Signal InputTimeDomainSignal { get; set; }
        public FILTER_TYPES InputFilterType { get; set; }
        public float InputFS { get; set; }
        public float? InputCutOffFrequency { get; set; }
        public float? InputF1 { get; set; }
        public float? InputF2 { get; set; }
        public float InputStopBandAttenuation { get; set; }
        public float InputTransitionBand { get; set; }
        public Signal OutputHn { get; set; }
        public Signal OutputYn { get; set; }

        public override void Run()
        {
            //OutputHn = new Signal(new List<float>(), new List<int>(), false);
            OutputYn = new Signal(new List<float>(), new List<int>(), false);

            List<float> H = new List<float>();           // list of HD(n)
            List<float> W = new List<float>();           //list of w(n)
            List<float> coff = new List<float>();          // list of cofficients
            List<int> Index = new List<int>();

            int N = 0;
            // transation width :-

            // Rectangular : 
            if (InputStopBandAttenuation <= 21)
                N = (int)(float)(0.9 / (InputTransitionBand / InputFS));

            // Hanning :
            else if (InputStopBandAttenuation <= 44)
                N = (int)(float)(3.1 / (InputTransitionBand / InputFS));

            // Hamming :
            else if (InputStopBandAttenuation <= 53)
                N = (int)(float)(3.3 / (InputTransitionBand / InputFS));

            // Black_Man :
            else if (InputStopBandAttenuation <= 74)
                N = (int)(float)(5.5 / (InputTransitionBand / InputFS));

            // it should be odd -> type one
            if (N % 2 == 0)
                N += 1;

            int Start = -(N / 2); 
            int End = (N / 2);

  
            for (int i = Start; i <= End; i++)
                Index.Add(i);

            // window method :
            // list of w(n) :

            if (InputStopBandAttenuation <= 21)
            {
                int z = Start;
                while (z <= End)
                {
                    W.Add(Rectangular(z, N));
                    z++;
                }
            }
            else if (InputStopBandAttenuation > 21 && InputStopBandAttenuation <= 44)
            {
                int a = Start;
                while (a <= End)
                {
                    W.Add(Hanning(a, N));
                    a++;
                }
            }
            else if (InputStopBandAttenuation > 44 && InputStopBandAttenuation <= 53)
            {
                int b = Start;
                while (b <= End)
                {
                    W.Add(Hamming(b, N));
                    b++;
                }
            }
            else if (InputStopBandAttenuation > 53 && InputStopBandAttenuation <= 74)
            {
                int c = Start;
                while (c <= End)
                {
                    W.Add(Black_Man(c, N));
                    c++;
                }
            }

            // check InputFilterType :
            // list HD(n)

            if (InputFilterType == FILTER_TYPES.LOW)
            {
                // fc'=fc+(f/2)   / sampling frequency ->normalized 
                float fc = ((float)(InputCutOffFrequency + (InputTransitionBand / 2)) / InputFS);
                int j = Start;
                while (j <= End)
                {
                    H.Add(Low_Pass(j, fc, 0));
                    j++;
                }
            }
            else if (InputFilterType == FILTER_TYPES.HIGH)
            {
                float fc = ((float)(InputCutOffFrequency - (InputTransitionBand / 2)) / InputFS);
                int k = Start;
                while (k <= End)
                {
                    H.Add(High_Pass(k, fc, 0));
                    k++;
                }
            }
            else if (InputFilterType == FILTER_TYPES.BAND_PASS)
            {
                float fc1 = ((float)(InputF1 - (InputTransitionBand / 2)) / InputFS);
                float fc2 = ((float)(InputF2 + (InputTransitionBand / 2)) / InputFS);
                int x = Start;
                while (x <= End)
                {
                    H.Add(Band_Pass(x, fc1, fc2));
                    x++;
                }
            }
            else if (InputFilterType == FILTER_TYPES.BAND_STOP)
            {
                float fc1 = ((float)(InputF1 + (InputTransitionBand / 2)) / InputFS);
                float fc2 = ((float)(InputF2 - (InputTransitionBand / 2)) / InputFS);
                int d = Start;
                while (d <= End)
                {
                    H.Add(Band_Stop(d, fc1, fc2));
                    d++;
                }
            }

            // calculate cofficients by multipl -> w(n) * HD(n) -> w*h :

            for (int i = 0; i < N; i++)
                coff.Add(H[i] * W[i]);
            OutputHn = new Signal(coff, Index, false);

            // convolution OutputHn with InputTimeDomainSignal -> OutputYn (signal FIR):

            DirectConvolution conv = new DirectConvolution();
            conv.InputSignal1 = InputTimeDomainSignal;
            conv.InputSignal2 = new Signal(new List<float>(), new List<int>(), false);
            for (int i = 0; i < OutputHn.Samples.Count; i++)
            {
                conv.InputSignal2.Samples.Add(OutputHn.Samples[i]);
                conv.InputSignal2.SamplesIndices.Add(OutputHn.SamplesIndices[i]);
            }
            conv.Run();
            OutputYn = conv.OutputConvolvedSignal;

        }

        // calculate hD(n) :
        private float Low_Pass(int n, float Fc, float zero)
        {
            if (n != 0)
            {
                return (float)(2 * Fc * ((Math.Sin(n * 2 * Math.PI * Fc)) / (n * 2 * Math.PI * Fc)));
            }
            else
            {
                return 2 * Fc;
            }
        }
        private float High_Pass(int n, float Fc, float zero)
        {
            if (n != 0)
            {
                return (float)(-2 * Fc * ((Math.Sin(n * 2 * Math.PI * Fc)) / (n * 2 * Math.PI * Fc)));
            }
            else
            {
                return 1 - (2 * Fc);
            }
        }
        private float Band_Pass(int n, float Fc1, float Fc2)
        {
            if (n != 0)
            {
                float res1 = (float)(2 * Fc1 * ((Math.Sin(n * 2 * Math.PI * Fc1)) / (n * 2 * Math.PI * Fc1)));
                float res2 = (float)(2 * Fc2 * ((Math.Sin(n * 2 * Math.PI * Fc2)) / (n * 2 * Math.PI * Fc2)));
                return res2 - res1;
            }
            else
            {
                return 2 * (Fc2 - Fc1);
            }
        }
        private float Band_Stop(int n, float Fc1, float Fc2)
        {
            if (n != 0)
            {
                float res1 = (float)(2 * Fc1 * ((Math.Sin(n * 2 * Math.PI * Fc1)) / (n * 2 * Math.PI * Fc1)));
                float res2 = (float)(2 * Fc2 * ((Math.Sin(n * 2 * Math.PI * Fc2)) / (n * 2 * Math.PI * Fc2)));
                return res1 - res2;
            }
            else
            {
                return 1 - (2 * (Fc2 - Fc1));
            }
        }

        // calculate window function :
        private float Rectangular(int n, int N)
        {
            return (1);
        }
        private float Hanning(int n, int N)
        {
            return ((float)(0.5 + 0.5 * Math.Cos((float)(2 * Math.PI * n) / N)));
        }
        private float Hamming(int n, int N)
        {
            return ((float)(0.54 + 0.46 * Math.Cos((float)(2 * Math.PI * n) / N)));
        }
        private float Black_Man(int n, int N)
        {
            return ((float)(0.42 + 0.5 * Math.Cos((float)(2 * Math.PI * n) / (N - 1)) + 0.08 * Math.Cos((float)(4 * Math.PI * n) / (N - 1))));
        }
    }
}