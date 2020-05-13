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
                    //_texture.Dispose();
                    _texture = null;
                }

                _texture = value;
             }
        }

        public bool ScaleImage { get; set; }
       
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
                if (!ScaleImage)
                    Container.ControlSpriteBatch.Draw(_textureView, Container.PositionToSurface(Container.Position).ToVector2(), SharpDX.Color.White);
                else
                    Container.ControlSpriteBatch.Draw(_textureView, Container.BoundsToSurface(Container.Bounds).ToRectangle(), new Rectangle(0, 0, _textureView.GetTexture2DSize().Width, _textureView.GetTexture2DSize().Height), Color.White);
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

        public override void ApplyStyle(UIStyle style)
        {
            ImageStyle imageStyle = style as ImageStyle;

            if (imageStyle == null)
                throw new ArgumentException("Style is not of type ImageStyle", "imageStyle");

            if (imageStyle.Texture != null)
                this.Texture = imageStyle.Texture;

            base.ApplyStyle(style);
        }
    }
}
