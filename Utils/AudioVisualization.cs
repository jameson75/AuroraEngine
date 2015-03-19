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
    //*********************************************************************************************
    //Two separate examples were used as references to implement this class.
    //1. The SpectrumAnalyzerXAPO C++ class at http://www.getcodesamples.com/src/5D67423E/31FD43A5
    //2. The WPF Suound Visualization Library at http://wpfsvl.codeplex.com/
    //*********************************************************************************************

    public class AudioVisualization : SharpDX.XAPO.AudioProcessorBase<AudioVisualizationParam>
    {
        const int DataLength = 2048;
        const int HalfDataLength = 1024;
        IGameApp _game = null;
        FrequencyDistribution _distributionMethod = FrequencyDistribution.Linear;       
        int _frequencyBucketCount = 0;
        int _linearDistributionBucketSize = 0;
        int[] _logarithmicDistributionBucketSizes = null;
        float[] _fftInput = null;
        int _fftInputBufferPos = 0;
        private readonly object SamplesSyncRoot = new object();

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

            switch (distriubtionMethod)
            {
                case FrequencyDistribution.Linear:
                    av._linearDistributionBucketSize = (HalfDataLength / av._frequencyBucketCount) + (HalfDataLength % av._frequencyBucketCount > 0 ? 1 : 0);
                    break;
                case FrequencyDistribution.Logarithmic:
                    av._logarithmicDistributionBucketSizes = new int[frequencyBucketCount];
                    throw new NotImplementedException();
            }

            return av;
        }

        public float[] GetSpectrum()
        {   
            float[] spectrum = new float[_frequencyBucketCount];   
        
            Complex[] data = new Complex[DataLength];
            lock(SamplesSyncRoot)
            {
                for(int i = 0; i < DataLength; i++)
                    data[i].Re = _fftInput[i];
            }
            Fourier.FFT(data, data.Length, FourierDirection.Forward);                                    
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

        private void AggregateSamples(float[] samplesForChannels)
        {
            float mid = samplesForChannels.Average();
            lock (SamplesSyncRoot)
            {
                _fftInput[_fftInputBufferPos] = mid;
                _fftInputBufferPos++;
                if(_fftInputBufferPos >= _fftInput.Length)
                    _fftInputBufferPos = 0;                
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

    public enum FrequencyDistribution
    {
        Linear,
        Logarithmic
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AudioVisualizationParam
    {

    }
}
