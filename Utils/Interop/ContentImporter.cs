using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System.Xml.XPath;
using CipherPark.AngelJacket.Core.World;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using System.Text.RegularExpressions;

namespace CipherPark.AngelJacket.Core.Utils.Interop
{
    public static class ContentImporter
    {
        private const string vertexPositionColorPattern = @"^(?:\s*(?<px>(?:[0-9]*.)?[0-9]+)\s*(?<py>(?:[0-9]*.)?[0-9]+)\s*(?<pz>(?:[0-9]*.)?[0-9]+)\s*(?<c>[0-9]+)\s*,?\s*)*$";
        private const string indexPattern = "";
        public static Model LoadModel(IGameApp game, string filePath) 
        {
            XDocument xmlDoc = XDocument.Load(filePath);
            var meshInfo = (from element in xmlDoc.Descendants("mesh")
                            select new 
                                {
                                    IndexCount = element.Attribute("IndexCount") != null ? Convert.ToInt32(element.Attribute("IndexCount").Value) : 0,
                                    VertexCount = Convert.ToInt32(element.Attribute("VertexCount").Value)                                   
                                }).First();           
                       
            string[] vertexStrings = xmlDoc.XPathSelectElement("mesh/vertices").Value.Split(',');
            int vertexElementSize = VertexPositionColor.ElementSize;
            InputElement[] vertexInputElements = VertexPositionColor.InputElements;            ;
            VertexPositionColor[] vertexData = (from Match m in Regex.Matches(xmlDoc.XPathSelectElement("mesh/vertices").Value, vertexPositionColorPattern)
                          select new VertexPositionColor()
                          {
                                Position = new Vector4(Convert.ToSingle(m.Groups["px"].Value),
                                                       Convert.ToSingle(m.Groups["py"].Value),
                                                       Convert.ToSingle(m.Groups["pz"].Value),
                                                       1.0f),
                                Color = new Color(Convert.ToInt32(m.Groups["c"].Value)).ToVector4()                                            
                          }).ToArray();
            BasicEffect effect = new BasicEffect(game.GraphicsDevice);
            effect.SetWorld(Matrix.Identity);
            effect.SetVertexColorEnabled(true);
            byte[] shaderCode = effect.SelectShaderByteCode();        

            MeshDescription meshDesc = new MeshDescription();
            BufferDescription vertexBufferDesc = new BufferDescription();
            vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
            vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
            vertexBufferDesc.SizeInBytes = meshInfo.VertexCount * vertexElementSize;
            vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
            vertexBufferDesc.StructureByteStride = 0; 
            meshDesc.VertexBuffer = DXBuffer.Create<VertexPositionColor>(game.GraphicsDevice, vertexData, vertexBufferDesc);

            if(meshInfo.IndexCount > 0)
            {
                short[] indexData = (from Match m in Regex.Matches(xmlDoc.XPathSelectElement("mesh/indices").Value, indexPattern)
                                     select Convert.ToInt16(m.Groups[0].Value)).ToArray();
                BufferDescription indexBufferDesc = new BufferDescription();
                indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
                indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
                indexBufferDesc.SizeInBytes = meshInfo.IndexCount * sizeof(short);
                indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
                indexBufferDesc.StructureByteStride = 0;
                meshDesc.IndexBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indexData, indexBufferDesc);
            }

            meshDesc.VertexCount = meshInfo.VertexCount;
            meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, shaderCode, vertexInputElements);
            meshDesc.VertexStride = vertexElementSize;
            meshDesc.Topology = PrimitiveTopology.TriangleList;              
            Mesh mesh = new Mesh(game, meshDesc);
            Model model = new Model(game);
            model.Mesh = mesh;
            model.Effect = new BasicGameEffect(effect);

            return model;
        }

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