using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace CipherPark.AngelJacket.Core.Utils
{
    public class SourceVoiceEx
    {
        private XAudio2 _device = null;

        public WaveFormat WaveFormat { get; private set; }
        public SourceVoice Voice { get; private set; }
        public AudioBuffer AudioBuffer { get; private set; }
       
        public SourceVoiceEx(XAudio2 audioDevice, WaveFormat waveFormat, AudioBuffer buffer)
        {
            _device = audioDevice;
            AudioBuffer = buffer;
            WaveFormat = waveFormat;
            Voice = new SourceVoice(audioDevice, waveFormat);
            Voice.SubmitSourceBuffer(buffer, null);
        }
        
        public void Play()
        {             
            Voice.Start();                        
        }
        
        public void Stop()
        {
            Voice.Stop();
        }

        public SourceVoiceInstance PlayInstance()
        {
            SourceVoice newVoice = new SourceVoice(_device, WaveFormat);
            newVoice.SubmitSourceBuffer(AudioBuffer, null);
            SourceVoiceInstance instance = new SourceVoiceInstance(newVoice);
            newVoice.Start();
            return instance;                     
        }
    }

    public class SourceVoiceInstance
    {
        private readonly object syncRoot = new object();

        SourceVoice _voice = null;

        public SourceVoiceInstance(SourceVoice voice)
        {
            voice.StreamEnd += voice_StreamEnd;
            _voice = voice;
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                if (_voice != null)
                    _voice.Stop();
            }
        }

        void voice_StreamEnd()
        {
            lock (syncRoot)
            {
                _voice.DestroyVoice();                
                _voice = null;
            }
        }
    }
}
