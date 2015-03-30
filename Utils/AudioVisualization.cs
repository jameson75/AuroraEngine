using System;
using System.Linq;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.XAPO;
using Exocortex.DSP;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    //*********************************************************************************************************************************
    //CREDITS:
    //The following exmaples were used as references to understand how to implement this class.
    //1. The SpectrumAnalyzerXAPO C++ class at http://www.getcodesamples.com/src/5D67423E/31FD43A5
    //2. The WPF Suound Visualization Library at http://wpfsvl.codeplex.com/
    //3. Real-Time Spectrum Analysis by Gerry Beauregard https://gerrybeauregard.wordpress.com/2010/08/06/real-time-spectrum-analysis/
    //***********************************************************************************************************************************

    public class AudioVisualization : SharpDX.XAPO.AudioProcessorBase<AudioVisualizationParam>
    {
        const int DataLength = 2048;
        const int HalfDataLength = 1024;
        IGameApp _game = null;
        FrequencyDistribution _distributionMethod = FrequencyDistribution.Linear;       
        int _frequencyBucketCount = 0;
        int _linearDistributionBucketSize = 0;
        int[] _logarithmicDistributionBucketRanges = null;
        float[] _fftInput = null;
        float[] _window = null;
        int _fftInputBufferWritePos = 0;
        private readonly object InputSyncRoot = new object();

        private AudioVisualization(IGameApp game)
        {
            _game = game;
            RegistrationProperties = new RegistrationProperties() 
            { 
                Clsid = Utilities.GetGuidFromType(typeof(AudioVisualization)), 
                CopyrightInfo = "Cipher Park Copyright © 2010-2013", 
                FriendlyName = "AudioVisualization", 
                MaxInputBufferCount = 1, 
                MaxOutputBufferCount = 1, 
                MinInputBufferCount = 1, 
                MinOutputBufferCount = 1, 
                Flags = PropertyFlags.Default 
            };
        }

        public static AudioVisualization Create(IGameApp game, int frequencyBucketCount, FrequencyDistribution distriubtionMethod)
        {
            AudioVisualization av = new AudioVisualization(game);

            if (frequencyBucketCount <= 0)
                throw new ArgumentException("Frequency count not greater than zero.");                         
            
            av._frequencyBucketCount = frequencyBucketCount;
            av._distributionMethod = distriubtionMethod;
            av._fftInput = new float[DataLength];
            av._window = new float[DataLength];

            int linearBucketLength = (HalfDataLength / av._frequencyBucketCount) + (HalfDataLength % av._frequencyBucketCount > 0 ? 1 : 0);
            for (int i = 0; i < DataLength; i++)
                av._window[i] = (float)WindowFunction.Hann(i, DataLength);

            switch (distriubtionMethod)
            {
                case FrequencyDistribution.Linear:
                    av._linearDistributionBucketSize = linearBucketLength;
                    break;
                case FrequencyDistribution.Logarithmic:
                    av._logarithmicDistributionBucketRanges = new int[frequencyBucketCount];                    
                    throw new NotImplementedException();
            }

            return av;
        }

        public float[] GetSpectrum()
        {   
            float[] spectrum = new float[_frequencyBucketCount];   
        
            //Since we don't want to lock [circular] input buffer up while performing an
            //FFT on it's data, we copy the data into an auxiliary buffer, first.
            //We also take this opportunity to apply a window function to the input buffer, as well.
            Complex[] data = new Complex[DataLength];
            lock(InputSyncRoot)
            {
                int fftInputBufferReadPos = _fftInputBufferWritePos;
                for (int i = 0; i < DataLength; i++)
                {
                    data[i].Re = _fftInput[fftInputBufferReadPos] * _window[i];
                    fftInputBufferReadPos++;
                    if (fftInputBufferReadPos >= _fftInput.Length)
                        fftInputBufferReadPos = 0;
                }
            }

            //Perform [in-place] FFT on the input.
            Fourier.FFT(data, data.Length, FourierDirection.Forward);                                    

            //FFT results are imaginary numbers. We convert these to real numbers and then 
            //return them to caller.
            //NOTE: The results of the FFT are symetrical, so we only concern ourselves the first half of the buffer.
            for (int i = 0; i < HalfDataLength; i++)
            {
                float frequency = ComplexToRealNumber(data[i]);
                int j = GetBucketIndex(i);
                if (spectrum[j] < frequency)
                    spectrum[j] = frequency;
            }       

            return spectrum;
        }         
       
        public override void Process(BufferParameters[] inputProcessParameters, BufferParameters[] outputProcessParameters, bool isEnabled)
        {
            int frameCount = inputProcessParameters[0].ValidFrameCount;
            DataStream input = new DataStream(inputProcessParameters[0].Buffer, frameCount * InputFormatLocked.BlockAlign, true, true);
            switch (inputProcessParameters[0].BufferFlags)
            {
                case SharpDX.XAPO.BufferFlags.Valid:
                    for (int i = 0; i < frameCount; i++)
                    {
                        float left = input.Read<float>();
                        float right = input.Read<float>();
                        AggregateSamples(new float[] { left, right });
                    }
                    break;
            }
            outputProcessParameters[0].ValidFrameCount = inputProcessParameters[0].ValidFrameCount;
            outputProcessParameters[0].BufferFlags = inputProcessParameters[0].BufferFlags;
        }

        /// <summary>
        /// Writes samples to a circular buffer to be processed by FFT
        /// </summary>
        /// <param name="samplesForChannels"></param>
        private void AggregateSamples(float[] samplesForChannels)
        {
            float mid = samplesForChannels.Average();
            lock (InputSyncRoot)
            {
                _fftInput[_fftInputBufferWritePos] = mid;
                _fftInputBufferWritePos++;
                if(_fftInputBufferWritePos >= _fftInput.Length)
                    _fftInputBufferWritePos = 0;                
            }
        }

        private int GetBucketIndex(int i)
        {
            int index = -1;
            switch (_distributionMethod)
            {
                case FrequencyDistribution.Linear:                   
                    index = i / _linearDistributionBucketSize;
                    break;
                case FrequencyDistribution.Logarithmic:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException("Unexpected distribution method encountered.");
            }
            return index;
        }    

        private static float ComplexToRealNumber(Complex data)
        {
            //convert complex number, a + bi, to a real number... real_number = sqrt( a^2 + bi^2 )
            return (float)Math.Sqrt(data.Re * data.Re + data.Im * data.Im);
        }          
    }
    
    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AudioVisualizationParam
    {   }

    /// <summary>
    /// 
    /// </summary>
    public static class WindowFunction
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="frameSize"></param>
        /// <returns></returns>
        public static double Hamming(int n, int frameSize) 
        { 
            return 0.54 - 0.46 * Math.Cos((2 * Math.PI * n) / (frameSize - 1)); 
        } 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="frameSize"></param>
        /// <returns></returns>
        public static double Hann(int n, int frameSize)
        { 
            return 0.5 * (1 - Math.Cos((2 * Math.PI * n) / (frameSize - 1))); 
        }   
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="frameSize"></param>
        /// <returns></returns>
        public static double BlackmannHarris(int n, int frameSize)
        { 
            return 0.35875 - (0.48829 * Math.Cos((2 * Math.PI * n) / (frameSize - 1))) + (0.14128 * Math.Cos((4 * Math.PI * n) / (frameSize - 1))) - (0.01168 * Math.Cos((4 * Math.PI * n) / (frameSize - 1))); 
        } 
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FrequencyDistribution
    {
        Linear,
        Logarithmic
    }    
}
