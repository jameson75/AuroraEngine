using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.Effects
{
    public class BokehEffect : PostEffect
    {
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _frameVertexShader = null;
        private PixelShader _dofPixelShader = null;
        private PixelShader _smartBlurPixelShader = null;
        private PixelShader _horzBlurPixelShader = null;
        private PixelShader _vertBlurPixelShader = null;
        private Mesh _quad = null;
        private ShaderResourceView _passThrougShaderResource = null;
        private ShaderResourceView _auxShaderResource = null;
        private RenderTargetView _passThruRenderTarget = null;
        private RenderTargetView _auxRenderTarget = null;
        private RenderTargetView _originalRenderTargetView = null;
        private DepthStencilView _originalDepthStencilView = null;
        private SharpDX.Direct3D11.Buffer _constantsBuffer = null;

        public float RetinaFocus { get; set; }
        public float RelaxedEyeFocus { get; set; }
        public float Accomodation { get; set; }
        public float PupilDiameterRange { get; set; }
        public float BaseBlurRadius { get; set; }
        public float BlurFalloff { get; set; }
        public float MaximumBlurRadius { get; set; }
        public bool UseDistantBlur { get; set; }
        public float DistantBlurStartRange { get; set; }
        public float DistantBlurEndRange { get; set; }
        public float DistantBlurPower { get; set; }
        public bool NoWeapoBlur { get; set; }
        public float WeaponBlurCutOff { get; set; }
        public Matrix ProjectionMatrix { get; set; }
        public float ScreenWidth { get; set; }
        public float ScreenHeight { get; set; }

        public BokehEffect(Device graphicsDevice, IGameApp game)
            : base(graphicsDevice)
        {
            string vsFileName = "Content\\Shaders\\bokeh-knu-x1x2x3x4-vs.cso";
            string psDofFileName = "Content\\Shaders\\bokeh-knu-x1-ps.cso";
            string psSmartBlurFileName = "Content\\Shaders\\bokeh-knu-x2-ps.cso";
            string psHorzBlurFileName = "Content\\Shaders\\bokeh-knu-x3-ps.cso";
            string psVertBlurFileName = "Content\\Shaders\\bokeh-knu-x4-ps.cso";

            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _frameVertexShader = new VertexShader(GraphicsDevice, _vertexShaderByteCode);
            _dofPixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psDofFileName));
            _smartBlurPixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psSmartBlurFileName));
            _horzBlurPixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psHorzBlurFileName));
            _vertBlurPixelShader = new PixelShader(GraphicsDevice, System.IO.File.ReadAllBytes(psVertBlurFileName));
            int bufferSize = 80;
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(graphicsDevice, bufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            _quad = ContentBuilder.BuildViewportQuad(game, _vertexShaderByteCode);

            Texture2DDescription textureDesc = new Texture2DDescription();
            textureDesc.ArraySize = 1;
            textureDesc.BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource;
            textureDesc.CpuAccessFlags = CpuAccessFlags.None;
            textureDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
            textureDesc.Height = game.RenderTarget.ResourceAs<Texture2D>().Description.Height;
            textureDesc.Width = game.RenderTarget.ResourceAs<Texture2D>().Description.Width;
            textureDesc.MipLevels = 1;
            textureDesc.OptionFlags = ResourceOptionFlags.None;
            textureDesc.Usage = ResourceUsage.Default;
            textureDesc.SampleDescription.Count = 1;
            Texture2D _texture = new Texture2D(game.GraphicsDevice, textureDesc);
            Texture2D _texture2 = new Texture2D(game.GraphicsDevice, textureDesc);

            RenderTargetViewDescription targetDesc = new RenderTargetViewDescription();
            targetDesc.Format = textureDesc.Format;
            targetDesc.Dimension = RenderTargetViewDimension.Texture2D;
            targetDesc.Texture2D.MipSlice = 0;
            _passThruRenderTarget = new RenderTargetView(game.GraphicsDevice, _texture, targetDesc);
            _auxRenderTarget = new RenderTargetView(game.GraphicsDevice, _texture2, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            _auxShaderResource = new ShaderResourceView(game.GraphicsDevice, _texture, resourceDesc);
            _passThrougShaderResource = new ShaderResourceView(game.GraphicsDevice, _texture2, resourceDesc);
        }

        public override void Apply()
        {
            SetShaderConstants();
            GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer);
            GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer);

            _originalRenderTargetView = GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out _originalDepthStencilView)[0];

            //Pass 0
            //------
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, Texture);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, Depth);
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror, AddressW = TextureAddressMode.Mirror }));
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp }));
            GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror, AddressW = TextureAddressMode.Mirror }));
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_passThruRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_dofPixelShader);
            _quad.Draw(0);

            //Pass 1
            //------            
            PostEffectChain.Swap<RenderTargetView>(ref _passThruRenderTarget, ref _auxRenderTarget);
            PostEffectChain.Swap<ShaderResourceView>(ref _passThrougShaderResource, ref _auxShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _passThrougShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_passThruRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_smartBlurPixelShader);
            _quad.Draw(0);

            //Pass 2
            //------
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            PostEffectChain.Swap<RenderTargetView>(ref _passThruRenderTarget, ref _auxRenderTarget);
            PostEffectChain.Swap<ShaderResourceView>(ref _passThrougShaderResource, ref _auxShaderResource);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _passThrougShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_passThruRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_horzBlurPixelShader);
            _quad.Draw(0);

            //Pass 3
            //------
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
            PostEffectChain.Swap<RenderTargetView>(ref _passThruRenderTarget, ref _auxRenderTarget);
            PostEffectChain.Swap<ShaderResourceView>(ref _passThrougShaderResource, ref _auxShaderResource);
            //GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _passThrougShaderResource);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalDepthStencilView, _passThruRenderTarget);
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalRenderTargetView);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_vertBlurPixelShader);
            _quad.Draw(0);

            //Un-bind resources from pixel shader.
            for (int i = 0; i < 3; i++)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, null);
                GraphicsDevice.ImmediateContext.PixelShader.SetSampler(i, null);
            }
        }

        public byte[] SelectShaderByteCode()
        {
            return _vertexShaderByteCode;
        }

        private void SetShaderConstants()
        {
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);

            float retinaFocus = this.RetinaFocus;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref retinaFocus);

            float relaxedEyeFocus = this.RelaxedEyeFocus;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref relaxedEyeFocus);

            float accomodation = this.Accomodation;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref accomodation);

            float pupilDiameterRange = this.PupilDiameterRange;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref pupilDiameterRange);

            float baseBlurRadius = this.BaseBlurRadius;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref baseBlurRadius);

            float blurFalloff = this.BlurFalloff;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref blurFalloff);

            float maxBlurRadius = this.MaximumBlurRadius;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref maxBlurRadius);

            bool useDistantBlur = this.UseDistantBlur;
            dataBox.DataPointer = Utilities.WriteAndPosition<bool>(dataBox.DataPointer, ref useDistantBlur);
            dataBox.DataPointer = Utilities.WriteAndPosition<bool>(dataBox.DataPointer, ref useDistantBlur);

            float distantBlurStartRange = this.DistantBlurStartRange;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref distantBlurStartRange);

            float distantBlurEndRange = this.DistantBlurEndRange;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref distantBlurEndRange);

            float distantBlurPower = this.DistantBlurPower;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref distantBlurPower);

            bool noWeaponBlur = this.NoWeapoBlur;
            dataBox.DataPointer = Utilities.WriteAndPosition<bool>(dataBox.DataPointer, ref noWeaponBlur);
            dataBox.DataPointer = Utilities.WriteAndPosition<bool>(dataBox.DataPointer, ref noWeaponBlur);

            float weaponBlurCutoff = this.WeaponBlurCutOff;
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref weaponBlurCutoff);

            float nearZ = -this.ProjectionMatrix.M43 / this.ProjectionMatrix.M33;
            float farZ = this.ProjectionMatrix.M43 / (1.0f - this.ProjectionMatrix.M33);
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref nearZ);
            dataBox.DataPointer = Utilities.WriteAndPosition<float>(dataBox.DataPointer, ref farZ);

            Vector2 reciprocalScreenRes = new Vector2(1.0f / ScreenWidth, 1.0f / ScreenHeight);
            dataBox.DataPointer = Utilities.WriteAndPosition<Vector2>(dataBox.DataPointer, ref reciprocalScreenRes);

            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }
    }
}
