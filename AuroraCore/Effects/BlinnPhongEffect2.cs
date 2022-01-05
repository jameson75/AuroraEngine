using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.World.Geometry;
using CipherPark.Aurora.Core.Utils;
using CipherPark.Aurora.Core.Animation;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// 
// Copyright © 2010-2013
// Aurora Engine is licensed under 
// MIT License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.Aurora.Core.Effects
{
    public class BlinnPhongEffect2 : SurfaceEffect, ISkinEffect
    {
        public const int MaxLights = 8;
        public const int MaxBones = 72;

        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferCommon = null;
        private SharpDX.Direct3D11.Buffer _vertexConstantsBufferSkin = null;
        private SharpDX.Direct3D11.Buffer _pixelConstantsBuffer = null;
        private byte[] _vertexShaderByteCode = null;
        private PixelShader _pixelShader = null;
        private VertexShader _vertexShader = null;
        private int VertexConstantBufferCommonSize = 256;
        private int VertexConstantBufferSkinSize = 3712;
        private int PixelConstantBufferSize = 320;

        private Vector3[] _lampDirPosArray = new Vector3[MaxLights];
        private Color[] _lampColorArray = new Color[MaxLights];

        private RasterizerState _oldRasterizerState = null;
        private bool _isRestoreRequired = false;

        public BlinnPhongLampType[] _lampTypes = new BlinnPhongLampType[8];
        public SamplerState _textureSamplerState = null;
        public SamplerState _alphaSamplerState = null;

        SurfaceVertexType _surfaceVertexType = SurfaceVertexType.None;

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

        public BlinnPhongLampType[] LightTypes { get { return _lampTypes; } }

        public int FirstActiveLightsCount { get; set; }

        public Color AmbientColor { get; set; }

        public float SpecularPower { get; set; }

        public float Eccentricity { get; set; }       

        public Matrix[] BoneTransforms { get; set; }

        public ShaderResourceView Texture { get; set; }

        public ShaderResourceView AlphaMap { get; set; }

        public Light[] Lighting { get; set; }

        public BlinnPhongEffect2(IGameApp game, SurfaceVertexType svt)
            : base(game)
        {
            FirstActiveLightsCount = 1;

            _surfaceVertexType = svt;

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

            switch (svt)
            {
                case SurfaceVertexType.SkinNormalTexture:
                    _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\blinnphong2-skin-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.PositionNormalTexture:
                    _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\blinnphong2-pnt-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.PositionNormalColor:
                    _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\blinnphong2-pnc-vs.cso", out _vertexShader);
                    break;
                case SurfaceVertexType.InstancePositionNormalColor:
                    _vertexShaderByteCode = LoadVertexShader("Resources\\Shaders\\blinnphong2-p-pnc-vs.cso", out _vertexShader);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported surface vertex type {svt} specified");
            }

            LoadPixelShader("Resources\\Shaders\\blinnphong2-ps.cso", out _pixelShader);
        }       

        public override void Apply()
        {
            //Setup Dynamic Lighting if specified.
            //------------------------------------
            if (Lighting != null)
                SetupLights(Lighting);

            //Setup Constants
            //---------------
            if (_surfaceVertexType == SurfaceVertexType.SkinNormalTexture)
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
            GraphicsDevice.ImmediateContext.VertexShader.Set(_vertexShader);

            //Setup Pixel Shader.
            //------------------  
            GraphicsDevice.ImmediateContext.PixelShader.Set(_pixelShader);

            //Setup Shader Textures
            //---------------------
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, _textureSamplerState);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);
            
            if (AlphaMap != null)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, _alphaSamplerState);
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, AlphaMap);
            }      

            //Configure Rasterizer.
            if (EnableBackFace)
            {
                _oldRasterizerState = GraphicsDevice.ImmediateContext.Rasterizer.State;
                RasterizerStateDescription newRasterizerStateDesc = (_oldRasterizerState != null) ? _oldRasterizerState.Description : RasterizerStateDescription.Default();
                newRasterizerStateDesc.CullMode = CullMode.None;
                GraphicsDevice.ImmediateContext.Rasterizer.State = new RasterizerState(GraphicsDevice, newRasterizerStateDesc);
                _isRestoreRequired = true;
            }
        }

        public override void Restore()
        {
            if (_isRestoreRequired)
            {
                GraphicsDevice.ImmediateContext.Rasterizer.State = _oldRasterizerState;
                _isRestoreRequired = false;
            }
            base.Restore();
        }

        public override byte[] GetVertexShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetupLights(Light[] lights)
        {
            FirstActiveLightsCount = lights.Length;
            for (int i = 0; i < MaxLights; i++)
            {
                if (i < lights.Length)
                {
                    _lampColorArray[i] = lights[i].Diffuse;
                    if (lights[i] is PointLight)
                    {
                        PointLight light = (PointLight)lights[i];
                        _lampDirPosArray[i] = light.WorldTransform().Translation;
                        _lampTypes[i] = BlinnPhongLampType.Point;
                    }
                    else if (lights[i] is DirectionalLight)
                    {
                        DirectionalLight light = (DirectionalLight)lights[i];
                        _lampDirPosArray[i] = light.Direction;
                        _lampTypes[i] = BlinnPhongLampType.Directional;
                    }
                    else
                        throw new ArgumentException("Unexepcted light type");
                }
                else
                {
                    _lampDirPosArray[i] = Vector3.Zero;
                    _lampColorArray[i] = Color.Zero;
                    _lampTypes[i] = BlinnPhongLampType.Default;
                }
            }
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
            for (int i = 0; i < MaxLights; i++)
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
            for (int i = 0; i < MaxLights; i++)
                dataBuffer.Set(offset + (i * Vector4.SizeInBytes), new Vector4(LampDirPosArray[i], (float)BlinnPhongLampType.Point));

            //Write EnableAlphaMap
            //--------------------
            offset += Vector4.SizeInBytes * MaxLights;
            dataBuffer.Set(offset, AlphaMap != null);

            //Write EnableVertexColor
            //-----------------------
            offset += sizeof(Int32); //A bool in HLSL is 32 bytes.
            dataBuffer.Set(offset, _surfaceVertexType == SurfaceVertexType.PositionNormalColor ||
                                   _surfaceVertexType == SurfaceVertexType.InstancePositionNormalColor);

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

        public bool EnableBackFace { get; set; }

        protected override void OnDispose()
        {
            _vertexConstantsBufferCommon?.Dispose();
            _vertexConstantsBufferSkin?.Dispose();
            _pixelConstantsBuffer?.Dispose();
            _textureSamplerState?.Dispose();
            _alphaSamplerState?.Dispose();
            _vertexShader?.Dispose();
            _pixelShader?.Dispose();
            base.OnDispose();
        }
    }

    public enum BlinnPhongLampType
    {
        Directional = 0,
        Point = 1,
        Default = Directional,
    }
}
   

