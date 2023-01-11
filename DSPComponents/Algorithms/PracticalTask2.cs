using DSPAlgorithms.DataStructures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
namespace DSPAlgorithms.Algorithms
{
    public class PracticalTask2 : Algorithm
    {
        public String SignalPath { get; set; }
        public float Fs { get; set; }
        public float miniF { get; set; }
        public float maxF { get; set; }
        public float newFs { get; set; }
        public int L { get; set; } //upsampling factor
        public int M { get; set; } //downsampling factor
        public Signal OutputFreqDomainSignal { get; set; }

        public override void Run()
        {
            //throw new NotImplementedException();
            Signal InputSignal = LoadSignal(SignalPath);

            FIR f = new FIR();
            f.InputStopBandAttenuation = 50;
            f.InputTransitionBand = 500;
            f.InputF1 = miniF;
            f.InputF2 = maxF;
            f.InputFilterType = FILTER_TYPES.BAND_PASS;
            f.InputFS = Fs;
            f.InputTimeDomainSignal = InputSignal;
            f.Run();
            Signal signal = f.OutputYn;
            using (StreamWriter w = new StreamWriter("C:\\Users\\yasmi\\OneDrive\\Desktop\\FIR.ds"))
            {
                w.WriteLine("0");
                w.WriteLine("0");
                w.WriteLine(f.OutputYn.Samples.Count().ToString());
                for (int i = 0; i < f.OutputYn.Samples.Count(); i++)
                {

                    w.WriteLine(f.OutputYn.SamplesIndices[i].ToString() + " " + f.OutputYn.Samples[i].ToString());

                }
            }
            //sampling
            int flag = 0;
            if (newFs >= 2 * maxF)
            {
                f.InputFS = newFs;
                flag = 1;
            }
            if (flag == 1)
            {
                Sampling sampling = new Sampling();
                sampling.L = L;
                sampling.M = M;
                sampling.InputSignal = signal;
                sampling.Run();
                using (StreamWriter w = new StreamWriter("C:\\Users\\yasmi\\OneDrive\\Desktop\\sampling.ds"))
                {
                    w.WriteLine("0");
                    w.WriteLine("0");
                    w.WriteLine(sampling.OutputSignal.Samples.Count().ToString());
                    for (int i = 0; i < sampling.OutputSignal.Samples.Count(); i++)
                    {

                        w.WriteLine(sampling.OutputSignal.SamplesIndices[i].ToString() + " " + sampling.OutputSignal.Samples[i].ToString());

                    }
                }
                signal = sampling.OutputSignal;
            }
            Console.WriteLine("newFs is not valid");
            DC_Component dC_Component = new DC_Component();
            dC_Component.InputSignal = signal;
            dC_Component.Run();
            using (StreamWriter w = new StreamWriter("C:\\Users\\yasmi\\OneDrive\\Desktop\\DC_component.ds"))
            {
                w.WriteLine("0");
                w.WriteLine("0");
                w.WriteLine(dC_Component.OutputSignal.Samples.Count().ToString());
                for (int i = 0; i < dC_Component.OutputSignal.Samples.Count(); i++)
                {

                    w.WriteLine(dC_Component.OutputSignal.SamplesIndices[i].ToString() + " " + dC_Component.OutputSignal.Samples[i].ToString());

                }
            }
            signal = dC_Component.OutputSignal;

            Normalizer normalizer = new Normalizer();
            normalizer.InputSignal = signal;
            normalizer.InputMinRange = -1;
            normalizer.InputMaxRange = 1;
            normalizer.Run();
            using (StreamWriter w = new StreamWriter("C:\\Users\\yasmi\\OneDrive\\Desktop\\Normalize.ds"))
            {
                w.WriteLine("0");
                w.WriteLine("0");
                w.WriteLine(normalizer.OutputNormalizedSignal.Samples.Count().ToString());
                for (int i = 0; i < normalizer.OutputNormalizedSignal.Samples.Count(); i++)
                {

                    w.WriteLine(normalizer.OutputNormalizedSignal.SamplesIndices[i].ToString() + " " + normalizer.OutputNormalizedSignal.Samples[i].ToString());

                }
            }
            signal = normalizer.OutputNormalizedSignal;

            DiscreteFourierTransform dft = new DiscreteFourierTransform();
            dft.InputTimeDomainSignal = signal;
            dft.Run();
            using (StreamWriter w = new StreamWriter("C:\\Users\\yasmi\\OneDrive\\Desktop\\DFTpractical2.ds"))
            {
                w.WriteLine("1");
                w.WriteLine("0");
                w.WriteLine(dft.OutputFreqDomainSignal.Frequencies.Count().ToString());
                for (int i = 0; i < dft.OutputFreqDomainSignal.Frequencies.Count(); i++)
                {

                    w.WriteLine(dft.OutputFreqDomainSignal.Frequencies[i].ToString() + " " + dft.OutputFreqDomainSignal.FrequenciesAmplitudes[i].ToString() + " " + dft.OutputFreqDomainSignal.FrequenciesPhaseShifts[i].ToString());

                }
            }
            OutputFreqDomainSignal = dft.OutputFreqDomainSignal;
        }

        public Signal LoadSignal(string filePath)
        {
            Stream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var sr = new StreamReader(stream);

            var sigType = byte.Parse(sr.ReadLine());
            var isPeriodic = byte.Parse(sr.ReadLine());
            long N1 = long.Parse(sr.ReadLine());

            List<float> SigSamples = new List<float>(unchecked((int)N1));
            List<int> SigIndices = new List<int>(unchecked((int)N1));
            List<float> SigFreq = new List<float>(unchecked((int)N1));
            List<float> SigFreqAmp = new List<float>(unchecked((int)N1));
            List<float> SigPhaseShift = new List<float>(unchecked((int)N1));

            if (sigType == 1)
            {
                SigSamples = null;
                SigIndices = null;
            }

            for (int i = 0; i < N1; i++)
            {
                if (sigType == 0 || sigType == 2)
                {
                    var timeIndex_SampleAmplitude = sr.ReadLine().Split();
                    SigIndices.Add(int.Parse(timeIndex_SampleAmplitude[0]));
                    SigSamples.Add(float.Parse(timeIndex_SampleAmplitude[1]));
                }
                else
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            if (!sr.EndOfStream)
            {
                long N2 = long.Parse(sr.ReadLine());

                for (int i = 0; i < N2; i++)
                {
                    var Freq_Amp_PhaseShift = sr.ReadLine().Split();
                    SigFreq.Add(float.Parse(Freq_Amp_PhaseShift[0]));
                    SigFreqAmp.Add(float.Parse(Freq_Amp_PhaseShift[1]));
                    SigPhaseShift.Add(float.Parse(Freq_Amp_PhaseShift[2]));
                }
            }

            stream.Close();
            return new Signal(SigSamples, SigIndices, isPeriodic == 1, SigFreq, SigFreqAmp, SigPhaseShift);
        }
    }
}
