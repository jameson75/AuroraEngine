using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Utils
{
    public static class QuadDimensions
    {
        public static RectangleF FromCenter(float width, float height)
        {
            float halfWidth = width / 2.0f;
            float halfHeight = height / 2.0f;
            return new RectangleF(-halfWidth, halfHeight, width, -height);
        }

        public static RectangleF FromOrigin(Vector2 origin, float width, float height)
        {
            if (origin.X > width)
                throw new ArgumentException("origin x component cannot be greater than width");

            if (origin.Y > height)
                throw new ArgumentException("origin y component cannot be greater than height");
            
            return new RectangleF(-origin.X, height - origin.Y, width, -height);
        }

        public static RectangleF FromBottomCenter(float width, float height)
        {
            return FromOrigin(new Vector2(width / 2f, 0), width, height);
        }

        public static RectangleF FromBottomLeft(float width, float height)
        {
            return FromOrigin(Vector2.Zero, width, height);
        }

        public static RectangleF FromCenterLeft(float width, float height)
        {
            return FromOrigin(new Vector2(0, height / 2f), width, height);
        }

        public static RectangleF FromTopLeft(float width, float height)
        {
            return FromOrigin(new Vector2(0, height), width, height);
        }

        public static RectangleF FromTopCenter(float width, float height)
        {
            return FromOrigin(new Vector2(width / 2f, height), width, height);
        }

        public static RectangleF FromTopRight(float width, float height)
        {
            return FromOrigin(new Vector2(width, height), width, height);
        }

        public static RectangleF FromCenterRight(float width, float height)
        {
            return FromOrigin(new Vector2(width, height / 2f), width, height);
        }

        public static RectangleF FromBottomRight(float width, float height)
        {
            return FromOrigin(new Vector2(width, 0), width, height);
        }
    }


}
