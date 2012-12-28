using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils.Toolkit;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public abstract class UIContent
    {
        public UIContent()
        {
            
        }
        
        public SpriteSortMode? SpriteSortMode { get; set; }

        public BlendState BlendState { get; set; }

        public SamplerState SamplerState { get; set; }

        public DepthStencilState DepthStencilState { get; set; }

        public RasterizerState RasterizerState { get; set; }

        public Action CustomShaderCallback { get; set; }

        public Matrix? TransformationMatrix { get; set; }

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

        public virtual void Draw(long gameTime)
        { 

        }

        //TODO: Deprecate this method.
        public virtual void Load(string path)
        {

        }

        public UIControl Container { get; set; }

        public abstract Rectangle CalculateSmallestBoundingRect();

        public virtual void ApplyTemplate(Components.UIContentTemplate template)
        {

        }
    }
}
