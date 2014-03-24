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
using SharpDX.MediaFoundation;
using DXBuffer = SharpDX.Direct3D11.Buffer;
using CoreTransform = CipherPark.AngelJacket.Core.Animation.Transform;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Effects;
using CipherPark.AngelJacket.Core.Animation;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Content
{
    public static class ContentImporter
    {
        public static SourceVoice LoadVoice(XAudio2 audioDevice, string resource, bool loop = false)
        {
            SourceVoice sourceVoice = null;
            AudioBuffer ab = new AudioBuffer();
            string resourceFileExt = System.IO.Path.GetExtension(resource).ToLower();
            if (resourceFileExt == ".wav")
            {
                VoiceData vd = LoadVoiceDataFromWav(resource);
                WaveFormat sourceVoiceFormat = WaveFormat.CreateCustomFormat((WaveFormatEncoding)vd.Format.FormatTag, (int)vd.Format.SamplesPerSec, (int)vd.Format.Channels, (int)vd.Format.AvgBytesPerSec, (int)vd.Format.BlockAlign, (int)vd.Format.BitsPerSample);
                sourceVoice = new SourceVoice(audioDevice, sourceVoiceFormat);
                ab.AudioBytes = vd.AudioBytes;
                ab.Flags = BufferFlags.EndOfStream;
                ab.Stream = new DataStream(vd.AudioBytes, true, true);               
                vd.AudioData.CopyTo(ab.Stream);
                if (loop)
                    ab.LoopCount = AudioBuffer.LoopInfinite;
                sourceVoice.SubmitSourceBuffer(ab, null);
            }
            else if (resourceFileExt == ".wma")
            {
                return null;
            }

            return sourceVoice;
        }

        public static XAudio2StreamingManager LoadStreamingVoice(XAudio2 audioDevice, string resource)
        {            
            IntPtr nativePointer = UnsafeNativeMethods.LoadStreamingAudio(resource);
            XAudio2StreamingManager manager = new XAudio2StreamingManager(audioDevice, nativePointer);
            return manager;
        }

        public static SpriteFont LoadFont(Device graphicsDevice, string resource)
        {        
            SpriteFont font = new SpriteFont(graphicsDevice, resource);
            return font;
        }

        public static Texture2D LoadTexture(DeviceContext deviceContext, string fileName)
        {
            IntPtr _nativeTextureResource = UnsafeNativeMethods.CreateTextureFromFile(deviceContext.NativePointer, fileName);
            return new Texture2D(_nativeTextureResource);
        }

        public static Texture2D LoadTextureCube(DeviceContext deviceContext, string[] fileNames)
        {
            if (fileNames.Length != 6)
                throw new InvalidOperationException("Number of specified file names is less or greater than 6");
            Texture2D[] _textures = new Texture2D[6];
            Texture2D[] _stagingTextures = new Texture2D[6];
            DataBox[] _dataBoxes = new DataBox[6];            
            for(int i = 0; i < _textures.Length; i++)                                   
                _textures[i] = LoadTexture(deviceContext, fileNames[i]);
            //NOTE: We have to first copy the textures into a "staging" area because the textures
            //returned by LoadTexture() are all GPU-only.
            Texture2DDescription stagingTextureDesc = _textures[0].Description;
            stagingTextureDesc.BindFlags = BindFlags.None;
            stagingTextureDesc.CpuAccessFlags = CpuAccessFlags.Read;
            stagingTextureDesc.Usage = ResourceUsage.Staging;
            for (int i = 0; i < _textures.Length; i++)
            {
                _stagingTextures[i] = new Texture2D(deviceContext.Device, stagingTextureDesc);
                deviceContext.CopyResource(_textures[i], _stagingTextures[i]);                
            }     
            //NOTE: we create a separate loop so that the DeviceContext.CopyResource() operation in the prior loop
            //can finish as many async operations as possible before we call DeviceContext.MapSubResource() on each
            //respective texture.
            for (int i = 0; i < _textures.Length; i++)
                _dataBoxes[i] = deviceContext.MapSubresource(_stagingTextures[i], 0, MapMode.Read, MapFlags.None);
            Texture2DDescription cubeDescription = _textures[0].Description;
            cubeDescription.ArraySize = 6;
            cubeDescription.OptionFlags = ResourceOptionFlags.TextureCube;
            cubeDescription.Usage = ResourceUsage.Default;
            cubeDescription.CpuAccessFlags = CpuAccessFlags.None;          
            Texture2D cubeTexture = new Texture2D(deviceContext.Device, cubeDescription, _dataBoxes );
            for (int i = 0; i < _textures.Length; i++)
                deviceContext.UnmapSubresource(_stagingTextures[i], 0);
            return cubeTexture;
        }    
      
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="fileName"></param>
        /// <param name="shaderByteCode"></param>
        /// <param name="channels"></param>
        /// <returns></returns>
        [Obsolete]
        public static Model ImportFBX(IGameApp app, string fileName, byte[] shaderByteCode, FBXFileChannels channels = FBXFileChannels.Default)
        {
            BasicModel result = null;
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
                case FBXFileChannels.PositionColor:
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

        public static Model ImportX(IGameApp app, string fileName, byte[] shaderByteCode, XFileChannels channels)
        {
            Model result = null;
            Mesh mesh = null;
            Frame rootFrame = null;
            List<KeyframeAnimationController> animationControllers = null;
            List<SkinOffset> skinOffsets = null;
            XFileTextDocument doc = new XFileTextDocument();     

            //Read X-File Data
            //----------------
            doc.Load(System.IO.File.Open(fileName, FileMode.Open, FileAccess.ReadWrite));

            //Access X-File's first mesh Data and it's frame
            //----------------------------------------------
            ////TODO: Remove hard coding.
            //XFileFrameObject xMeshFrame = ((XFileFrameObject)doc.DataObjects[4]);
            
            XFileFrameObject xMeshFrame = (XFileFrameObject)doc.DataObjects.GetDataObject<XFileFrameObject>(1);
            XFileMeshObject xMesh = xMeshFrame.Meshes[0];            

            //Calculate mesh bounding box from mesh Data
            //------------------------------------------
            BoundingBox boundingBox = BoundingBox.FromPoints(xMesh.Vertices.Select(v => new Vector3(v.X, v.Y, v.Z)).ToArray());            

            //Construct model vertex indices from mesh data
            //----------------------------------------------            
            short[] _indices = xMesh.Faces.SelectMany(e => e.FaceVertexIndices.Select( x => (short)x)).ToArray();
           
            //Extract texture coords from mesh data
            //-------------------------------------
            Vector2[] texCoords = null;
            if((channels & XFileChannels.DeclTextureCoords1) != 0)
                texCoords = xMesh.DeclData.GetTextureCoords();

            //Extract normals.
            //----------------
            Vector3[] normals = null;
            if((channels & XFileChannels.DeclNormals) != 0)
                normals = xMesh.DeclData.GetNormals();

            //Construct model vertices from mesh data
            //---------------------------------------
            //BasicVertexPositionNormalTexture[] _vertices = xMesh.Vertices.Zip(normals, (e, f) => new BasicVertexPositionNormalTexture() { Position = new Vector4(e.X, e.Y, e.Z, 1.0f), Normal = f, Color = Color.Green.ToVector4() }).ToArray();            
            if ((channels & XFileChannels.Skinning) != 0)
            {
                //dynamic _vertices = null;
                //if(normals != null && texCoords != null)
                //{
                BasicSkinnedVertexTexture[] _vertices = xMesh.Vertices.Select((v, i) => new BasicSkinnedVertexTexture()
                {
                    Position = new Vector4(v.X, v.Y, v.Z, 1.0f),
                    Normal = normals[i],
                    TextureCoord = texCoords[i]
                }).ToArray();
                //}

                //Convert skinning information in .x file to skinning information required by a skinning shader.
                //(A skinning shader associates each vertex with 4 bone indices and 4 bone weights - we extract this 
                //information from SkinWeights data object).
                //-------------------------------------------------------------------------------------------------

                List<float>[] weights = new List<float>[_vertices.Length];
                List<int>[] boneIndices = new List<int>[_vertices.Length];
                List<string>[] boneNames = new List<string>[_vertices.Length];
                skinOffsets = new List<SkinOffset>();

                for (int i = 0; i < xMesh.SkinWeightsCollection.Count; i++)
                {                    
                    XFileSkinWeightsObject xSkinWeights = xMesh.SkinWeightsCollection[i];
                    CoreTransform skinOffsetTransform = new CoreTransform(new Matrix(xSkinWeights.MatrixOffset.m));
                    skinOffsets.Add(new SkinOffset() { Name = xSkinWeights.TransformNodeName, Transform = skinOffsetTransform });
                    for (int j = 0; j < xSkinWeights.NWeights; j++)
                    {
                        int k = xSkinWeights.VertexIndices[j];

                        if (boneIndices[k] == null)
                            boneIndices[k] = new List<int>();
                        boneIndices[k].Add(i);

                        if (weights[k] == null)
                            weights[k] = new List<float>();
                        weights[k].Add(xSkinWeights.Weights[j]);
                    }
                }
                for (int i = 0; i < _vertices.Length; i++)
                {
                    float[] weightValues = new float[4] { 0, 0, 0, 0 };
                    int[] boneIndicesValues = new int[4] { 0, 0, 0, 0 };
                    if (weights[i] != null)
                    {
                        float[] _weights = weights[i].ToArray();
                        //TODO: assert that _weights.Length <= weightValues.Length (we assume the .x file will have no more than 4 per vertex).
                        Array.Copy(_weights, weightValues, _weights.Length);
                    }
                    if (boneIndices[i] != null)
                    {
                        int[] _boneIndices = boneIndices[i].ToArray();
                        //TODO: assert that _bondIndices.Length <= boneIndicesValues.Length (we assume the .x file will have no more than 4 per vertex).
                        Array.Copy(_boneIndices, boneIndicesValues, _boneIndices.Length);
                    }
                    _vertices[i].Weights = new Vector4(weightValues);
                    _vertices[i].BoneIndices = new Int4(boneIndicesValues);
                }

                //Construct bone hierarchy (skeleton) from frame data
                //----------------------------------------------------              
                //****************************************************
                //NOTE: It is assumed that
                //there is only one bone at the root and, therefore,
                //one bone-frame at the root level of the X-File
                //****************************************************
                ////TODO: Remove hard coding.
                //XFileFrameObject xRootBoneFrame = ((XFileFrameObject)doc.DataObjects[5]);
                XFileFrameObject xRootBoneFrame = doc.DataObjects.GetDataObject<XFileFrameObject>(2);
                rootFrame = new Frame() { Name = xRootBoneFrame.Name, Transform = new CoreTransform(new Matrix(xRootBoneFrame.FrameTransformMatrix.m)) };
                BuildBoneFrameHierarchy(xRootBoneFrame, rootFrame, skinOffsets);

                if ((channels & XFileChannels.Animation) != 0)
                {
                    //Construct animation data from frame data
                    //----------------------------------------
                    ////TODO: Remove hard coding.           
                    //XFileAnimationSetObject xAnimationSet = (XFileAnimationSetObject)doc.DataObjects[8];
                    XFileAnimationSetObject xAnimationSet = doc.DataObjects.GetDataObject<XFileAnimationSetObject>(1);
                    animationControllers = new List<KeyframeAnimationController>();
                    List<Frame> frameList = rootFrame.FlattenToList();
                    for (int i = 0; i < xAnimationSet.Animations.Count; i++)
                    {
                        XFileAnimationObject xAnimation = xAnimationSet.Animations[i];
                        Frame animationTarget = frameList.Find(f => f.Name == xAnimation.FrameRef);
                        if (animationTarget != null)
                        {
                            Dictionary<long, Matrix> matrixTransformKeys = new Dictionary<long, Matrix>();
                            for (int j = 0; j < xAnimation.Keys.Count; j++)
                            {
                                XFileAnimationKeyObject xAnimationKey = xAnimation.Keys[j];
                                switch (xAnimationKey.KeyType)
                                {
                                    case KeyType.Matrix:
                                        for (int k = 0; k < xAnimationKey.NKeys; k++)
                                        {
                                            matrixTransformKeys.Add(xAnimationKey.TimedFloatKeys[k].Time,
                                                                    new Matrix(xAnimationKey.TimedFloatKeys[k].Values));
                                        }
                                        break;
                                }
                            }
                            TransformAnimation modelAnimation = new TransformAnimation();
                            foreach (long time in matrixTransformKeys.Keys)
                                modelAnimation.SetKeyFrame(new AnimationKeyFrame((ulong)time, new CoreTransform(matrixTransformKeys[time])));
                            animationControllers.Add(new KeyframeAnimationController(modelAnimation, animationTarget));
                        }
                    }
                }

                mesh = ContentBuilder.BuildMesh<BasicSkinnedVertexTexture>(app, shaderByteCode, _vertices, _indices, BasicSkinnedVertexTexture.InputElements, BasicSkinnedVertexTexture.ElementSize, boundingBox);

                //Create and return model
                //-----------------------
                RiggedModel riggedModel = new RiggedModel(app);
                riggedModel.Mesh = mesh;
                riggedModel.SkinOffsets.AddRange(skinOffsets);
                riggedModel.FrameTree = rootFrame;
                riggedModel.AnimationRig.AddRange(animationControllers);
                result = riggedModel;
            }
            
            else
            {
                BasicVertexPositionColor[] _vertices = xMesh.Vertices.Select((v, i) => new BasicVertexPositionColor()
                {
                    Position = new Vector4(v.X, v.Y, v.Z, 1.0f)
                }).ToArray();
                mesh = ContentBuilder.BuildMesh<BasicVertexPositionColor>(app, shaderByteCode, _vertices, _indices, BasicVertexPositionColor.InputElements, BasicVertexPositionColor.ElementSize, boundingBox);
                BasicModel basicModel = new BasicModel(app);
                basicModel.Mesh = mesh;
                result = basicModel;
            }

            return result;
        }

        private static void BuildBoneFrameHierarchy(XFileFrameObject xBoneFrame, Frame boneframe, List<SkinOffset> skinOffsets)
        {
            foreach (XFileFrameObject xChildBoneFrame in xBoneFrame.ChildFrames)
            {
                SkinOffset skinOffset = skinOffsets.Find(b => b.Name == xChildBoneFrame.Name);
                Frame childBoneFrame = new Frame() { Name = xChildBoneFrame.Name, Transform = new CoreTransform(new Matrix(xChildBoneFrame.FrameTransformMatrix.m)) };
                if (skinOffset != null)
                    skinOffset.BoneReference = childBoneFrame;
                boneframe.Children.Add(childBoneFrame);                
                BuildBoneFrameHierarchy(xChildBoneFrame, childBoneFrame, skinOffsets);
            }
        }

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

        private static class UnsafeNativeMethods
        {
            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_LoadVoiceDataFromWav")]
            public static extern int LoadVoiceDataFromWav([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref VoiceDataThunk voiceData);

            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_CreateTextureFromFile")]
            public static extern IntPtr CreateTextureFromFile(IntPtr deviceContext, [MarshalAs(UnmanagedType.LPTStr)] string fileName);

            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_LoadFBX")]
            public static extern IntPtr LoadFBX([MarshalAs(UnmanagedType.LPWStr)] string fileName, ref FBXMeshThunk fbxMesh);
            
            [DllImport("AngelJacketNative.dll", EntryPoint = "ContentImporter_LoadStreamingAudio")]
            public static extern IntPtr LoadStreamingAudio([MarshalAs(UnmanagedType.LPWStr)] string fileName);
        }       
    }
  
    [Flags]
    public enum FBXFileChannels
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

    [Flags]
    public enum XFileChannels
    {
        Mesh =                  0x0001, //NOTE: Mesh is always an assumed channel.
        Skinning =              0x0002,
        Frames =                0x0004,
        Animation =             0x0008,
        MeshTextureCoords1 =    0x0010,
        MeshTextureCoords2 =    0x0020,
        MeshVertexColors =      0x0040,
        DeclTextureCoords1 =    0x0080,
        DeclTextureCoords2 =    0x0100,
        MeshNormals =           0x0200,
        DeclNormals =           0x0400,
        CommonAlinBasic = Mesh,
        CommonAlinRigged = Mesh | Skinning | Frames | Animation,
        CommonAlinRiggedTexture1 = CommonAlinRigged | DeclTextureCoords1,
        CommonAlinRiggedLitTexture1 = CommonAlinRiggedTexture1 | DeclNormals,
        CommonAlinRiggedTexture2 = CommonAlinRiggedTexture1 | DeclTextureCoords2, 
        CommonAlinRiggedLitTexture2 = CommonAlinRiggedTexture2 | DeclNormals,
        CommonAlinComplex = Mesh |Frames | Animation,
        CommonAlinComplexTexture1 = CommonAlinComplex | DeclTextureCoords1,
        CommonAlinComplexTexture2 = CommonAlinComplexTexture1 | DeclTextureCoords2,
        CommonLegacyRigged = Skinning | Frames | Animation,
        CommonLegacyRiggedTexture1 = CommonLegacyRigged | MeshTextureCoords1,
        CommonLegacyRiggedTexture2 = CommonLegacyRiggedTexture1 | MeshTextureCoords2,
        CommonLegacyComplex = Mesh | Frames | Animation,
        CommonLegacyComplexTexture1 = CommonLegacyComplex | MeshTextureCoords1,
        CommonLegacyComplexTexture2 = CommonLegacyComplexTexture1 | MeshTextureCoords2       
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
}