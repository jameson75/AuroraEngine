using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using SharpDX.MediaFoundation;
using Exocortex.DSP;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class AudioVisualization
    {
        const int DataLength = 2048;
        const int HalfDataLength = 1024;
        IGameApp _game = null;
        FrequencyDistribution _distributionMethod = FrequencyDistribution.Linear;
        SourceVoice _voice = null;
        int _frequencyBucketCount = 0;

        public AudioVisualization(IGameApp game)
        {
            _game = game;
        }

        public void Initialize(SourceVoice voice, int frequencyBucketCount, FrequencyDistribution distriubtionMethod)
        {
            if (frequencyBucketCount <= 0)
                throw new ArgumentException("Frequency count not greater than zero."); 
            
            if (IsInitialized)
                _Uninitialize();                       
            
            _frequencyBucketCount = frequencyBucketCount;
            _distributionMethod = distriubtionMethod;
            _voice = voice;
            _voice.BufferStart += Voice_OnBufferStart;
        }

        public float[] GetVisualizationData()
        {
            if (!IsInitialized)
                throw new InvalidOperationException("Audio visualization not initialized.");                       

            //TODO: populate data with samples.            
           
            Complex[] data = new Complex[DataLength];
            Fourier.FFT(data, data.Length, FourierDirection.Forward);
            
            float[] maxIntensities = new float[_frequencyBucketCount];            
            for (int i = 0; i < HalfDataLength; i++)
            {
                float intensity = ComplexToRealNumber(data[i]);
                int j = GetBucketIndex(i);
                if (maxIntensities[j] < intensity)
                    maxIntensities[j] = intensity;
            }

            return maxIntensities;
        }

        private void _Uninitialize()
        {
            _voice.ProcessingPassStart += Voice_OnBufferStart;
            _voice = null;
        }

        private void Voice_OnBufferStart(IntPtr pBufferContext)
        {
            //TODO: Read buffer to aggreate samples.
        }

        private int GetBucketIndex(int i)
        {
            switch (_distributionMethod)
            {
                case FrequencyDistribution.Linear:
                    break;
                case FrequencyDistribution.Logarithmic:
                    break;
                default:
                    throw new InvalidOperationException("Unexpected distribution method encountered.");
            }
        }

        private bool IsInitialized { get { return _voice != null; } } 

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
}
