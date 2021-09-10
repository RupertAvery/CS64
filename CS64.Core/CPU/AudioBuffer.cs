using System;
using BizHawk.Emulation.Common;
using CS64.Core.Audio;
using CS64.Core.Video;

namespace CS64.Core.CPU
{
    public partial class MC6502State
    {
        private BlipBuffer blip = new BlipBuffer(4096);
        private const int blipbuffsize = 4096;
        private const int cpuclockrate = 1789773; // NTSC
        private int old_s;

        public VICII Vic;
        public SID Sid;

        public void GetSamplesSync(out short[] samples, out int nsamp)
        {
            if (Sid == null || blip == null)
            {
                nsamp = 0;
                samples = new short[nsamp * 2];
                return;
            }

            blip.EndFrame(Sid.sampleclock);
            Sid.sampleclock = 0;

            nsamp = blip.SamplesAvailable();
            samples = new short[nsamp * 2];


            blip.ReadSamples(samples, nsamp, false);
            //HighPassFilter(samples, nsamp, 90.0, 0.25);
            //HighPassFilter(samples, nsamp, 440.0, 0.25);
            //LowPassFilter(samples, nsamp, 5000.0, 1.0);

            for (int i = nsamp - 1; i >= 0; i--)
            {
                samples[i * 2] = samples[i];
                samples[i * 2 + 1] = samples[i];
            }
        }

        static void LowPassFilter(short[] sample, int samples, double frequency, double q)
        {
            double O = 2.0 * Math.PI * frequency / 44100.0;
            double C = q / O;
            double L = 1 / q / O;
            for (int c = 0; c < 1; c++)
            {
                double V = 0, I = 0, T;
                for (int s = 0; s < samples; s++)
                {
                    T = (I - V) / C;
                    I += (sample[s] * O - V) / L;
                    V += T;
                    sample[s] = (short)(V / O);
                }
            }
        }

        static void HighPassFilter(short[] sample, int samples, double Frequency, double Q)
        {
            double O = 2.0 * Math.PI * Frequency / 44100;
            double C = Q / O;
            double L = 1 / Q / O;
            for (int c = 0; c < 1; c++)
            {
                double V = 0, I = 0, T;
                for (int s = 0; s < samples; s++)
                {
                    T = sample[s] * O - V;
                    V += (I + T) / C;
                    I += T / L;
                    sample[s] -= (short)(V / O);
                }
            }
        }

        public void DiscardSamples()
        {
            blip.Clear();
            Sid.sampleclock = 0;
        }


    }
}