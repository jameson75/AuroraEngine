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

        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferCommon = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferSkin = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantsBuffer = null;  

        private int VertexConstantBufferCommonSize = 256;
        private int VertexConstantBufferSkinSize = 3712;
        private int PixelConstantBufferSize = 320;

        private PixelShader _pixelShader = null;
        private VertexShader _vertexShaderPNT = null;
        private VertexShader _vertexShaderSkin = null;
        private VertexShader _vertexShaderPNTA = null;
        private VertexShader _vertexShaderPNC = null;        

        private byte[] _vertexShaderPNTByteCode = null;
        private byte[] _vertexShaderSkinByteCode = null;
        private byte[] _vertexShaderPNTAByteCode = null;
        private byte[] _vertexShaderPNCByteCode = null;

        private Vector3[] _lampDirPosArray = new Vector3[MaxLights];
        private Color[] _lampColorArray = new Color[MaxLights];
        public BlinnPhongLightType[] _lightTypes = new BlinnPhongLightType[8];
        public SamplerState _textureSamplerState = null;
        public SamplerState _alphaSamplerState = null;

        public Matrix WorldInverseTranspose
        {
            get
            { //return Matrix.Invert(Matrix.Transpose(this.World)); }
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

        public bool EnableSkinning { get; set; }

        public bool EnableAlphaMap { get; set; }

        public bool EnableVertexColor { get; set; }

        public Matrix[] BoneTransforms { get; set; }

        public ShaderResourceView Texture { get; set; }

        public ShaderResourceView AlphaMap { get; set; }

        public BlinnPhongEffect2(Device graphicsDevice)
            : base(graphicsDevice)
        {            
            FirstActiveLightsCount = 1;
           
            //Create shaders...
            _vertexShaderSkinByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-skin-vs.cso", out _vertexShaderSkin);
            _vertexShaderPNTByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnt-vs.cso", out _vertexShaderPNT);
            _vertexShaderPNTAByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnta-vs.cso", out _vertexShaderPNTA);
            _vertexShaderPNCByteCode = LoadVertexShader("Content\\Shaders\\blinnphong2-pnc-vs.cso", out _vertexShaderPNC);           
            LoadPixelShader("Content\\Shaders\\blinnphong2-ps.cso", out _pixelShader);
                  
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
            //Write Constants
            //---------------
            if (EnableSkinning)            
                WriteVertexConstantsSkin();                                            
            else            
                WriteVertexConstantsCommon();
               
             WritePixelConstants();
            
            //Setup Vertex Shader.
            //-------------------
            if (EnableSkinning)
            {
                GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderSkin);
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferSkin);
            }           
            else
            {
                if (EnableAlphaMap)
                    GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNTA);
                else if (EnableVertexColor)
                    GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNC);
                else
                    GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShaderPNT);
                
                GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _vertexConstantsBufferCommon);                
            }

            //Setup Pixel Shader.
            //------------------  
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _pixelConstantsBuffer);

            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _textureSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);            
            
            if (EnableAlphaMap)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _alphaSamplerState);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, AlphaMap);
            }            
        }

        public override byte[] SelectShaderByteCode()
        {
            if (EnableSkinning)
                return _vertexShaderSkinByteCode;
            else if (EnableAlphaMap)
                return _vertexShaderPNTAByteCode;
            else if (EnableVertexColor)
                return _vertexShaderPNCByteCode;
            else
                return _vertexShaderPNTByteCode;
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
            dataBuffer.Set(offset, EnableAlphaMap);
            
            //Write EnableVertexColor
            //-----------------------
            offset += sizeof(Int32); //A bool in HLSL is 32 bytes.
            dataBuffer.Set(offset, EnableVertexColor);
            
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
}

