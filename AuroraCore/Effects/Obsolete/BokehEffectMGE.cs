using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Content;

namespace CipherPark.AngelJacket.Core.Effects
{
    [Obsolete]
    public class BokehEffect : PostEffect
    {
        private byte[] _vertexShaderByteCode = null;
        private VertexShader _frameVertexShader = null;
        private PixelShader _dofPixelShader = null;
        private PixelShader _smartBlurPixelShader = null;
        private PixelShader _horzBlurPixelShader = null;
        private PixelShader _vertBlurPixelShader = null;
        private Mesh _quad = null;
        private ShaderResourceView _renderShaderResource2 = null;
        private ShaderResourceView _renderShaderResource1 = null;
        private RenderTargetView _renderTargetView1 = null;
        private RenderTargetView _renderTargetView2 = null;
        private RenderTargetView _originalRenderTargetView = null;
        private DepthStencilView _originalDepthStencilView = null;
        private SharpDX.Direct3D11.Buffer _constantsBuffer = null;
        private int _constantBufferSize = 80;

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

        public BokehEffect(IGameApp game)
            : base(game)
        {
            string vsFileName = "Content\\Shaders\\bokeh-knu-x1x2x3x4-vs.cso";
            string psDofFileName = "Content\\Shaders\\bokeh-knu-x1-ps.cso";
            string psSmartBlurFileName = "Content\\Shaders\\bokeh-knu-x2-ps.cso";
            string psHorzBlurFileName = "Content\\Shaders\\bokeh-knu-x3-ps.cso";
            string psVertBlurFileName = "Content\\Shaders\\bokeh-knu-x4-ps.cso";

            _vertexShaderByteCode = System.IO.File.ReadAllBytes(vsFileName);
            _frameVertexShader = new VertexShader(game.GraphicsDevice, _vertexShaderByteCode);
            _dofPixelShader = new PixelShader(game.GraphicsDevice, System.IO.File.ReadAllBytes(psDofFileName));
            _smartBlurPixelShader = new PixelShader(game.GraphicsDevice, System.IO.File.ReadAllBytes(psSmartBlurFileName));
            _horzBlurPixelShader = new PixelShader(game.GraphicsDevice, System.IO.File.ReadAllBytes(psHorzBlurFileName));
            _vertBlurPixelShader = new PixelShader(game.GraphicsDevice, System.IO.File.ReadAllBytes(psVertBlurFileName));
            
            _constantBufferSize = 80;
            _constantsBuffer = new SharpDX.Direct3D11.Buffer(game.GraphicsDevice, _constantBufferSize, ResourceUsage.Dynamic, BindFlags.ConstantBuffer, CpuAccessFlags.Write, ResourceOptionFlags.None, 0);

            _quad = ContentBuilder.BuildBasicViewportQuad(game.GraphicsDevice, _vertexShaderByteCode);

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
            _renderTargetView1 = new RenderTargetView(game.GraphicsDevice, _texture, targetDesc);
            _renderTargetView2 = new RenderTargetView(game.GraphicsDevice, _texture2, targetDesc);

            ShaderResourceViewDescription resourceDesc = new ShaderResourceViewDescription();
            resourceDesc.Format = targetDesc.Format;
            resourceDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
            resourceDesc.Texture2D.MostDetailedMip = 0;
            resourceDesc.Texture2D.MipLevels = 1;
            _renderShaderResource1 = new ShaderResourceView(game.GraphicsDevice, _texture, resourceDesc);
            _renderShaderResource2 = new ShaderResourceView(game.GraphicsDevice, _texture2, resourceDesc);
        }

