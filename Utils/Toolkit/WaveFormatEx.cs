using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX.Multimedia;

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
{
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

    public static class WaveFormatExtension
    {
        public static WaveFormat ConvertToSDXWaveFormat(this WaveFormatEx wfex)
        {
            WaveFormatEncoding tag = (WaveFormatEncoding)wfex.FormatTag;
            return WaveFormat.CreateCustomFormat(tag,
                                                 (int)wfex.SamplesPerSec,
                                                 (int)wfex.Channels,
                                                 (int)wfex.AvgBytesPerSec,
                                                 (int)wfex.BlockAlign,
                                                 (int)wfex.BitsPerSample);
        }
    }
}