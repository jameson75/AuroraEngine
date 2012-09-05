using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using CipherPark.AngelJacket.Core.Utils;

namespace CipherPark.AngelJacket.Core.UI.Controls
{
    public class ColorContent : UIContent
    {
        Color4 _color = Colors.Transparent;

        public ColorContent()
        { }

        public ColorContent(Color4 color)
        {
            _color = color;
        }

        public override void Draw(long gameTime)
        {
            if (Container == null)
                throw new InvalidOperationException("No container for this content was specified.");

            if (_color != Colors.Transparent)
            {
                Texture2D backgroundTexture = new Texture2D(Container.Game.GraphicsDevice, (int)Container.Size.X, (int)Container.Size.Y);
                int dataLength = (int)Container.Size.X * (int)Container.Size.Y;
                Color4[] colorData = new Color4[dataLength];
                for (int i = 0; i < dataLength; i++)
                    colorData[i] = _color;
                backgroundTexture.SetData<Color4>(colorData);
                Rectangle screenRectangle = Container.BoundsToSurface(Container.Bounds);
                Container.ControlSpriteBatch.Begin();
                Container.ControlSpriteBatch.Draw(backgroundTexture, screenRectangle, Color4.White);
                Container.ControlSpriteBatch.End();
            }
        }

        public override Rectangle CalculateSmallestBoundingRect()
        {
            return Rectangle.Empty;
        }
        
    }
}
