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
    public struct VertexPositionColor
    {
        public Vector4 Position;
        public Vector4 Color;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static VertexPositionColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }

        public VertexPositionColor(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            Color = SharpDX.Color.Transparent.ToVector4();
        }

        public VertexPositionColor(Vector3 position, Vector4 color)
        {
            Position = new Vector4(position, 1.0f);
            Color = color;
        }
    }
   
    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexPositionTexture
    {
        public Vector4 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static VertexPositionTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 16, 0)
             };
            _elementSize = 24;
        }
        public VertexPositionTexture(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector2.Zero;
        }
        public VertexPositionTexture(Vector3 position, Vector2 textureCoord)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = textureCoord;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BillboardVertex
    {
        public Vector4 Position;
        public Vector4 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        static BillboardVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32B32A32_Float, 16, 0)
             };
            _elementSize = 32;
        }

        public BillboardVertex(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector4.Zero;
        }

        public BillboardVertex(Vector3 position, Vector2 textureCoords, Vector2 dimensions)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, dimensions.X, dimensions.Y);
        }
    }

    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct BillboardInstanceVertex
    {
        public Vector4 Position;
        public Vector4 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        private static int _instanceSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        public static int InstanceSize { get { return _instanceSize; } }
        static BillboardInstanceVertex()
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

        public BillboardInstanceVertex(Vector3 position)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = Vector4.Zero;
        }

        public BillboardInstanceVertex(Vector3 position, Vector2 textureCoords, Vector2 dimensions)
        {
            Position = new Vector4(position, 1.0f);
            TextureCoord = new Vector4(textureCoords.X, textureCoords.Y, dimensions.X, dimensions.Y);
        }
    }    

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ScreenVertex
    {
        public Vector2 Position;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static ScreenVertex()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32_Float, 0, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 8, 0)
             };
            _elementSize = 16;
        }
        
        public ScreenVertex(Vector2 position)
        {
            Position = position;
            TextureCoord = Vector2.Zero;
        }
        
        public ScreenVertex(Vector2 position, Vector2 textureCoord)
        {
            Position = position;
            TextureCoord = textureCoord;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct VertexPositionNormalTexture
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;        

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        
        static VertexPositionNormalTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0)
             };
            _elementSize = 36;
        }      
        
        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoord)
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
    public struct VertexPositionNormalColor
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector4 Color;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }

        static VertexPositionNormalColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 28, 0)
             };
            _elementSize = 44;
        }

        public VertexPositionNormalColor(Vector3 position, Vector3 normal, Vector4 color)
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
    public struct VertexPositionNormalTextureSkin
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

        static VertexPositionNormalTextureSkin()
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

        public VertexPositionNormalTextureSkin(Vector3 position, Vector3 normal, Vector2 textureCoord, float[] weights, int[] boneIndices)
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
    public struct ParticleVertexPostionNormalTexture
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector2 TextureCoord;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        private static int _instanceSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        public static int InstanceSize { get { return _instanceSize; } }
        static ParticleVertexPostionNormalTexture()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("TEXCOORD", 0, Format.R32G32_Float, 28, 0),
                 new InputElement("MATRIX", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1)               
             };
            _elementSize = 42;
            _instanceSize = 64;
        }      
    }   

    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ParticleVertexPositionNormalColor
    {
        public Vector4 Position;
        public Vector3 Normal;
        public Vector4 Color;

        private static InputElement[] _inputElements = null;
        private static int _elementSize = 0;
        private static int _instanceSize = 0;
        public static InputElement[] InputElements { get { return _inputElements; } }
        public static int ElementSize { get { return _elementSize; } }
        public static int InstanceSize { get { return _instanceSize; } }

        static ParticleVertexPositionNormalColor()
        {
            _inputElements = new InputElement[]
             {
                 new InputElement("SV_POSITION", 0, Format.R32G32B32A32_Float, 0, 0),
                 new InputElement("NORMAL", 0, Format.R32G32B32_Float, 16, 0),
                 new InputElement("COLOR", 0, Format.R32G32B32A32_Float, 28, 0),
                 new InputElement("MATRIX", 0, Format.R32G32B32A32_Float, 0, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 1, Format.R32G32B32A32_Float, 16, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 2, Format.R32G32B32A32_Float, 32, 1, InputClassification.PerInstanceData, 1),
                 new InputElement("MATRIX", 3, Format.R32G32B32A32_Float, 48, 1, InputClassification.PerInstanceData, 1)
             };
            _elementSize = 44;
            _instanceSize = 64;
        }

        public ParticleVertexPositionNormalColor(Vector3 position, Vector3 normal, Color color)
        {
            Position = new Vector4(position, 1.0f);
            Normal = normal;
            Color = color.ToVector4();
        }
    }  
    
    /// <summary>
    /// </summary>
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
    public struct ParticleInstanceVertexData
    {
        public Matrix Matrix;
        public static int SizeInBytes
        {
            //Matrix.SizeInBytes + Vector2.SizeInBytes * 3
            get {return Matrix.SizeInBytes; }
        }
    }
}