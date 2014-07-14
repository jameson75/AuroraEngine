using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public class BlinnPhongEffect2 : ForwardEffect, ISkinEffect
    {
        public const int MaxLights = 8;
        public const int MaxBones = 72;

        private BlinnPhongShader _shader = null;

        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferCommon = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferSkin = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantsBuffer = null;  

        private int VertexConstantBufferCommonSize = 256;
        private int VertexConstantBufferSkinSize = 3712;
        private int PixelConstantBufferSize = 320;

        private Vector3[] _lampDirPosArray = new Vector3[MaxLights];
        private Color[] _lampColorArray = new Color[MaxLights];
        public BlinnPhongLightType[] _lightTypes = new BlinnPhongLightType[8];
        public SamplerState _textureSamplerState = null;
        public SamplerState _alphaSamplerState = null;

        public Matrix WorldInverseTranspose
        {
            get
            { 
                //return Matrix.Invert(Matrix.Transpose(this.World)); //Wrong order.
                return Matrix.Transpose(Matrix.Invert(this.World));
            }
        }

        public Matrix ViewInverse
        {
            get { return Matrix.Invert(this.View); }
        }

        public Matrix WorldViewProjection
        {
            get { return World * View * Projection; }
        }

        public Vector3[] LampDirPosArray { get { return _lampDirPosArray; } }

        public Color[] LampColorArray { get { return _lampColorArray; } }

        public BlinnPhongLightType[] LightTypes { get { return _lightTypes; } }

        public int FirstActiveLightsCount { get; set; }

        public Color AmbientColor { get; set; }

        public float SpecularPower { get; set; }

        public float Eccentricity { get; set; }

        //public bool EnableSkinning { get; set; }

        //public bool EnableAlphaMap { get; set; }

        //public bool EnableVertexColor { get; set; }

        public Matrix[] BoneTransforms { get; set; }

        public ShaderResourceView Texture { get; set; }

        public ShaderResourceView AlphaMap { get; set; }

        public BlinnPhongEffect2(BlinnPhongShader shader)
            : base(shader.GraphicsDevice)
        {            
            FirstActiveLightsCount = 1;
           
            //Set shader...          
            _shader = shader;

            //Create constant buffer... 
            _vertexConstantsBufferCommon = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexConstantBufferCommonSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);   
            _vertexConstantsBufferSkin = new SharpDX.Direct3D11.Buffer(GraphicsDevice, VertexConstantBufferSkinSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);
            _pixelConstantsBuffer = new SharpDX.Direct3D11.Buffer(GraphicsDevice, PixelConstantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);           
            
            //Create sampler states...
            SamplerStateDescription samplerStateDesc = SamplerStateDescription.Default();
            samplerStateDesc.Filter = Filter.MinMagMipLinear;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            samplerStateDesc.AddressV = TextureAddressMode.Clamp;
            samplerStateDesc.AddressU = TextureAddressMode.Clamp;
            _textureSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);
            _alphaSamplerState = new SamplerState(GraphicsDevice, samplerStateDesc);
        }

        public override void Apply()
        {           
            //Setup Constants
            //---------------
            if (_shader.VertexType == BlinnPhongShaderVertexType.Skin)
            {
                WriteVertexConstantsSkin();
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferSkin);
            }
            else
            {
                WriteVertexConstantsCommon();
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferCommon); 
            }  
            WritePixelConstants();
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _pixelConstantsBuffer);
            
            //Setup Vertex Shader.
            //-------------------
            //if (EnableSkinning)
            //{
            //    GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderSkin);                
            //}           
            //else
            //{
            //    if (EnableAlphaMap)
            //        GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNTA);
            //    else if (EnableVertexColor)
            //        GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNC);
            //    else
            //        GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNT);                        
            //}           
            GraphicsDevice.ImmediateContext.VertexShader.Set(_shader.VertexShader);        

            //Setup Pixel Shader.
            //------------------  
            GraphicsDevice.ImmediateContext.PixelShader.Set(_shader.PixelShader);
            
            //Setup Shader Textures
            //---------------------
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _textureSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);            
            
            if (_shader.VertexType == BlinnPhongShaderVertexType.PositionNormalTextureAlpha)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _alphaSamplerState);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, AlphaMap);
            }            
        }

        public override byte[] SelectShaderByteCode()
        {
            return _shader.VertexShaderByteCode;
        }

        private void WriteVertexConstantsSkin()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantsBufferSkin, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexConstantBufferSkinSize);
            int offset = 0;

            //Write data buffer matrices
            //--------------------------
            offset = _SetDataBufferMatrices(dataBuffer);
            
            if (BoneTransforms != null)
            {
                if (BoneTransforms.Length > MaxBones)
                    throw new InvalidOperationException("Max bones exceeded.");
                for (int i = 0; i < BoneTransforms.Length; i++)
                {
                    Matrix boneTranspose = Matrix.Transpose(BoneTransforms[i]);
                    dataBuffer.Set(offset, boneTranspose.Row1);
                    offset += sizeof(float) * 4;
                    dataBuffer.Set(offset, boneTranspose.Row2);
                    offset += sizeof(float) * 4;
                    dataBuffer.Set(offset, boneTranspose.Row3);
                    offset += sizeof(float) * 4;
                }
            }

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantsBufferSkin, 0);
        }
     
        private void WriteVertexConstantsCommon()
        {
            //Open Access to Buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_vertexConstantsBufferCommon, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, VertexConstantBufferCommonSize);            

            //Write matrices to data buffer.
            //-------------------------------
            _SetDataBufferMatrices(dataBuffer);    

            //Close Access to Buffer
            //----------------------
            GraphicsDevice.ImmediateContext.UnmapSubresource(_vertexConstantsBufferCommon, 0);
        }       

        private void WritePixelConstants()
        {
            //Open access to buffer
            //---------------------
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_pixelConstantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, PixelConstantBufferSize);            
            
            //LampColorArray
            //--------------
            int offset = 0;
            for(int i = 0; i < MaxLights; i++)
               dataBuffer.Set(offset + (i * Vector4.SizeInBytes), LampColorArray[i].ToVector4());           
           
            //AmbiColor
            //------------
            offset += sizeof(float) * 4 * MaxLights;
            dataBuffer.Set(offset, AmbientColor.ToVector3());           
            
            //Ks
            //--
            offset += sizeof(float) * 3;
            dataBuffer.Set(offset, SpecularPower);
           
            //Eccentricity
            //-------------
            offset += sizeof(float);
            dataBuffer.Set(offset, Eccentricity);  
          
            //NLights
            //-------
            offset += sizeof(float);
            dataBuffer.Set(offset, (float)FirstActiveLightsCount);            
            
            //LightPosDirArray
            //Close access to buffer
            //----------------------
            offset += sizeof(float) * 3;
            for(int i = 0; i < MaxLights; i++)
                dataBuffer.Set(offset + (i * Vector4.SizeInBytes), new Vector4(LampDirPosArray[i], (float)BlinnPhongLightType.Point));
                     
            //Write EnableAlphaMap
            //--------------------
            offset += Vector4.SizeInBytes * MaxLights;
            dataBuffer.Set(offset, _shader.VertexType == BlinnPhongShaderVertexType.PositionNormalTextureAlpha);
            
            //Write EnableVertexColor
            //-----------------------
            offset += sizeof(Int32); //A bool in HLSL is 32 bytes.
            dataBuffer.Set(offset, _shader.VertexType == BlinnPhongShaderVertexType.PositionNormalColor);
            
            GraphicsDevice.ImmediateContext.UnmapSubresource(_pixelConstantsBuffer, 0);
        }

        private int _SetDataBufferMatrices(DataBuffer dataBuffer)
        {
            int offset = 0;

            //WorldITXf
            //---------
            Matrix worldITXf = this.WorldInverseTranspose;
            worldITXf.Transpose();
            dataBuffer.Set(offset, worldITXf);
            offset += sizeof(float) * 16;
            //WorldXf
            //-------
            Matrix world = this.World;
            world.Transpose();
            dataBuffer.Set(offset, world);
            offset += sizeof(float) * 16;
            //ViewIXf
            //-------
            Matrix viewIXf = this.ViewInverse;
            viewIXf.Transpose();
            dataBuffer.Set(offset, viewIXf);
            offset += sizeof(float) * 16;
            //WvpXf
            //-----
            Matrix wvpXf = this.WorldViewProjection;
            wvpXf.Transpose();
            dataBuffer.Set(offset, wvpXf);
            offset += sizeof(float) * 16;

            return offset;
        }
    }

    public enum BlinnPhongLightType
    {
        Directional = 0,
        Point = 1
    }

    public class BlinnPhongShader
    {
        private BlinnPhongShaderVertexType _vertexType;
        private Device _graphicsDevice = null;

        public BlinnPhongShader(Device graphicsDevice, BlinnPhongShaderVertexType vertexType)
        {
            _graphicsDevice = graphicsDevice;
            _vertexType = vertexType;
            switch (vertexType)
            {
                case BlinnPhongShaderVertexType.Skin:
                    _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-skin-vs.cso", out _vertexShader);
                    break;
                case BlinnPhongShaderVertexType.PositionNormalTexture:
                    _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnt-vs.cso", out _vertexShader);
                    break;
                case BlinnPhongShaderVertexType.PositionNormalTextureAlpha:
                    _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnta-vs.cso", out _vertexShader);
                    break;
                case BlinnPhongShaderVertexType.PositionNormalColor:
                    _vertexShaderByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnc-vs.cso", out _vertexShader);
                    break;
            }
            LoadPixelShader("Content\\Shaders\\blinnphong2-ps.cso", out _pixelShader);
        }

        private byte[] _vertexShaderByteCode = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;

        public BlinnPhongShaderVertexType VertexType { get { return _vertexType; } }

        public Byte[] VertexShaderByteCode { get { return _vertexShaderByteCode; } }

        public PixelShader PixelShader { get { return _pixelShader; } }

        public VertexShader VertexShader { get { return _vertexShader; } }

        public Device GraphicsDevice { get { return _graphicsDevice; } }

        protected byte[] LoadVertexShader(string fileName, out VertexShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new VertexShader(_graphicsDevice, shaderByteCode);
            return shaderByteCode;
        }

        protected byte[] LoadPixelShader(string fileName, out PixelShader shader)
        {
            byte[] shaderByteCode = System.IO.File.ReadAllBytes(fileName);
            shader = new PixelShader(_graphicsDevice, shaderByteCode);
            return shaderByteCode;
        }
    }

    public enum BlinnPhongShaderVertexType
    {
        Skin,
        PositionNormalTexture,
        PositionNormalTextureAlpha,
        PositionNormalColor
    }
}

