using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;
using SharpDX.Direct3D11;
using CipherPark.AngelJacket.Core.Utils;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ImageContent : UIContent
    {       
        private ShaderResourceView _textureView = null;
        private Texture2D _texture = null;

        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                if (_textureView != null)
                {
                    _textureView.Dispose();
                    _textureView = null;
                }

                if (_texture != null)
                {
                    _texture.Dispose();
                    _texture = null;
                }

                _texture = value;
             }
        }
       
        public ImageContent()
        { }

        public ImageContent(Texture2D texture) 
        {
            Texture = texture;
        }

        public override void Draw(GameTime gameTime)
        {
            if (this.Container == null)
                throw new InvalidOperationException("Container is null. Container must be specified before calling Draw method.");

            if (_texture != null)
            {
                if (_textureView == null)
                    _textureView = new ShaderResourceView(Container.Game.GraphicsDevice, _texture);
                this.BeginDraw();
                //if (!HasDrawParameters)
                //    Container.ControlSpriteBatch.Begin();
                //else
                //    Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Toolkit.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, CustomShaderCallback, TransformationMatrix);                                
                Container.ControlSpriteBatch.Draw(_textureView, Container.PositionToSurface(Container.Position).ToVector2(), SharpDX.Color.White);              
                // Container.ControlSpriteBatch.End();
                this.EndDraw();
            }

            base.Draw(gameTime);
        }       

        public override RectangleF CalculateSmallestBoundingRect()
        {
            if (Texture == null)
                return RectangleF.Empty;
            else
                return new RectangleF(0, 0, Texture.Description.Width, Texture.Description.Height);
        }
    }
}
