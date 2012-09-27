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
        public Texture2D Texture { get; set; }

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

            if (Texture != null)
            {
                Container.ControlSpriteBatch.Begin();
                Container.ControlSpriteBatch.Draw(Texture, Container.PositionToSurface(Container.Position), Colors.White);
                Container.ControlSpriteBatch.End();
            }

            base.Draw(gameTime);
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            if (Texture == null)
                return Rectangle.Empty;
            else
                return new Rectangle(0, 0, Texture.Description.Width, Texture.Description.Height);
        }
    }
}
