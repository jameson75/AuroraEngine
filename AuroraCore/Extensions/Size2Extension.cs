using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Size2Extension
    {
        private static Size2 _zero = new Size2(0, 0);

        public static Size2 Zero { get { return _zero; } }

        public static Size2 Add(this Size2 s, Size2 s2)
        {
            return new Size2(s.Width + s2.Width, s.Height + s2.Height);
        }

        public static Size2F ToSize2F(this Size2 size)
        {
            return new Size2F(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static Vector2 ToVector2(this Size2 size)
        {
            return new Vector2(Convert.ToSingle(size.Width), Convert.ToSingle(size.Height));
        }

        public static float AspectRatio(this Size2 sizeF)
        {
            return (float)sizeF.Width / (float)sizeF.Height;
        }
    }


}
