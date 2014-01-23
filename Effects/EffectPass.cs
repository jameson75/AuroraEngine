using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.World.Geometry;
using CipherPark.AngelJacket.Core.Content;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Effects
{
    public abstract class EffectPass
    {
        protected List<ShaderResourceView> _vertexShaderResources = new List<ShaderResourceView>();
        protected List<ShaderResourceView> _pixelShaderResources = new List<ShaderResourceView>();
        protected List<SamplerState> _vertexShaderSamplers = new List<SamplerState>();
        protected List<SamplerState> _pixelShaderSamplers = new List<SamplerState>();
        protected List<SharpDX.Direct3D11.Buffer> _vertexShaderConstantBuffers = new List<SharpDX.Direct3D11.Buffer>();
        protected List<SharpDX.Direct3D11.Buffer> _pixelShaderConstantBuffers = new List<SharpDX.Direct3D11.Buffer>();
        protected Device _graphicsDevice = null;

        public string Name { get; set; }       
        public List<SharpDX.Direct3D11.Buffer> VertexShaderConstantBuffers { get { return _vertexShaderConstantBuffers; } }
        public List<SharpDX.Direct3D11.Buffer> PixelShaderConstantBuffers { get { return _pixelShaderConstantBuffers; } } 
        public List<ShaderResourceView> PixelShaderResources { get { return _pixelShaderResources; } }
        public List<ShaderResourceView> VertexShaderResources { get { return _pixelShaderResources; } }
        public List<SamplerState> VertexShaderSamplers { get { return _vertexShaderSamplers; } }
        public List<SamplerState> PixelShaderSamplers { get { return _pixelShaderSamplers; } }
        public RenderTargetView RenderTarget { get; set; }
        public VertexShader VertexShader { get; set; }
        public PixelShader PixelShader { get; set; }       
        public BlendState BlendState { get; set; }
        public RasterizerState RasterizerState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }       

        protected EffectPass(Device graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;         
            
        }

        public abstract void Execute();
    }

    public class PostEffectPass : EffectPass
    {
        public Mesh ScreenQuad { get; set; }

        public PostEffectPass(Device graphicsDevice)
            : base(graphicsDevice)
        { }

        public override void Execute()
        {
            //SETUP VERTEX SHADER
            //-------------------
            //Shader...
            _graphicsDevice.ImmediateContext.VertexShader.Set(VertexShader);
            //Constant Buffer...
            for (int i = 0; i < _vertexShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(i, _vertexShaderConstantBuffers[i]);
            //Resources (Textures)...
            for (int i = 0; i < _vertexShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetShaderResource(i, _vertexShaderResources[i]);
            //Samplers/SamplerStates...
            for (int i = 0; i < _vertexShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetSampler(i, _vertexShaderSamplers[i]);

            //SETUP PIXEL SHADER
            //-------------------
            //Shader...
            _graphicsDevice.ImmediateContext.PixelShader.Set(PixelShader);
            //Constant Buffer...
            for (int i = 0; i < _pixelShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(i, _pixelShaderConstantBuffers[i]);
            //Resources (Textures)...
            for (int i = 0; i < _pixelShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, _pixelShaderResources[i]);
            //Samplers/SamplerStates...
            for (int i = 0; i < _pixelShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetSampler(i, _pixelShaderSamplers[i]);

            //SETUP STATES
            //------------
            BlendState _oldBlendState = null;
            RasterizerState _oldRasterizerState = null;
            DepthStencilState _oldDepthStencilState = null;
            //Blend State
            if (BlendState != null)
            {
                _oldBlendState = _graphicsDevice.ImmediateContext.OutputMerger.BlendState;
                _graphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }
            //Rasterizer State
            if (RasterizerState != null)
            {
                _oldRasterizerState = _graphicsDevice.ImmediateContext.Rasterizer.State;
                _graphicsDevice.ImmediateContext.Rasterizer.State = RasterizerState;
            }
            //DepthStencilState
            if (DepthStencilState != null)
            {
                _oldDepthStencilState = _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
                _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = DepthStencilState;
            }

            //SETUP RENDER TARGET
            //-------------------
            _graphicsDevice.ImmediateContext.OutputMerger.SetTargets(RenderTarget);

            //EXECUTE SHADERS
            //---------------
            ScreenQuad.Draw(0);       

            //CLEAN UP VERTEX SHADER
            //----------------------
            //Shader...
            _graphicsDevice.ImmediateContext.VertexShader.Set(null);
            //Constant Buffer...
            for (int i = 0; i < _vertexShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(i, null);
            //Resources (Textures)...
            for (int i = 0; i < _vertexShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetShaderResource(i, null);
            //Samplers/SamplerStates...
            for (int i = 0; i < _vertexShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetSampler(i, null);

            //CLEAN UP PIXEL SHADER
            //---------------------
            //Shader...
            _graphicsDevice.ImmediateContext.PixelShader.Set(null);
            //Constant Buffer...
            for (int i = 0; i < _pixelShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(i, null);
            //Resources (Textures)...
            for (int i = 0; i < _pixelShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, null);
            //Samplers/SamplerStates...
            for (int i = 0; i < _pixelShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetSampler(i, null);

            //CLEAN UP STATES
            //---------------   
            //Blend State
            if (BlendState != null)
                _graphicsDevice.ImmediateContext.OutputMerger.BlendState = _oldBlendState;            
            //Rasterizer State
            if (RasterizerState != null)                          
                _graphicsDevice.ImmediateContext.Rasterizer.State = _oldRasterizerState;            
            //DepthStencilState
            if (DepthStencilState != null)
                _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = _oldDepthStencilState;

            //CLEAN UP RENDER TARGET
            //-----------------------
            _graphicsDevice.ImmediateContext.OutputMerger.SetTargets((RenderTargetView)null);
        }
    }

    public class ForwardEffectPass : EffectPass
    {
        BlendState _oldBlendState = null;
        RasterizerState _oldRasterizerState = null;
        DepthStencilState _oldDepthStencilState = null;

        public ForwardEffectPass(Device graphicsDevice)
            : base(graphicsDevice)
        { }

        public override void Execute()
        {
            //SETUP VERTEX SHADER
            //-------------------
            //Shader...
            _graphicsDevice.ImmediateContext.VertexShader.Set(VertexShader);
            //Constant Buffer...
            for (int i = 0; i < _vertexShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(i, _vertexShaderConstantBuffers[i]);
            //Resources (Textures)...
            for (int i = 0; i < _vertexShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetShaderResource(i, _vertexShaderResources[i]);
            //Samplers/SamplerStates...
            for (int i = 0; i < _vertexShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetSampler(i, _vertexShaderSamplers[i]);

            //SETUP PIXEL SHADER
            //-------------------
            //Shader...
            _graphicsDevice.ImmediateContext.PixelShader.Set(PixelShader);
            //Constant Buffer...
            for (int i = 0; i < _pixelShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(i, _pixelShaderConstantBuffers[i]);
            //Resources (Textures)...
            for (int i = 0; i < _pixelShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, _pixelShaderResources[i]);
            //Samplers/SamplerStates...
            for (int i = 0; i < _pixelShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetSampler(i, _pixelShaderSamplers[i]);

            //SETUP STATES
            //------------
            //Blend State
            if (BlendState != null)
            {
                _oldBlendState = _graphicsDevice.ImmediateContext.OutputMerger.BlendState;
                _graphicsDevice.ImmediateContext.OutputMerger.BlendState = BlendState;
            }
            //Rasterizer State
            if (RasterizerState != null)
            {
                _oldRasterizerState = _graphicsDevice.ImmediateContext.Rasterizer.State;
                _graphicsDevice.ImmediateContext.Rasterizer.State = RasterizerState;
            }
            //DepthStencilState
            if (DepthStencilState != null)
            {
                _oldDepthStencilState = _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState;
                _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = DepthStencilState;
            }                         
        }

        public void CleanUp()
        {
            //CLEAN UP VERTEX SHADER
            //----------------------
            //Shader...
            _graphicsDevice.ImmediateContext.VertexShader.Set(null);
            //Constant Buffer...
            for (int i = 0; i < _vertexShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetConstantBuffer(i, null);
            //Resources (Textures)...
            for (int i = 0; i < _vertexShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetShaderResource(i, null);
            //Samplers/SamplerStates...
            for (int i = 0; i < _vertexShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.VertexShader.SetSampler(i, null);

            //CLEAN UP PIXEL SHADER
            //---------------------
            //Shader...
            _graphicsDevice.ImmediateContext.PixelShader.Set(null);
            //Constant Buffer...
            for (int i = 0; i < _pixelShaderConstantBuffers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetConstantBuffer(i, null);
            //Resources (Textures)...
            for (int i = 0; i < _pixelShaderResources.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetShaderResource(i, null);
            //Samplers/SamplerStates...
            for (int i = 0; i < _pixelShaderSamplers.Count; i++)
                _graphicsDevice.ImmediateContext.PixelShader.SetSampler(i, null);

            //CLEAN UP STATES
            //---------------   
            //Blend State
            if (BlendState != null)
                _graphicsDevice.ImmediateContext.OutputMerger.BlendState = _oldBlendState;            
            //Rasterizer State
            if (RasterizerState != null)                          
                _graphicsDevice.ImmediateContext.Rasterizer.State = _oldRasterizerState;            
            //DepthStencilState
            if (DepthStencilState != null)
                _graphicsDevice.ImmediateContext.OutputMerger.DepthStencilState = _oldDepthStencilState;           
        }
    }
}
