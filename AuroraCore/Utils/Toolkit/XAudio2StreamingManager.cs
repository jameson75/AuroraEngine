using System;
using System.Runtime.InteropServices;
using SharpDX.XAudio2;
using SharpDX.Multimedia;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Utils.Toolkit
{       
    public class XAudio2StreamingManager : IDisposable
    {
        private SourceVoice _sourceVoice = null;
        private IntPtr _nativePointer = IntPtr.Zero;

        #region constructor / finalizer

        public XAudio2StreamingManager(IntPtr nativePointer, XAudio2 audioDevice)
        {
            WaveFormatEx formatEx = new WaveFormatEx();           
            UnsafeNativeMethods.GetWaveFormat(nativePointer, ref formatEx);
            WaveFormat format = formatEx.ConvertToSDXWaveFormat();
            _sourceVoice = new SharpDX.XAudio2.SourceVoice(audioDevice, format, true);
            _sourceVoice.BufferEnd += SourceVoice_BufferEnd;            
            _nativePointer = nativePointer;
            SubmitNextBlock();
            SubmitNextBlock();                       
        }
        
        ~XAudio2StreamingManager()
        {
            Dispose();
        }

        #endregion

        #region properties
        
        public SourceVoice SourceVoice
        {
            get => _sourceVoice;
        }

        public bool LoopAudio
        {
            get => UnsafeNativeMethods.IsLoopAudioEnabled(_nativePointer);
            set => UnsafeNativeMethods.SetLoopAudioEnabled(_nativePointer, value);
        }

        public long CurrentSampleTime
        {
            get => UnsafeNativeMethods.GetSampleTime(_nativePointer);
        }
        
        #endregion   
        
        #region methods

        public void Start()
        {
            _sourceVoice.Start();
        }

        public void Stop()
        {
            _sourceVoice.Stop();
        }

        public void DestroyVoice()
        {
            _sourceVoice.DestroyVoice();
        }

        public void Dispose()
        {
            if (_nativePointer != IntPtr.Zero)
            {
                UnsafeNativeMethods.Dispose(_nativePointer);
                _nativePointer = IntPtr.Zero;
                if (!_sourceVoice.IsDisposed)
                {
                    _sourceVoice.DestroyVoice();
                    _sourceVoice.Dispose();
                }
            }
        }

        private void SubmitNextBlock()
        {
            int blockLength = 0;
            IntPtr blockPtr = UnsafeNativeMethods.GetNextBlock(_nativePointer, ref blockLength);

            if (blockPtr != IntPtr.Zero)
            {
                AudioBuffer buffer = new AudioBuffer();
                buffer.AudioBytes = blockLength;
                buffer.AudioDataPointer = blockPtr;
                buffer.Context = blockPtr;
                _sourceVoice.SubmitSourceBuffer(buffer, null);
            }
        }   

        #endregion       

        #region handlers

        private void SourceVoice_BufferEnd(IntPtr context)
        {           
            UnsafeNativeMethods.DestroyBlock(context);
            bool isEOF = UnsafeNativeMethods.IsAtEndOfStream(_nativePointer);
            if(!isEOF)
            {
                SubmitNextBlock();
            }            
        }       

        #endregion             

        private static class UnsafeNativeMethods
        {
            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_Dispose")]
            public static extern void Dispose(IntPtr nativePointer);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_IsAtEndOfStream")]
            public static extern bool IsAtEndOfStream(IntPtr nativePointer);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_IsLoopAudioEnabled")]
            public static extern bool IsLoopAudioEnabled(IntPtr nativePointer);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_SetLoopAudioEnabled")]
            public static extern void SetLoopAudioEnabled(IntPtr nativePointer, bool enabled);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_GetSampleTime")]
            public static extern long GetSampleTime(IntPtr nativePointer);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_GetNextBlock")]
            public static extern IntPtr GetNextBlock(IntPtr nativePointer, ref int blockLengthRef);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_GetWaveFormat")]
            public static extern void GetWaveFormat(IntPtr nativePointer, ref WaveFormatEx formatRef);

            [DllImport("AuroraNative.dll", EntryPoint = "XAudio2StreamingManager_DestroyBlock")]
            public static extern void DestroyBlock(IntPtr blockPointer);
        }       
    }
}
