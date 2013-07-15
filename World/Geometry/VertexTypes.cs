using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using Float4 = SharpDX.Vector4;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.World.Geometry
{
    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionColor
    {
        public Vector4 Position;
        public Vector4 Color;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static BasicVertexPositionColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }

        public BasicVertexPositionColor(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            Color = SharpDX.Color.Transparent.ToVector4();
        }

        public BasicVertexPositionColor(Vector3 position, Vector4 color)
        {
            Position = new Vector4(position, 1.0f);
            Color = color;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionTexture
    {
        public Vector4 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static BasicVertexPositionTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
             };
            _elementSize = 24;
        }
        public BasicVertexPositionTexture(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector2.Zero;
        }
        public BasicVertexPositionTexture(Vector3 position, Vector2 textureCoord)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = textureCoord;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexScreenTexture
    {
        public Vector2 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static BasicVertexScreenTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
             };
            _elementSize = 16;
        }
        
        public BasicVertexScreenTexture(Vector2 position)
        {
            Position = position;
            TextureCoord = Vector2.Zero;
        }
        
        public BasicVertexScreenTexture(Vector2 position, Vector2 textureCoord)
        {
            Position = position;
            TextureCoord = textureCoord;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionNormalTexture
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;        

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static BasicVertexPositionNormalTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0)
             };
            _elementSize = 36;
        }      
        
        public BasicVertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoord)
        {
            Position = new Vector4(position, 1.0f);
            Normal = normal;
            TextureCoord = textureCoord;          
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexPositionNormalColor
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector4 Color;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static BasicVertexPositionNormalColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 28, 0)
             };
            _elementSize = 44;
        }

        public BasicVertexPositionNormalColor(Vector3 position, Vector3 normal, Vector4 color)
        {
            Position = new Vector4(position, 1.0f);
            Normal = normal;
            Color = color;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicSkinnedVertexTexture
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;
        public Int4 BoneIndices;
        public Float4 Weights;
       
        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }        

        static BasicSkinnedVertexTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
                 new InputElement("BLENDINDICES", 0, Format.R32G32B32A32_UInt, 36, 0),
                 new InputElement("BLENDWEIGHT", 0, Format.R32G32B32A32_Float, 52, 0)
             };
            _elementSize = 68;
        }

        public BasicSkinnedVertexTexture(Vector3 position, Vector3 normal, Vector2 textureCoord, float[] weights, int[] boneIndices)
        {
            Position = new Vector4(position, 1.0f);
            Normal = normal;
            TextureCoord = textureCoord;
            Weights = new Float4(weights);
            BoneIndices = new Int4(boneIndices);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BasicVertexDualTextureIPosition
    {        
        public Vector2 TextureCoord0;
        public Vector2 TextureCoord1;

        private static InputElement[] _inputElements = null;
        private static int _vertexElementSize = 0;
        private static int _instanceElementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int VertexElementSize { get { return _vertexElementSize; } }
        public static int InstanceElementSize { get { return _instanceElementSize; } }

        static BasicVertexDualTextureIPosition()
        {
            _inputElements = new InputElement[]
             {                
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 0, 0),
                 new InputElement("TEXCOORD", 1, Format.R32G32_Float, 8, 0),
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1)
             };
            _vertexElementSize = 16;
            _instanceElementSize = 16;
        }

        public BasicVertexDualTextureIPosition(Vector2 textureCoord0, Vector2 textureCoord1)
        {
            TextureCoord0 = textureCoord0;
            TextureCoord1 = textureCoord1;
        }
    }    
}