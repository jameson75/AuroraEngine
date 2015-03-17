﻿using System;
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
using SharpDX.MediaFoundation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
{       
    public class XAudio2StreamingManager : IDisposable
    {
        private SourceVoice _sourceVoice = null;
        private IntPtr _nativePointer = IntPtr.Zero;

        public XAudio2StreamingManager(XAudio2 audioDevice, IntPtr nativePointer)
        {
            WaveFormatEx formatEx = new WaveFormatEx();           
            UnsafeNativeMethods.GetWaveFormat(nativePointer, ref formatEx);
            WaveFormat format = formatEx.ConvertToWaveFormat();
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

        public SourceVoice SourceVoice { get { return _sourceVoice; } }

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
            UnsafeNativeMethods.Dispose(_nativePointer);
            _nativePointer = IntPtr.Zero;
            _sourceVoice.Dispose();
        }

        private void SourceVoice_BufferEnd(IntPtr context)
        {
            UnsafeNativeMethods.DestroyBlock(context);
            bool isEOF = UnsafeNativeMethods.IsAtEndOfStream(_nativePointer);
            if(!isEOF)
            {
                SubmitNextBlock();
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

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "XAudio2StreamingManager_Dispose")]
            public static extern void Dispose(IntPtr nativePointer);

            [DllImport("AngelJacketNative.dll", EntryPoint = "XAudio2StreamingManager_IsAtEndOfStream")]
            public static extern bool IsAtEndOfStream(IntPtr nativePointer);

            [DllImport("AngelJacketNative.dll", EntryPoint = "XAudio2StreamingManager_GetNextBlock")]
            public static extern IntPtr GetNextBlock(IntPtr nativePointer, ref int blockLengthRef);

            [DllImport("AngelJacketNative.dll", EntryPoint = "XAudio2StreamingManager_GetWaveFormat")]
            public static extern void GetWaveFormat(IntPtr nativePointer, ref WaveFormatEx formatRef);

            [DllImport("AngelJacketNative.dll", EntryPoint = "XAudio2StreamingManager_DestroyBlock")]
            public static extern void DestroyBlock(IntPtr blockPointer);
        }       
    }
}