        public override void Apply()
        {
            _originalRenderTargetView = Game.GraphicsDevice.ImmediateContext.OutputMerger.GetRenderTargets(1, out _originalDepthStencilView)[0];
            ShaderResourceView _lastPassRenderTarget = null;
            RenderTargetView _currentPassRenderTarget = null;
            
            //Set the shader's constants
            //--------------------------
            SetShaderConstants();
            Game.GraphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(0, _constantsBuffer);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(0, _constantsBuffer);

            //Set the resource's used by all pasess
            //-------------------------------------
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(0, InputTexture);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(1, Depth);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(0, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror, AddressW = TextureAddressMode.Mirror }));
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(1, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Clamp, AddressV = TextureAddressMode.Clamp, AddressW = TextureAddressMode.Clamp }));
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetSampler(2, new SamplerState(GraphicsDevice, new SamplerStateDescription { Filter = SharpDX.Direct3D11.Filter.MinMagLinearMipPoint, AddressU = TextureAddressMode.Mirror, AddressV = TextureAddressMode.Mirror, AddressW = TextureAddressMode.Mirror }));
                        
            //Pass 0
            //------
            _currentPassRenderTarget = _renderTargetView1;
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_currentPassRenderTarget);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_dofPixelShader);            
            _quad.Draw(null);

            //Pass 1
            //------    
            //Since we want to use the output from the last pass as input to this pass, we need to flip the roles of the input and output textures.
            _currentPassRenderTarget = _renderTargetView2;
            _lastPassRenderTarget = _renderShaderResource1;
            //Unbind new output from input.
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);
            //NOTE: the next call will have the effect of unbinding the new input from output.    
            Game.GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_currentPassRenderTarget);
            Game.GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _lastPassRenderTarget);
            Game.GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            Game.GraphicsDevice.ImmediateContext.PixelShader.Set(_smartBlurPixelShader);
            _quad.Draw(null);

            //Pass 2
            //------
            //Since we want to use the output from the last pass as input to this pass, we need to flip the roles of the input and output textures.
            _currentPassRenderTarget = _renderTargetView1;
            _lastPassRenderTarget = _renderShaderResource2;           
            //Unbind new output from input.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);
            //NOTE: the next call will have the effect of unbinding the new input from output.    
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_currentPassRenderTarget);                      
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _lastPassRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_horzBlurPixelShader);
            _quad.Draw(null);

            //Pass 3
            //------
            //Since we want to use the output from the last pass as input to this pass, we need to flip the roles of the input and output textures.
            //However, unlike the previous passes, the output texture is the render target cached at the begining of this method.
            _currentPassRenderTarget = _originalRenderTargetView;
            _lastPassRenderTarget = _renderShaderResource1;
            //Unbind new output from input.
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, null);             
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_currentPassRenderTarget);
            GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(2, _lastPassRenderTarget);
            GraphicsDevice.ImmediateContext.VertexShader.Set(_frameVertexShader);
            GraphicsDevice.ImmediateContext.PixelShader.Set(_vertBlurPixelShader);
            _quad.Draw(null);
            
            GraphicsDevice.ImmediateContext.OutputMerger.SetTargets(_originalDepthStencilView, _originalRenderTargetView);            

            //Un-bind resources from pixel shader.
            for (int i = 0; i < 3; i++)
            {
                GraphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, null);
                //GraphicsDevice.ImmediateContext.PixelShader.SetSampler(i, null);
            }
        }    

        private void SetShaderConstants()
        {
            const int HLSL_Component_Boundary_Size = 4;
            DataBox dataBox = GraphicsDevice.ImmediateContext.MapSubresource(_constantsBuffer, 0, MapMode.WriteDiscard, MapFlags.None);            
            DataBuffer dataBuffer = new DataBuffer(dataBox.DataPointer, _constantBufferSize);

            int offset = 0;     

            dataBuffer.Set(offset, RetinaFocus);
            offset += sizeof(float);

            dataBuffer.Set(offset, RelaxedEyeFocus);
            offset += sizeof(float);

            dataBuffer.Set(offset, Accomodation);
            offset += sizeof(float);

            dataBuffer.Set(offset, PupilDiameterRange);
            offset += sizeof(float);

            dataBuffer.Set(offset, BaseBlurRadius);
            offset += sizeof(float);

            dataBuffer.Set(offset, BlurFalloff);
            offset += sizeof(float);

            dataBuffer.Set(offset, MaximumBlurRadius);
            offset += sizeof(float);

            dataBuffer.Set(offset, DistantBlurStartRange);
            offset += sizeof(float);

            dataBuffer.Set(offset, DistantBlurEndRange);
            offset += sizeof(float);

            dataBuffer.Set(offset, DistantBlurPower);
            offset += sizeof(float);

            dataBuffer.Set(offset, WeaponBlurCutOff);
            offset += sizeof(float);

            float nearZ = -this.ProjectionMatrix.M43 / this.ProjectionMatrix.M33;
            dataBuffer.Set(offset, nearZ);
            offset += sizeof(float);

            float farZ = this.ProjectionMatrix.M43 / (1.0f - this.ProjectionMatrix.M33);
            dataBuffer.Set(offset, farZ);
            offset += sizeof(float);

            dataBuffer.Set(offset, UseDistantBlur);
            offset += HLSL_Component_Boundary_Size;

            dataBuffer.Set(offset, NoWeapoBlur);
            offset += HLSL_Component_Boundary_Size;

            offset += HLSL_Component_Boundary_Size; //The next Vector2 data will straddel 16 byte alignment boundary so we need to skip to the begning of the next boundary.

            Vector2 reciprocalScreenRes = new Vector2(1.0f / ScreenWidth, 1.0f / ScreenHeight);
            dataBuffer.Set(offset, reciprocalScreenRes);
            offset += (sizeof(float) * 2);
            
            GraphicsDevice.ImmediateContext.UnmapSubresource(_constantsBuffer, 0);
        }
    }
}
