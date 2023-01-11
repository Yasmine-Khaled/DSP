using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSPAlgorithms.Algorithms
{
    public class Sampling : Algorithm
    {
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal InputSignal { get; set; }
        public Signal OutputSignal { get; set; }




        public override void Run()
        {
            List<float> Upper_List = new List<float>();
            List<float> Lower_List = new List<float>();
            FIR Fir_Low = new FIR();
        
            Fir_Low.InputFilterType = FILTER_TYPES.LOW;
            Fir_Low.InputFS = 8000;
            Fir_Low.InputStopBandAttenuation = 50;
            Fir_Low.InputTransitionBand = 500;
            Fir_Low.InputCutOffFrequency = 1500;

            // L!=0 and m=0 
            if (M == 0 && L != 0)
            {
                L = L - 1;

                for (int i = 0; i < InputSignal.Samples.Count; i++)
                {
                    // upsampling
                    // add zeros between samples 
                    Upper_List.Add(InputSignal.Samples[i]);
                    if (i == InputSignal.Samples.Count - 1)
                    { break; }
                    for (int j = 0; j < L; j++)
                    {
                        Upper_List.Add(0);
                    }
                }
                // apply low pass filter 
                Fir_Low.InputTimeDomainSignal = new Signal(Upper_List, false);
                Fir_Low.Run();
                OutputSignal = Fir_Low.OutputYn;

            }

            // L=0 ans M!=0 
            else if (L == 0 && M != 0)
            {
                M = M - 1;
                Fir_Low.InputTimeDomainSignal = InputSignal;
                Fir_Low.Run();
                //apply low pass filter
                OutputSignal = Fir_Low.OutputYn;
                int Lower_number = M;
                // downsampling
                for (int i = 0; i < OutputSignal.Samples.Count; i++)
                {
                    if (Lower_number == M)
                    {
                        Lower_List.Add(OutputSignal.Samples[i]);
                        Lower_number = 0;
                    }
                    else
                    {
                        Lower_number++;
                    }
                }
                OutputSignal = new Signal(Lower_List, false);
            }
            // L != 0 and M != 0 -> non intger (fraction)
            else if (L != 0 && M != 0)
            {
                // add zeros between samples 
                L = (L - 1);
                for (int i = 0; i < InputSignal.Samples.Count; i++)
                {
                    Upper_List.Add(InputSignal.Samples[i]);
                    for (int j = 0; j < L; j++)
                    {
                        Upper_List.Add(0);
                    }
                }
                // apply low pass filter
                Fir_Low.InputTimeDomainSignal = new Signal(Upper_List, false);
                Fir_Low.Run();
                OutputSignal = Fir_Low.OutputYn;
                M = (M - 1);
                int Lower_number = M;
                
                for (int i = 0; i < OutputSignal.Samples.Count; i++)
                {
                    if (Lower_number == M)
                    {
                        if (OutputSignal.Samples[i] != 0)
                        {
                            Lower_List.Add(OutputSignal.Samples[i]);
                        }
                        Lower_number = 0;
                    }
                    else
                    {
                        Lower_number++;
                    }
                }
                OutputSignal = new Signal(Lower_List, false);
            }
            else
            {
                // L=0 and M=0
                Console.WriteLine("error");
            }
        }
    }
}