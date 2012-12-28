using System;
using System.Collections.Generic;
using CipherPark.AngelJacket.Core.UI.Components;
using SharpDX;
using SharpDX.Direct3D11;

using CipherPark.AngelJacket.Core.Utils;

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

                if( _texture != null )                
                    _textureView = new ShaderResourceView(Container.Game.GraphicsDevice, _texture);                
            }
        }
       
        public ImageContent()
        { }

        public ImageContent(Texture2D texture) 
        {
            Texture = texture;
        }

        public override void Draw(long gameTime)
        {
            if (this.Container == null)
                throw new InvalidOperationException("Container is null");

            if (_textureView != null)
            {
                if (!HasDrawParameters)
                    Container.ControlSpriteBatch.Begin();
                else
                    Container.ControlSpriteBatch.Begin(SpriteSortMode == null ? CipherPark.AngelJacket.Core.Utils.Toolkit.SpriteSortMode.Deferred : SpriteSortMode.Value, BlendState, SamplerState, DepthStencilState, RasterizerState, CustomShaderCallback, TransformationMatrix);
                Container.ControlSpriteBatch.Draw(_textureView, Container.PositionToSurface(Container.Position).ToVector2(), SharpDX.Color.White);              
                Container.ControlSpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            if (Texture == null)
                return Rectangle.Empty;
            else
                return RectangleExtension.CreateLTWH(0, 0, Texture.Description.Width, Texture.Description.Height);
        }
    }
}
