using System;
using SharpDX;

namespace CipherPark.Aurora.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class PointExtension
    {
        private static Point _zero = new Point(0, 0);

        public static Point Zero { get { return _zero; } }

        public static Point Add(this Point s, Point s2)
        {
            return new Point(s.X + s2.X, s.Y + s2.Y);
        }

        /* Vector2 is deprecated */
        //
        //public static Vector2 ToVector2(this Point point)
        //{
        //    return new Vector2(Convert.ToInt32(point.X), Convert.ToInt32(point.Y));
        //}

        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(Convert.ToSingle(point.X), Convert.ToSingle(point.Y));
        }
    }


}
