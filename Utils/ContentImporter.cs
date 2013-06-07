using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.IO;
using System.Xml.XPath;
using System.Text.RegularExpressions;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Animation;



///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils.Toolkit
{
    public static class ContentImporter
    {
        private const string vertexPositionColorPattern = @"\G\s*(?<x>-?[0-9]+(?:\.[0-9]+)?)\s+(?<y>-?[0-9]+(?:\.[0-9]+)?)\s+(?<z>-?[0-9]+(?:\.[0-9]+)?)\s+(?<c>[0-9]+)\s*(?:,|$)";
        private const string indexPattern = @"\G\s*(?<i>[0-9]+)(?:\s+|$)";


        //public static Model LoadModel(IGameApp game, string filePath)
        //{
        //    XDocument xmlDoc = XDocument.Load(filePath);
        //    var meshInfo = (from element in xmlDoc.Descendants("mesh")
        //                    select new
        //                        {
        //                            IndexCount = element.Attribute("IndexCount") != null ? Convert.ToInt32(element.Attribute("IndexCount").Value) : 0,
        //                            VertexCount = Convert.ToInt32(element.Attribute("VertexCount").Value)
        //                        }).First();

        //    int vertexElementSize = BasicVertexPositionColor.ElementSize;
        //    InputElement[] vertexInputElements = BasicVertexPositionColor.InputElements;
        //    BasicVertexPositionColor[] vertexData = (from Match m in Regex.Matches(xmlDoc.XPathSelectElement("model/mesh/vertices").Value, vertexPositionColorPattern)
        //                                             select new BasicVertexPositionColor()
        //                                             {
        //                                                 Position = new Vector4(Convert.ToSingle(m.Groups["x"].Value),
        //                                                                        Convert.ToSingle(m.Groups["y"].Value),
        //                                                                        Convert.ToSingle(m.Groups["z"].Value),
        //                                                                        1.0f),
        //                                                 Color = Color.Red.ToVector4() /*new Color(Convert.ToInt32(m.Groups["c"].Value)).ToVector4()*/
        //                                             }).ToArray();
        //    BasicEffectEx effect = new BasicEffectEx(game.GraphicsDevice);
        //    effect.World = Matrix.Identity;
        //    effect.EnableVertexColor = true;
        //    byte[] shaderCode = effect.SelectShaderByteCode();

        //    MeshDescription meshDesc = new MeshDescription();
        //    BufferDescription vertexBufferDesc = new BufferDescription();
        //    vertexBufferDesc.BindFlags = BindFlags.VertexBuffer;
        //    vertexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
        //    vertexBufferDesc.SizeInBytes = meshInfo.VertexCount * vertexElementSize;
        //    vertexBufferDesc.OptionFlags = ResourceOptionFlags.None;
        //    vertexBufferDesc.StructureByteStride = 0;
        //    meshDesc.VertexBuffer = DXBuffer.Create<BasicVertexPositionColor>(game.GraphicsDevice, vertexData, vertexBufferDesc);

        //    if (meshInfo.IndexCount > 0)
        //    {
        //        short[] indexData = (from Match m in Regex.Matches(xmlDoc.XPathSelectElement("model/mesh/indices").Value, indexPattern)
        //                             select Convert.ToInt16(m.Groups[0].Value)).ToArray();
        //        BufferDescription indexBufferDesc = new BufferDescription();
        //        indexBufferDesc.BindFlags = BindFlags.IndexBuffer;
        //        indexBufferDesc.CpuAccessFlags = CpuAccessFlags.None;
        //        indexBufferDesc.SizeInBytes = meshInfo.IndexCount * sizeof(short);
        //        indexBufferDesc.OptionFlags = ResourceOptionFlags.None;
        //        indexBufferDesc.StructureByteStride = 0;
        //        meshDesc.IndexBuffer = DXBuffer.Create<short>(game.GraphicsDevice, indexData, indexBufferDesc);
        //    }

        //    meshDesc.VertexCount = meshInfo.VertexCount;
        //    meshDesc.VertexLayout = new InputLayout(game.GraphicsDevice, shaderCode, vertexInputElements);
        //    meshDesc.VertexStride = vertexElementSize;
        //    meshDesc.Topology = PrimitiveTopology.TriangleList;
        //    Vector3[] vertices = (from v in vertexData select new Vector3(v.Position.X, v.Position.Y, v.Position.Z)).ToArray();
        //    meshDesc.BoundingBox = BoundingBox.FromPoints(vertices);
        //    Mesh mesh = new Mesh(game, meshDesc);
        //    Model model = new Model(game);
        //    model.Mesh = mesh;
        //    model.Effect = effect;
        //    return model;
        //}

        private static VoiceData LoadVoiceDataFromWav(string filePath)
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

            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_LoadFBX")]
            public static extern IntPtr LoadFBX([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref FBXMeshThunk fbxMesh);
        }
       
        public static Model ImportFBX(IGameApp app, string fileName, byte[] shaderByteCode, MeshImportChannel channels = MeshImportChannel.Default)
        {
            Model result = null;
            FBXMeshThunk fbxMeshThunk = new FBXMeshThunk();            
            fbxMeshThunk.m = new float[16];
            ContentImporter.UnsafeNativeMethods.LoadFBX(fileName, ref fbxMeshThunk);
            //TODO: Remove need for tempThunk.
            FBXMeshThunk tempThunk = fbxMeshThunk;
            fbxMeshThunk = tempThunk.MarshalChildren()[0];
            Mesh mesh = null;
            XMFLOAT3[] vertices = MarshalHelper.PtrToStructures<XMFLOAT3>(fbxMeshThunk.Vertices, fbxMeshThunk.VertexCount);
            int[] indices = new int[fbxMeshThunk.IndexCount];
            Marshal.Copy(fbxMeshThunk.Indices, indices, 0, fbxMeshThunk.IndexCount);
            //TODO: Remove need for tempThunk
            fbxMeshThunk = tempThunk;
            fbxMeshThunk.Dispose();
            short[] _indices = Array.ConvertAll<int, short>(indices, e => (short)e);
            BoundingBox boundingBox = BoundingBox.FromPoints(vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());
            //NOTE: We only support specific channel combinations. We throw an exception for an unsupported combination.
            switch (channels)
            {
                case MeshImportChannel.PositionColor:
                    BasicVertexPositionColor[] _vertices = vertices.Select(e => new BasicVertexPositionColor() { Position = new Vector4(e.X, e.Y, e.Z, 1.0f), Color = Color.Red.ToVector4() }).ToArray();
                    mesh = ContentBuilder.BuildMesh<BasicVertexPositionColor>(app, shaderByteCode, _vertices, _indices, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
                    result = new BasicModel(app);
                    result.Mesh = mesh;
                    break;
                default:
                    throw new ArgumentException("Specified channel combination is unsupported.", "channels");
            }                        
            return result;
        }

        public static Model ImportX(IGameApp app, string fileName, byte[] shaderByteCode, MeshImportChannel channels = MeshImportChannel.Default)
        {
            SkinnedModel result = null;
            XFileDocument doc = new XFileDocument();          
            
            //Read X-File Data
            //----------------
            doc.Load(System.IO.File.ReadAllText(fileName));            
            
            //Access X-File's first mesh Data and it's frame
            //----------------------------------------------
            //TODO: Remove hard coding.
            XFileFrameObject xMeshFrame = ((XFileFrameObject)doc.DataObjects[4]);
            XFileMeshObject xMesh = xMeshFrame.Meshes[0];            

            //Calculate mesh bounding box from mesh Data
            //------------------------------------------
            BoundingBox boundingBox = BoundingBox.FromPoints(xMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());            

            //Extract texture coords from mesh data
            //-------------------------------------
            float[] texUStream = xMesh.DeclData.GetVertexDataStream<float>(VertexElementUsage.TexCoord, 0);
            float[] texVStream = xMesh.DeclData.GetVertexDataStream<float>(VertexElementUsage.TexCoord, 1);
            Vector2[] texCoords = texUStream.Zip(texVStream, (f, s) => new Vector2(f, s)).ToArray();         

            //Construct model vertices from mesh data
            //---------------------------------------
            BasicSkinnedVertexTexture[] _vertices = xMesh.Vertices.Zip(texCoords, (e,f) => new BasicSkinnedVertexTexture() { Position = new Vector4(e.X, e.Y, e.Z, 1.0f), TextureCoord = f }).ToArray();            

            //Construct model vertex indices from mesh data
            //----------------------------------------------
            short[] _indices = xMesh.Faces.SelectMany(e => e.FaceVertexIndices.Cast<short>()).ToArray();

            //Construct model skinning information from mesh data
            //---------------------------------------------------
            List<float>[] weights = new List<float>[_vertices.Length];
            List<int>[] boneIndices = new List<int>[_vertices.Length];
            List<Bone> boneList = new List<Bone>();
            for (int i = 0; i < xMesh.SkinWeightsCollection.Count; i++)
            {
                XFileSkinWeightsObject xSkinWeights = xMesh.SkinWeightsCollection[i];
                Animation.Transform boneTransform = new Animation.Transform(new Matrix(xSkinWeights.MatrixOffset.m));
                boneList.Add(new Bone() { Name = xSkinWeights.TransformNodeName, Transform = boneTransform });
                for(int j = 0; j < xSkinWeights.NWeights; j++ )
                {
                    int k = xSkinWeights.VertexIndices[j];
                    boneIndices[k].Add(i);
                    weights[k].Add(xSkinWeights.Weights[j]);                    
                }
            }
            for (int i = 0; i < _vertices.Length; i++)
            {               
                float[] weightValues = new float[4] { 0, 0, 0, 0};
                int[] boneIndicesValues = new int[4] { 0, 0, 0, 0 };
                Array.Copy(weights, weightValues, weights.Length);
                Array.Copy(boneIndices, boneIndicesValues, boneIndices.Length);
                _vertices[i].Weights = new Vector4(weightValues);
                _vertices[i].BoneIndices = new Int4(boneIndicesValues);
            }
            
            //Construct animation data from frame data
            //----------------------------------------
            //TODO: Remove hard coding.
            XFileAnimationSetObject xAnimationSet = (XFileAnimationSetObject)doc.DataObjects[6];
            List<TransformAnimationController> modelAnimationControllers = new List<TransformAnimationController>();
            for( int i = 0; i < xAnimationSet.Animations.Count; i++)
            {
                XFileAnimationObject xAnimation = xAnimationSet.Animations[i];
                Bone animationTarget = boneList.Find(b => b.Name == xAnimation.FrameRef);
                if(animationTarget != null)
                {       
                    Dictionary<long, Matrix> matrixTransformKeys = new Dictionary<long,Matrix>();
                    for(int j = 0; j < xAnimation.Keys.Count; j++)
                    {
                        XFileAnimationKeyObject xAnimationKey = xAnimation.Keys[j];                        
                        switch(xAnimationKey.KeyType)
                        {
                            case KeyType.Matrix:
                                for(int k = 0; k < xAnimationKey.NKeys; k++)
                                {
                                    matrixTransformKeys.Add(xAnimationKey.TimedFloatKeys[k].Time, 
                                                            new Matrix(xAnimationKey.TimedFloatKeys[k].Values));
                                }
                                break;
                        }
                    }                    
                    TransformAnimation modelAnimation = new TransformAnimation();
                    foreach(long time in matrixTransformKeys.Keys)                    
                        modelAnimation.SetKeyFrame(new AnimationKeyFrame((ulong)time, new Transform(matrixTransformKeys[time])));

                    modelAnimationControllers.Add(new TransformAnimationController(modelAnimation, animationTarget));
                }
            }

            //****************************************************
            //NOTE: From this point, forward, it is assumed that
            //there is only one bone at the root and, therefore,
            //one bone-frame at the root level of the X-File
            //****************************************************

            //Access bones frame data
            //-----------------------
            //TODO: Remove hard coding.
            XFileFrameObject xRootBoneFrame = ((XFileFrameObject)doc.DataObjects[5]);
           
            //Construct bone hierarchy (skeleton) from  frame data
            //----------------------------------------------------           
            XFileFrameObject currentXBoneFrame = xRootBoneFrame;               
            XFileFrameObject lastProcessedChild = null;
            Stack<XFileFrameObject> xBoneFrameLineage = new Stack<XFileFrameObject>();            
            while (true)
            {
                //********************************************************************************
                //NOTE: This would probably look a lot simpler if this tree traversal were 
                //implemented recursively. However, for some strange reason, I felt like 
                //implementing this iteratively.
                //********************************************************************************

                XFileFrameObject currentXBoneFrameParent = xBoneFrameLineage.Peek();
                if (currentXBoneFrameParent != null)
                {
                    Bone currentBone = boneList.Find(b => b.Name == currentXBoneFrame.Name);
                    Bone currentBoneParent = boneList.Find(b => b.Name == currentXBoneFrameParent.Name);
                    currentBoneParent.Children.Add(currentBone);
                    currentBone.Parent = currentBoneParent;
                }

                int lastProcessedChildIndex = (lastProcessedChild == null) ? -1 : currentXBoneFrame.ChildFrames.IndexOf(lastProcessedChild);
                if (lastProcessedChildIndex < currentXBoneFrame.ChildFrames.Count - 1)
                {
                    xBoneFrameLineage.Push(currentXBoneFrame);
                    currentXBoneFrame = currentXBoneFrame.ChildFrames[lastProcessedChildIndex + 1];
                    lastProcessedChild = null;
                }
                else
                {
                    lastProcessedChild = currentXBoneFrame;
                    currentXBoneFrame = xBoneFrameLineage.Pop();
                }

                if (currentXBoneFrame == xRootBoneFrame)
                    break;
            }
            
            //Create and return model
            //-----------------------
            result = new SkinnedModel(app);
            result.Mesh = ContentBuilder.BuildMesh<BasicSkinnedVertexTexture>(app, shaderByteCode, _vertices, _indices, BasicSkinnedVertexTexture.InputElements, BasicSkinnedVertexTexture.ElementSize, boundingBox);
            result.Bones.AddRange(boneList.Where(b => b.Parent == null));
            result.Animation.AddRange(modelAnimationControllers);            
            return result;
        }
    }
  
    [Flags]
    public enum MeshImportChannel
    {
        Default = Position | Color,
        Position = 0x01,
        Normal = 0x02,
        Color = 0x04,
        Texture0 = 0x08,
        Texture1 = 0x10,
        PositionColor = Position | Color,
        PositionTexture = Position | Texture0,
        PositionNormalColor = Position | Normal | Color,
        PositionNormalTexture = Position | Normal | Texture0,
        PositionMultiTexture = Position | Texture0 | Texture1,
        PositionNormalMultiTexture  = Position | Normal | Texture0 | Texture1
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FBXMeshThunk : IDisposable
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public float[] m;
        public IntPtr Vertices;
        public int VertexCount;
        public IntPtr Indices;
        public int IndexCount;
        public IntPtr Children;
        public int ChildCount;
        public IntPtr AnimationTakes;
        public int TakeCount;

        public void Dispose()
        {
            FBXMeshThunk.DisposeMeshTree(ref this);
        }

        private static void DisposeMeshTree(ref FBXMeshThunk parent)
        {            
            if (parent.Children != IntPtr.Zero)
            {
                FBXMeshThunk[] children = parent.MarshalChildren();
                for (int i = 0; i < children.Length; i++)
                    FBXMeshThunk.DisposeMeshTree(ref children[i]);                
                Marshal.FreeHGlobal(parent.Children);
                parent.Children = IntPtr.Zero;
            }
            
            if (parent.Vertices != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(parent.Vertices);
                parent.Vertices = IntPtr.Zero;
            }
            
            if (parent.Indices != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(parent.Indices);
                parent.Indices = IntPtr.Zero;
            }          
        }

        public FBXMeshThunk[] MarshalChildren()
        {
            if (Children == IntPtr.Zero)
                return null;
            else
            {
                //FBXMeshThunk[] structures = new FBXMeshThunk[ChildCount];
                //int sizeofTypeT = Marshal.SizeOf(typeof(FBXMeshThunk));
                //for (int i = 0; i < ChildCount; i++)
                //{
                //    IntPtr cursor = IntPtr.Add(Children, i * sizeofTypeT);
                //    //structures[i].m = new float[16];
                //    structures[i] = (FBXMeshThunk)Marshal.PtrToStructure(cursor, typeof(FBXMeshThunk));
                //}
                //return structures;
                return MarshalHelper.PtrToStructures<FBXMeshThunk>(Children, ChildCount);
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FBXKeyFrame
    {
        public XVECTOR4 Transform;
        public XVECTOR4 Rotation;
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