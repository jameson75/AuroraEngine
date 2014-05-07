using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
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
    public struct BillboardVertexPositionTexture
    {
        public Vector4 Position;
        public Vector4 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static BillboardVertexPositionTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }

        public BillboardVertexPositionTexture(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector4.Zero;
        }

        public BillboardVertexPositionTexture(Vector3 position, Vector2 textureCoords, Vector2 dimensions)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, dimensions.X, dimensions.Y);
        }
    }

    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BillboardInstancePositionVertexTexture
    {
        public Vector4 Position;
        public Vector4 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        private static int _instanceSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        public static int InstanceSize { get { return _instanceSize; } }
        static BillboardInstancePositionVertexTexture()
        {           
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0),
                 new InputElement("MATRIX", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1)
             };
            _elementSize = 32;
            _instanceSize = 64;
        }

        public BillboardInstancePositionVertexTexture(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector4.Zero;
        }

        public BillboardInstancePositionVertexTexture(Vector3 position, Vector2 textureCoords, Vector2 dimensions)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, dimensions.X, dimensions.Y);
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
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FormInstanceVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector4 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        private static int _instanceSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        public static int InstanceSize { get { return _instanceSize; } }
        static FormInstanceVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 28, 0),
                 new InputElement("MATRIX", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("TEXCOORD", 1, Format.R32G32B32A32_Float, 64, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("TEXCOORD", 2, Format.R32G32B32A32_Float, 80, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("TEXCOORD", 3, Format.R32G32B32A32_Float, 96, 1, InputClassification.PerInstanceData, 1)
             };
            _elementSize = 44;
            _instanceSize = 112;
        }      
    }

    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct FormVertex
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;
        public Vector2 TransparencyCoord;
        public Vector2 LightMapCoord;
        public Vector2 ColorMapCoord;
       
        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static FormVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
                 new InputElement("TEXCOORD", 1, Format.R32G32_Float, 36, 0),
                 new InputElement("TEXCOORD", 2, Format.R32G32_Float, 44, 0),
                 new InputElement("TEXCOORD", 3, Format.R32G32_Float, 52, 0)
             };
            _elementSize = 60;
        }       
    }
}