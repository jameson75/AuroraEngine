﻿using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.Aurora.Core.Toolkit;
using CipherPark.Aurora.Core.Extensions;

namespace CipherPark.Aurora.Core.World
{
    public class SpriteBatchContext
    {
        private DeviceContext _graphicsDeviceContext = null;
        private Action internalCallback = null;
        private SpriteBatch _renderer = null;
        private List<Sprite> _sprites = null;
        
        public SpriteBatchContext(DeviceContext graphicsDeviceContext)
        {
            _graphicsDeviceContext = graphicsDeviceContext;
            _renderer = new SpriteBatch(graphicsDeviceContext);
            _sprites = new List<Sprite>();
            BlendFactor = Color.Zero;
            internalCallback = new Action(this.InternalShaderCallback);
        }

        public List<Sprite> Sprites { get { return _sprites; } }

        public Color4 BlendFactor { get; set; }

        public SpriteSortMode? SpriteSortMode { get; set; }

        public BlendState BlendState { get; set; }

        public SamplerState SamplerState { get; set; }

        public DepthStencilState DepthStencilState { get; set; }

        public RasterizerState RasterizerState { get; set; }

        public Action CustomShaderCallback { get; set; }

        public Matrix? TransformationMatrix { get; set; }                       

        public void Draw()
        {
            OnBeginDraw();

            if (!HasDrawParameters)
                _renderer.Begin();
            else
                _renderer.Begin(SpriteSortMode == null ? CipherPark.Aurora.Core.Toolkit.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, internalCallback, TransformationMatrix);

            for (int i = 0; i < _sprites.Count; i++)   
                if(_sprites[i].Visible)
                    _renderer.Draw(_sprites[i].Texture, _sprites[i].Position, _sprites[i].SourceRectangle, _sprites[i].Tint, _sprites[i].Rotation, _sprites[i].Origin, _sprites[i].Scale, _sprites[i].SpriteEffects, _sprites[i].Layer);
                
            _renderer.End();

            OnEndDraw();
        }    

        protected bool HasDrawParameters
        {
            get
            {
                return SpriteSortMode != null ||
                       BlendState != null ||
                       SamplerState != null ||
                       DepthStencilState != null ||
                       RasterizerState != null ||
                       CustomShaderCallback != null ||
                       TransformationMatrix != null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InternalShaderCallback()
        {
            //Work around. Shader overwrites BlendFactor so we set it here, during the shader callback.
            this._graphicsDeviceContext.OutputMerger.BlendFactor = BlendFactor;

            if (CustomShaderCallback != null)
                CustomShaderCallback();
        }

        private BlendState _cachedBlendState = null;
        private DepthStencilState _cachedDepthStencilState = null;
        private RasterizerState _cachedRasterizerState = null;

        protected virtual void OnBeginDraw()
        {
            _cachedBlendState = _graphicsDeviceContext.OutputMerger.BlendState;
            _cachedDepthStencilState = _graphicsDeviceContext.OutputMerger.DepthStencilState;
            _cachedRasterizerState = _graphicsDeviceContext.Rasterizer.State;
        }

        protected virtual void OnEndDraw()
        {
            _graphicsDeviceContext.OutputMerger.BlendState = _cachedBlendState;
            _graphicsDeviceContext.OutputMerger.DepthStencilState = _cachedDepthStencilState;
            _graphicsDeviceContext.Rasterizer.State = _cachedRasterizerState;
        }
    }

    public class Sprite : ISprite
    {
        public Sprite()
        {
            Texture = null;
            Position = Vector2.Zero;                    
            Tint = Color.White;
            SourceRectangle = null;
            SpriteEffects = SpriteEffects.None;
            Rotation = 0;
            Origin = Vector2.Zero;
            Scale = Vector2.One;
            Visible = true;
        }        

        public Vector2 Position { get; set; }

        public Size2 Size
        {
            get
            {
                if (Texture == null)
                    return Size2Extension.Zero;
                else
                {
                    Texture2DDescription desc = Texture.GetTextureDescription();
                    return new Size2(desc.Width, desc.Height);
                }
            }
        }
        
        public ShaderResourceView Texture { get; set; }

        public Color Tint { get; set; }

        public RectangleF Bounds { get { return new RectangleF(Position.X, Position.Y, this.Size.Width, this.Size.Height); } }

        public Rectangle? SourceRectangle { get; set; }

        public float Rotation { get; set; }

        public Vector2 Origin { get; set; }

        public Vector2 Scale { get; set; }

        public SpriteEffects SpriteEffects { get; set; }

        public int Layer { get; set; }

        public bool Visible { get; set; }
    }
}
