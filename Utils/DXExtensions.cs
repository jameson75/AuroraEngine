using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.DirectInput;

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
        public static bool IsKeyDown(this KeyboardState ks, VirtualKey key)
        {
            return ks.PressedKeys.Contains(DirectInputVKMap.ToDirectInputKey(key));
        }

        public static bool IsKeyUp(this KeyboardState ks, VirtualKey key)
        {
            return !IsKeyDown(ks, key);
        }
    }

    public static class RectangleFExtension
    {
        public static DrawingSizeF GetSize(this RectangleF r)
        {
            return new DrawingSizeF(r.Right - r.Left, r.Bottom - r.Top);
        }

        public static Vector2 Position(this RectangleF r)
        {
            return new Vector2(r.Left, r.Top);
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
    }

}
