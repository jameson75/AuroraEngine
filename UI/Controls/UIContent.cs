using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class UIContent
    {
        private UIControl _container = null;
        private bool _beginDrawCallOpen = false;
        private bool _restoreRasterizerState = false;
        private bool _restoreBlendState = false;
       
    #region Rasterizer state/variable cache
        private Rectangle[] oldScissorRectangles = null;
        private RasterizerState oldRasterizerState = null;
        private BlendState oldBlendState = null;
        private Color4 oldBlendFactor = Color.Transparent;
        private Action internalCallback = null;
    #endregion

        public UIContent()
        {
            ClippingEnabled = true;
            BlendFactor = Color.Zero;
            internalCallback = new Action(InternalShaderCallback);
        }

        public Color4 BlendFactor { get; set; }

        public SpriteSortMode? SpriteSortMode { get; set; }

        public BlendState BlendState { get; set; }

        public SamplerState SamplerState { get; set; }

        public DepthStencilState DepthStencilState { get; set; }

        public RasterizerState RasterizerState { get; set; }

        public Action CustomShaderCallback { get; set; }

        public Matrix? TransformationMatrix { get; set; }      

        public bool ClippingEnabled { get; set; }

        public PredefinedBlend PredefinedBlend { get; set; }

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

        public virtual void Draw(GameTime gameTime)
        { 

        }
       
        public UIControl Container
        {
            get { return _container; }
            set 
            {
                OnContainerChanging();
                _container = value;
                OnContainerChanged();
            }
        }

        public abstract RectangleF CalculateSmallestBoundingRect();

        public virtual void ApplyStyle(Components.UIStyle style)
        { }

        protected virtual void OnContainerChanged()
        { }

        protected virtual void OnContainerChanging()
        { }

        protected void BeginDraw()
        {            
            if (_beginDrawCallOpen == true)
                throw new InvalidOperationException("Mismatch call");
             
            IGameApp game = this.Container.Game;
            RectangleF bounds = this.Container.BoundsToSurface(this.Container.Bounds);
            
            //Clipping
            //--------
            if (RasterizerState == null)
            {
                if (ClippingEnabled)
                {
                    //oldScissorRectangles = game.GraphicsDeviceContext.Rasterizer.GetScissorRectangles<Rectangle>();
                    oldRasterizerState = game.GraphicsDeviceContext.Rasterizer.State;
                    RasterizerStateDescription newRSDescription = RasterizerStateDescription.Default();                    
                    newRSDescription.IsScissorEnabled = true;
                    newRSDescription.IsMultisampleEnabled = true;                   
                    this.RasterizerState = new RasterizerState(game.GraphicsDevice, newRSDescription);
                    game.GraphicsDeviceContext.Rasterizer.SetScissorRectangle((int)bounds.Left, (int)bounds.Top, (int)bounds.Right, (int)bounds.Bottom);
                    _restoreRasterizerState = true;
                }
            }

            //Blending
            //--------
            if (BlendState == null)
            {
                //https://msdn.microsoft.com/en-us/library/bb976070.aspx

                if (PredefinedBlend == PredefinedBlend.Opacity)
                {                   
                    oldBlendState = game.GraphicsDeviceContext.OutputMerger.BlendState;
                    oldBlendFactor = game.GraphicsDeviceContext.OutputMerger.BlendFactor;
                    BlendStateDescription blendDesc = BlendStateDescription.Default();
                    blendDesc.RenderTarget[0].IsBlendEnabled = true;
                    blendDesc.RenderTarget[0].SourceBlend = BlendOption.BlendFactor;
                    blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                    blendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseBlendFactor;
                    blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    this.BlendState = new BlendState(game.GraphicsDevice, blendDesc);     
                    _restoreBlendState = true;                                
                }
                else if(PredefinedBlend == PredefinedBlend.Additive)
                {                    
                    oldBlendState = game.GraphicsDeviceContext.OutputMerger.BlendState;
                    oldBlendFactor = game.GraphicsDeviceContext.OutputMerger.BlendFactor;
                    BlendStateDescription blendDesc = BlendStateDescription.Default();
                    blendDesc.RenderTarget[0].IsBlendEnabled = true;
                    blendDesc.RenderTarget[0].SourceBlend =  BlendOption.BlendFactor;
                    blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                    blendDesc.RenderTarget[0].DestinationBlend = BlendOption.One;
                    blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    this.BlendState = new BlendState(game.GraphicsDevice, blendDesc);
                    _restoreBlendState = true;
                }
                else if(PredefinedBlend == PredefinedBlend.LightMap)
                {
                    oldBlendState = game.GraphicsDeviceContext.OutputMerger.BlendState;
                    oldBlendFactor = game.GraphicsDeviceContext.OutputMerger.BlendFactor;
                    BlendStateDescription blendDesc = BlendStateDescription.Default();
                    blendDesc.RenderTarget[0].IsBlendEnabled = true;
                    blendDesc.RenderTarget[0].SourceBlend = BlendOption.Zero;
                    blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.Zero;
                    blendDesc.RenderTarget[0].DestinationBlend = BlendOption.SourceColor;
                    blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.One;                   
                    this.BlendState = new BlendState(game.GraphicsDevice, blendDesc);
                    _restoreBlendState = true;
                }
                else if (PredefinedBlend == PredefinedBlend.AlphaBlend)
                {
                    oldBlendState = game.GraphicsDeviceContext.OutputMerger.BlendState;
                    oldBlendFactor = game.GraphicsDeviceContext.OutputMerger.BlendFactor;
                    BlendStateDescription blendDesc = BlendStateDescription.Default();
                    blendDesc.RenderTarget[0].IsBlendEnabled = true;
                    blendDesc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
                    blendDesc.RenderTarget[0].SourceAlphaBlend = BlendOption.One;
                    blendDesc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                    blendDesc.RenderTarget[0].DestinationAlphaBlend = BlendOption.InverseSourceAlpha;
                    this.BlendState = new BlendState(game.GraphicsDevice, blendDesc);
                    _restoreBlendState = true;
                } 
                game.GraphicsDeviceContext.OutputMerger.BlendFactor = this.BlendFactor;
            }           

            if (!HasDrawParameters)
                Container.ControlSpriteBatch.Begin();
            else
                Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Toolkit.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, internalCallback, TransformationMatrix);                                               

            _beginDrawCallOpen = true;
        }

        protected void EndDraw()
        {            
            if (_beginDrawCallOpen == false)
                throw new InvalidOperationException("Mismatch call");

            Container.ControlSpriteBatch.End();

            if (_restoreRasterizerState)
            {                
                this.Container.Game.GraphicsDeviceContext.Rasterizer.State = oldRasterizerState;
                this.Container.Game.GraphicsDeviceContext.Rasterizer.SetScissorRectangles(oldScissorRectangles);
                oldRasterizerState = null;
                oldScissorRectangles = null;
                this.RasterizerState = null;
                _restoreRasterizerState = false;
            }

            if (_restoreBlendState)
            {
                this.Container.Game.GraphicsDeviceContext.OutputMerger.BlendState = oldBlendState;
                this.Container.Game.GraphicsDeviceContext.OutputMerger.BlendFactor = oldBlendFactor;
                oldBlendState = null;
                oldBlendFactor = Color.Zero;
                this.BlendState = null;               
                _restoreBlendState = false;
            }      

            _beginDrawCallOpen = false;
        }

        private void InternalShaderCallback()
        {
            //Work around. SpriteBatch in directX tool kit overwrites BlendFactor so we set it here, during the shader callback.
            this.Container.Game.GraphicsDeviceContext.OutputMerger.BlendFactor = BlendFactor;
            if (CustomShaderCallback != null)
                CustomShaderCallback();
        }
    }


    public static class UIContentExtension
    {
        public static T As<T>(this UIContent content) where T : UIContent
        {
            if (typeof(T).IsAssignableFrom(content.GetType()))
                return (T)content;
            else if (content is LayeredContent)
            {
                LayeredContent layeredContent = (LayeredContent)content;
                foreach (UIContent child in layeredContent.ChildContents)
                    if (typeof(T).IsAssignableFrom(child.GetType()))
                        return (T)child;
            }

            throw new InvalidOperationException("Cannot cast content to specified type");
        }
    }

    public enum PredefinedBlend
    {
        None,
        Opacity,
        Additive,
        LightMap,
        AlphaBlend
    }}
