using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.XAudio2;
using SharpDX.Multimedia;

namespace CipherPark.AngelJacket.Core.Utils.Interop
{
    public static class ContentImporter
    {
        public static VoiceData LoadVoiceDataFromWav(string filePath)
        {
            VoiceDataThunk vdt = new VoiceDataThunk();
            VoiceData voiceData = new VoiceData();
            ContentImporter.UnsafeNativeMethods.LoadVoiceDataFromWav(filePath, ref vdt);
            voiceData.Format = vdt.Format;
            voiceData.AudioBytes = vdt.AudioBytes;
            voiceData.AudioData = new MemoryStream(vdt.MarshalFromAudioData());
            vdt.Dispose();
            return voiceData;
        }

        public static SourceVoice LoadVoice(XAudio2 audioDevice, string resource)
        {
            SourceVoice sourceVoice = null;
            VoiceData vd = LoadVoiceDataFromWav(resource);
            WaveFormat sourceVoiceFormat = WaveFormat.CreateCustomFormat((WaveFormatEncoding)vd.Format.FormatTag, (int)vd.Format.SamplesPerSec, (int)vd.Format.Channels, (int)vd.Format.AvgBytesPerSec, (int)vd.Format.BlockAlign, (int)vd.Format.BitsPerSample);
            sourceVoice = new SourceVoice(audioDevice, sourceVoiceFormat);
            AudioBuffer ab = new AudioBuffer();
            ab.AudioBytes = vd.AudioBytes;
            ab.Flags = BufferFlags.EndOfStream;
            ab.Stream = new DataStream(vd.AudioBytes, true, true);
            vd.AudioData.CopyTo(ab.Stream);
            sourceVoice.SubmitSourceBuffer(ab, null);
            return sourceVoice;
        }

        public static SpriteFont LoadFont(Device graphicsDevice, string resource)
        {            
            //ShaderResourceView fontShaderResourceView = new ShaderResourceView(game.GraphicsDevice, resource);
            SpriteFont font = new SpriteFont(graphicsDevice, resource);
            return font;
        }

        public static Texture2D LoadTexture(DeviceContext deviceContext, string fileName)
        {
            IntPtr _nativeShaderResourceView = UnsafeNativeMethods.CreateTextureFromFile(deviceContext.NativePointer, fileName);
            return new Texture2D(_nativeShaderResourceView);
        }

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_LoadVoiceDataFromWav")]
            public static extern int LoadVoiceDataFromWav([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref VoiceDataThunk voiceData);

            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_CreateTextureFromFile")]
            public static extern IntPtr CreateTextureFromFile(IntPtr deviceContext, [MarshalAs(UnmanagedType.LPTStr)] string fileName);
        }
    }

    public struct VoiceData
    {
        public WaveFormatEx Format;
        public int AudioBytes;
        public Stream AudioData;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct VoiceDataThunk : IDisposable
    {
        public WaveFormatEx Format;
        public int AudioBytes;
        public IntPtr AudioDataPtr;

        public byte[] MarshalFromAudioData()
        {
            if (AudioDataPtr == IntPtr.Zero)
                return null;
            else
            {
                byte[] audioData = new byte[AudioBytes];
                Marshal.Copy(AudioDataPtr, audioData, 0, audioData.Length);
                return audioData;
            }
        }

        public void MarshalToAudioData(byte[] data)
        {
            if (AudioDataPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(AudioDataPtr);
            if (data == null)
                AudioDataPtr = IntPtr.Zero;
            else
            {
                AudioDataPtr = Marshal.AllocHGlobal(data.Length);
                Marshal.Copy(data, 0, AudioDataPtr, data.Length);
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            this.MarshalToAudioData(null);
        }

        #endregion
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct WaveFormatEx
    {
        public UInt16 FormatTag;
        public UInt16 Channels;
        public UInt32 SamplesPerSec;
        public UInt32 AvgBytesPerSec;
        public UInt16 BlockAlign;
        public UInt16 BitsPerSample;
        public UInt16 PaddingSize;
    }
}