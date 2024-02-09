using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class Vector2Extension
    {
        private static Vector2 _zero = new Vector2(0, 0);

        public static Vector2 Zero { get { return _zero; } }

        public static Point ToPoint(this Vector2 pointF)
        {
            return new Point(Convert.ToInt32(Math.Floor(pointF.X)), Convert.ToInt32(Math.Floor(pointF.Y)));
        }

        public static Vector2 Add(this Vector2 s, Vector2 s2)
        {
            return new Vector2(s.X + s2.X, s.Y + s2.Y);
        }

        public static Vector2 Subtract(this Vector2 s, Vector2 s2)
        {
            return new Vector2(s.X - s2.X, s.Y - s2.Y);
        }

        public static Vector2 ToVector2(this Vector2 pointF)
        {
            return new Vector2(pointF.X, pointF.Y);
        }
    }


}
