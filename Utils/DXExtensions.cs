using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.DirectInput;

///////////////////////////////////////////////////////////////////////////////
// Developer: Eugene Adams
// Company: Cipher Park
// Copyright © 2010-2013
// Angel Jacket by Cipher Park is licensed under 
// a Creative Commons Attribution-NonCommercial-NoDerivs 3.0 Unported License.
///////////////////////////////////////////////////////////////////////////////

namespace CipherPark.AngelJacket.Core.Utils
{
    public static class Vector2Extension
    {
        public static Vector2 Size(this Rectangle r)
        {
            return new Vector2(r.Width, r.Height);
        }
    }

    public static class RectangleExtension
    {
        public static bool Contains(this Rectangle r, DrawingPoint point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static Vector2 Position(this Rectangle r)
        {
            return new Vector2(r.X, r.Y);
        }

        public static DrawingSize GetSize(this Rectangle r)
        {
            return new DrawingSize(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Rectangle CreateLTWH(int l, int t, int width, int height)
        {
            return new Rectangle(l, t, l + width, t + height);
        }
    }

    public static class MouseStateExtension
    {
        public static ButtonState LeftButton(this MouseState ms)
        {
            return ms.Buttons[0] ? ButtonState.Pressed : ButtonState.Released;
        }

        public static ButtonState RightButton(this MouseState ms)
        {
            return ms.Buttons[1] ? ButtonState.Pressed : ButtonState.Released;
        }

        public static ButtonState MiddleButton(this MouseState ms)
        {
            return ms.Buttons[2] ? ButtonState.Pressed : ButtonState.Released;
        }
    }

    public static class KeyboardStateExtension
    {
        public static bool IsKeyDown(this KeyboardState ks, Key key)
        {
            return ks.PressedKeys.Contains(key);
        }

        public static bool IsKeyUp(this KeyboardState ks, Key key)
        {
            return !IsKeyDown(ks, key);
        }
    }

    public static class RectangleFExtension
    {
        public static bool Contains(this RectangleF r, DrawingPointF point)
        {
            return r.X <= point.X && r.X + r.Width >= point.X &&
                   r.Y <= point.Y && r.Y + r.Height >= point.Y;
        }

        public static DrawingSizeF GetSize(this RectangleF r)
        {
            return new DrawingSizeF(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Vector2 Position(this RectangleF r)
        {
            return new Vector2(r.Left, r.Top);
        }

        public static RectangleF CreateLTWH(float l, float t, float width, float height)
        {
            return new RectangleF(l, t, l + width, t + height);
        }
    }

    public static class DrawingSizeFExtension
    {
        private static DrawingSizeF _zero = new DrawingSizeF(0, 0);

        public static DrawingSizeF Zero { get { return _zero; } }

        public static DrawingSizeF Add(this DrawingSizeF s, DrawingSizeF s2)
        {
            return new DrawingSizeF(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static DrawingSize ToDrawingSize(this DrawingSizeF sizeF)
        {
            return new DrawingSize(Convert.ToInt32(Math.Ceiling(sizeF.Width)), Convert.ToInt32(Math.Ceiling(sizeF.Height)));
        } 

        public static Vector2 ToVector2(this DrawingSizeF sizeF)
        {
            return new Vector2(sizeF.Width, sizeF.Height);
        }
    }

    public static class DrawingSizeExtension
    {
        private static DrawingSize _zero = new DrawingSize(0, 0);

        public static DrawingSize Zero { get { return _zero; } }

        public static DrawingSize Add(this DrawingSize s, DrawingSize s2)
        {
            return new DrawingSize(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static DrawingSizeF ToDrawingSizeF(this DrawingSize size)
        {
            return new DrawingSizeF(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static Vector2 ToVector2(this DrawingSize size)
        {
            return new Vector2(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }
    }

    public static class DrawingPointExtension
    {
        private static DrawingPoint _zero = new DrawingPoint(0, 0);

        public static DrawingPoint Zero { get { return _zero; } }

        public static DrawingPoint Add(this DrawingPoint s, DrawingPoint s2)
        {
            return new DrawingPoint(s.X + s2.X, s.Y + s2.Y);
        }

        public static DrawingPointF ToDrawingPointF(this DrawingPoint point)
        {
            return new DrawingPointF(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        }

        public static Vector2 ToVector2(this DrawingPoint point)
        {
            return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }
    }

    public static class DrawingPointFExtension
    {
        private static DrawingPointF _zero = new DrawingPointF(0, 0);

        public static DrawingPointF Zero { get { return _zero; } }

        public static DrawingPoint ToDrawingPoint(this DrawingPointF pointF)
        {
            return new DrawingPoint(Convert.ToInt32(Math.Floor(pointF.X)), Convert.ToInt32(Math.Floor(pointF.Y)));
        }

        public static DrawingPointF Add(this DrawingPointF s, DrawingPointF s2)
        {
            return new DrawingPointF(s.X + s2.X, s.Y + s2.Y);
        }

        public static Vector2 ToVector2(this DrawingPointF pointF)
        {
            return new Vector2(pointF.X, pointF.Y);
        }
    }

    public static class BoundingBoxExtension
    {
        private static BoundingBox _empty = new BoundingBox(Vector3.Zero, Vector3.Zero);

        public static BoundingBox Transform(this BoundingBox boundingBox, Matrix matrix)
        {
            Vector4 vMin = Vector3.Transform(boundingBox.Minimum, matrix);
            Vector4 vMax = Vector3.Transform(boundingBox.Maximum, matrix);
            return new BoundingBox(new Vector3(vMin.X, vMin.Y, vMin.Z), new Vector3(vMax.X, vMax.Y, vMax.Z));
        }

        public static BoundingBox Empty { get { return _empty; } }    
    }
}
